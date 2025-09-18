using LRA.Account.Application.Configuration;
using LRA.Account.Application.DTOs;
using LRA.Account.Application.Interfaces;
using LRA.Account.Domain.Models;
using LRA.Common.DTOs;
using LRA.Common.DTOs.Auth;
using LRA.Common.DTOs.KYC;
using LRA.Common.Enums;
using LRA.Common.Exceptions;
using LRA.Common.Models;
using Microsoft.Extensions.Options;
using ChangeEmailRequest = LRA.Account.Application.DTOs.ChangeEmailRequest;
using ChangePasswordRequest = LRA.Account.Application.DTOs.ChangePasswordRequest;
using CompleteResetPasswordRequest = LRA.Account.Application.DTOs.CompleteResetPasswordRequest;

namespace LRA.Account.Application.Implementation;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
    private readonly IKeycloakServiceClient _keycloakServiceClient;
    private readonly IMailSendService _mailSendService;
    private readonly TemporaryPasswordSettings _temporaryPasswordSettings;
    private readonly IAccountImageService _accountImageService;
    private readonly IKycRepository _kycRepository;
    
    public AccountService(
        IAccountRepository accountRepository,
        IKeycloakServiceClient keycloakServiceClient,
        IMailSendService mailSendService,
        ITokenRepository tokenRepository,
        IOneTimePasswordRepository oneTimePasswordRepository,
        IOptions<TemporaryPasswordSettings> temporaryPasswordSettings,
        IAccountImageService accountImageService,
        IKycRepository kycRepository)
    {
        _accountRepository = accountRepository;
        _keycloakServiceClient = keycloakServiceClient;
        _mailSendService = mailSendService;
        _tokenRepository = tokenRepository;
        _oneTimePasswordRepository = oneTimePasswordRepository;
        _temporaryPasswordSettings = temporaryPasswordSettings.Value;
        _accountImageService = accountImageService;
        _kycRepository = kycRepository;
    }
    
    public async Task<IEnumerable<AccountDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _accountRepository.GetAllAsync(cancellationToken);
    }

    public async Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _accountRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<PagedResult<AccountSearchResult>> GetFilteredAsync(ViewFilterParams filterParams, CancellationToken cancellationToken)
    {
        return await _accountRepository.GetFilteredAsync(filterParams, cancellationToken);
    }

    public async Task<PagedResult<AccountSearchResult>> SearchAsync(AccountSearchParams searchParams, CancellationToken cancellationToken)
    {
        return await _accountRepository.SearchAsync(searchParams, cancellationToken);
    }
    
    public async Task CreateAsync(CreateAccountRequestDto account, CancellationToken cancellationToken)
    {
        var exist = await _accountRepository.GetbyEmail(account.Email, cancellationToken);
        if (exist != null)
        {
            throw new InvalidOperationException($"Account with email {account.Email} already exists");
        }
        var keycloakAccountCreateRequest = new KeycloakAccountCreateRequest
        {
            Email = account.Email,
            Enabled = true,
            Credentials = new List<KeycloakCredential>
            {
                new KeycloakCredential { type = "password", value = account.Password, temporary = false }
            }
        };
        
        var keycloakId = await _keycloakServiceClient.CreateAccountAsync(keycloakAccountCreateRequest, cancellationToken);

        var accountDto = new AccountDto
        {
            FirstName = account.FirstName,
            Email = account.Email,
            LastName = account.LastName,
            Phone = account.Phone,
            Roles = account.Roles,
            KeycloakId = keycloakId,
            IsTemporaryPassword = account.IsTemporaryPassword,
            IsTemporaryPasswordUsed = false,
            ExpiresAt = DateTime.UtcNow.AddHours(_temporaryPasswordSettings.ExpirationHours)
        };
        
        await _accountRepository.CreateAsync(accountDto, cancellationToken);
    }
    
    public async Task RegisterAsync(RegisterAccountDto account, CancellationToken cancellationToken)
    {
        var keycloakAccountCreateRequest = new KeycloakAccountCreateRequest
        {
            Email = account.Email,
            Enabled = false,
            Credentials = new List<KeycloakCredential>
            {
                new KeycloakCredential { type = "password", value = account.Password, temporary = false }
            }
        };
        
        var keycloakId = await _keycloakServiceClient.CreateAccountAsync(keycloakAccountCreateRequest, cancellationToken);

        var accountDto = new AccountDto
        {
            Email = account.Email,
            KeycloakId = keycloakId,
            Roles = new List<string>{"client"},
            IsTemporaryPassword = false,
        };
            
        await _accountRepository.CreateAsync(accountDto, cancellationToken);
        
        var confiramtionToken = new EmailConfirmationTokenDto
        {
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            UserEmail = account.Email
        };
        
        var token = await _tokenRepository.CreateToken(confiramtionToken, cancellationToken);
        var mailMessage = new EmailMessage
        {
            Content = new EmailСontent
            {
                Body = token,
                Subject = "account mail confirmation"
            },
            RecipientEmail = account.Email,
        };
        
        await _mailSendService.SendMailAsync(mailMessage, cancellationToken);
    }

    public async Task<bool> ConfirmEmailAsync(TokenDto dto, CancellationToken cancellationToken)
    {
        var emailConfirmationToken = await _tokenRepository.CheckToken(dto, cancellationToken);
        if (emailConfirmationToken.ExpiresAt > DateTime.UtcNow && emailConfirmationToken.UserEmail == dto.Email)
        {
            var account = await _accountRepository.GetbyEmail(dto.Email, cancellationToken);
            var updateDto = new KeycloakAccountUpdateRequest
            {
                Email = emailConfirmationToken.UserEmail,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Enabled = true,
            };
            
            await _keycloakServiceClient.ActivateAccountAsync(account.KeycloakId, cancellationToken);

            return true;
        }
        return false;
    }

    public async Task<(JwtTokenDto?, bool)> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetbyEmail(loginRequest.Email, cancellationToken);
        if (account.IsBlocked)
        {
            throw new Exception("Account is blocked");
        }
        var keycloakToken = await _keycloakServiceClient.LoginAccountAsync(loginRequest, cancellationToken);
        if (account.IsTemporaryPassword && account.IsTemporaryPasswordUsed)
        {
             return (null, false);
        }
        if (account.IsTwoFactorEnabled)
        {
            var oneTimePassword = new OneTimePasswordDto
            {
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                UserEmail = account.Email,
            };
            
            var password = await _oneTimePasswordRepository.CreatePassword(oneTimePassword, cancellationToken);
            var mailMessage = new EmailMessage
            {
                Content = new EmailСontent
                {
                    Body = password,
                    Subject = "one time password"
                },
                RecipientEmail = account.Email,
            };
            
            await _mailSendService.SendMailAsync(mailMessage, cancellationToken);
            throw new Required2FaException("2FA required");
        }
        
        if(account.IsTemporaryPassword)
        {
            var credetialsUpdateDto = new UpdateAccountCredentialsRequest
            {
                IsTemporaryPassword = true,
                IsTemporaryPasswordUsed = true,
                ExpiresAt = account.ExpiresAt
            };
            await _accountRepository.UpdateCredentialsAsync(account.Email, credetialsUpdateDto, cancellationToken);
        }
        var token = new JwtTokenDto()
        {
            AccessToken = keycloakToken.AccessToken,
            RefreshToken = keycloakToken.RefreshToken,
        };
        return (token, true);
    }
    
    public async Task<(JwtTokenDto?, bool)> Complete2FaAsync(Complete2FaRequest dto, CancellationToken cancellationToken)
    {
        var tokenDto = new TokenDto
        {
            Email = dto.Email,
            Token = dto.OneTimePassword
        };
        var oneTimePassword = await _oneTimePasswordRepository.CheckPassword(tokenDto, cancellationToken);
        if (oneTimePassword.ExpiresAt > DateTime.UtcNow && oneTimePassword.UserEmail == dto.Email)
        {
            var loginRequest = new LoginRequest
            {
                Email = dto.Email,
                Password = dto.Password
            };
            var keycloakToken = await _keycloakServiceClient.LoginAccountAsync(loginRequest, cancellationToken);

            var accessToken = new JwtTokenDto
            {
                AccessToken = keycloakToken.AccessToken,
                RefreshToken = keycloakToken.RefreshToken,
            };
            
            return (accessToken, true);
        }
        var account = await _accountRepository.GetbyEmail(dto.Email, cancellationToken);
        if (account.IsTemporaryPassword && !account.IsTemporaryPasswordUsed)
        {
            var credetialsUpdateDto = new UpdateAccountCredentialsRequest
            {
                IsTemporaryPassword = account.IsTemporaryPassword,
                IsTemporaryPasswordUsed = true,
                ExpiresAt = account.ExpiresAt
            };
            await _accountRepository.UpdateCredentialsAsync(account.Email, credetialsUpdateDto, cancellationToken);
        }
        return (null, false);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetbyEmail(resetPasswordRequest.Email, cancellationToken);
        
        var oneTimePassword = new OneTimePasswordDto
        {
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            UserEmail = account.Email,
        };
            
        var password = await _oneTimePasswordRepository.CreatePassword(oneTimePassword, cancellationToken);
        var mailMessage = new EmailMessage
        {
            Content = new EmailСontent
            {
                Body = password,
                Subject = "one time password for password reset"
            },
            RecipientEmail = account.Email,
        };
        
        await _mailSendService.SendMailAsync(mailMessage, cancellationToken);
    }

    public async Task<bool> CompletePasswordResetAsync(CompleteResetPasswordRequest completeResetPasswordRequest, CancellationToken cancellationToken)
    {
        var token = new TokenDto()
        {
            Email = completeResetPasswordRequest.Email,
            Token = completeResetPasswordRequest.OneTimePassword
        };  
        var oneTimePasswordDto = await _oneTimePasswordRepository.CheckPassword(token, cancellationToken);
        if (oneTimePasswordDto.ExpiresAt > DateTime.UtcNow)
        {
            return true;
        }
        var account = await _accountRepository.GetbyEmail(completeResetPasswordRequest.Email, cancellationToken);

        await _keycloakServiceClient.ResetPasswordAsync(completeResetPasswordRequest.NewPassword, account.KeycloakId, cancellationToken);
        return false;
    }

    public async Task ChangePasswordAsync(string userMail, ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken)
    {
        var user = await _accountRepository.GetbyEmail(userMail, cancellationToken);
        var loginRequest = new LoginRequest
        {
            Password = changePasswordRequest.OldPassword,
            Email = userMail
        };
        try
        {
            var token = await _keycloakServiceClient.LoginAccountAsync(loginRequest, cancellationToken);
        }
        catch (InvalidOperationException e)
        {
            throw new InvalidOperationException("password is incorrect");
        }
        await _keycloakServiceClient.ChangePasswordAsync(changePasswordRequest, user.KeycloakId, cancellationToken);
        
        var credetialsUpdateDto = new UpdateAccountCredentialsRequest
        {
            IsTemporaryPassword = false,
            IsTemporaryPasswordUsed = false,
            ExpiresAt = DateTime.MaxValue
        };
        await _accountRepository.UpdateCredentialsAsync(user.Email, credetialsUpdateDto, cancellationToken);
    }
    
    public async Task<JwtTokenDto> RefreshToken(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken)
    {
        var keycloakToken = await _keycloakServiceClient.RefreshTokenAsync(refreshTokenDto.Token, cancellationToken);
        var token = new JwtTokenDto
        {
            AccessToken = keycloakToken.AccessToken,
            RefreshToken = keycloakToken.RefreshToken,
        };
        return token;
    }

    public async Task UpdateAsync(Guid id, AccountDto account, CancellationToken cancellationToken)
    {
        if (account.KeycloakId != null)
        {
            var updateDto = new KeycloakAccountUpdateRequest
            {
                Email = account.Email,
                LastName = account.LastName,
                FirstName = account.FirstName,
            };
            await _keycloakServiceClient.UpdateAccountAsync(account.KeycloakId, updateDto, cancellationToken);
        }
        await _accountRepository.UpdateAsync(id, account, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(id, cancellationToken);
        await _keycloakServiceClient.DeleteAccountAsync(account.KeycloakId, cancellationToken);
        await _accountRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(id, cancellationToken);
        await _keycloakServiceClient.DeleteAccountAsync(account.KeycloakId, cancellationToken);
        await _accountRepository.SoftDeleteAsync(id, cancellationToken);
    }

    public async Task<JsonWebKeysList> GetJwksAsync(CancellationToken cancellationToken)
    {
        var jsonWebKeysList = await _keycloakServiceClient.GetJwksAsync(cancellationToken);
        return jsonWebKeysList;
    }

    public async Task Update2FaAsync(string keycloakId, Change2FaRequest сhange2FaRequest, 
        CancellationToken cancellationToken)
    {
        var id = new KeycloakId
        {
            Key = keycloakId,
        };
        await _accountRepository.Update2FaAsync(id, сhange2FaRequest, cancellationToken);
    }

    public async Task ChangeFullNameAsync(string keycloakId, ChangeFullNameRequest changeFullNameRequest,
        CancellationToken cancellationToken)
    {
        var id = new KeycloakId
        {
            Key = keycloakId,
        };
        await _accountRepository.ChangeFullNameAsync(id, changeFullNameRequest, cancellationToken);
    }

    public async Task ChangeMailAsync(string keycloakId, ChangeEmailRequest changeEmailRequest, CancellationToken cancellationToken)
    {
        var id = new KeycloakId
        {
            Key = keycloakId,
        };
        var account = await _accountRepository.GetByKeycloakIdAsync(id, cancellationToken);
        var mailtakenAccount = await _accountRepository.GetbyEmail(changeEmailRequest.NewEmail, cancellationToken);
        if (mailtakenAccount == null)
        {
            throw new UserAlreadyExistsException("mail is already in use");
        }
        var loginRequest = new LoginRequest
        {
            Password = changeEmailRequest.Password,
            Email = account.Email,
        };
        var token = await _keycloakServiceClient.LoginAccountAsync(loginRequest, cancellationToken);

        if (token.AccessToken != null)
        {
            var confiramtionToken = new EmailConfirmationTokenDto
            {
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                UserEmail = changeEmailRequest.NewEmail
            };
        
            var confirmationToken = await _tokenRepository.CreateToken(confiramtionToken, cancellationToken);
            var mailMessage = new EmailMessage
            {
                Content = new EmailСontent
                {
                    Body = confirmationToken,
                    Subject = "account mail change confirmation"
                },
                RecipientEmail = account.Email,
            };
            
            await _mailSendService.SendMailAsync(mailMessage, cancellationToken);
        }
    }

    public async Task<bool> CompleteChangeMailAsync(TokenDto dto, CancellationToken cancellationToken)
    {
        var confirmationToken = await _tokenRepository.CheckToken(dto, cancellationToken);
        if (confirmationToken.ExpiresAt > DateTime.UtcNow && confirmationToken.UserEmail == dto.Email)
        {
            var account = await _accountRepository.GetbyEmail(dto.Email, cancellationToken);
            var updateDto = new KeycloakAccountUpdateRequest
            {
                Email = confirmationToken.UserEmail,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Enabled = true,
            };
            
            await _keycloakServiceClient.ActivateAccountAsync(account.KeycloakId, cancellationToken);

            await _keycloakServiceClient.CompleteEmailChangeAsync(account.KeycloakId, dto.Email, cancellationToken);
            
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<RedirectUrl> GetGoogleAuthUrl(CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            var url = _keycloakServiceClient.GetGoogleAuthUrl();
            var redirectUrl = new RedirectUrl
            {
                Url = url,
            };
            return redirectUrl;   
        }
        return null;
    }

    public async Task<JwtTokenDto> CompleteGoogleAuthAsync(string code, CancellationToken cancellationToken)
    {
        var token = await _keycloakServiceClient.ExchangeCodeForTokenAsync(code, cancellationToken);
        var result = new JwtTokenDto
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
        };
       
        var verifiedToken = await _keycloakServiceClient.GetUserInfoFromAccessTokenAsync(token.AccessToken, cancellationToken);
        
        var account = await _accountRepository.GetByKeycloakIdAsync(verifiedToken.Item1, cancellationToken);
        
        if (account == null)
        {
            var Email = await _keycloakServiceClient.GetUserEmailByIdAsync(verifiedToken.Item1, cancellationToken);
        
            var accountDto = new AccountDto
            {
                Email = Email,
                KeycloakId = verifiedToken.Item1,
            };
            
            await _accountRepository.CreateAsync(accountDto, cancellationToken);
        }
        
        return result;
    }

    public async Task<AccountRoles> GetAccountRolesAsync(string email, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetbyEmail(email, cancellationToken);

        if (account == null)
        {
            throw new KeyNotFoundException("no account found");
        }

        var accountRoles = new AccountRoles
        {
            Roles = account.Roles
        };
        return accountRoles;
    }

    public async Task<AccountRoles> GetAccountRolesByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(id, cancellationToken);

        if (account == null)
        {
            throw new KeyNotFoundException("no account found");
        }

        var accountRoles = new AccountRoles
        {
            Roles = account.Roles,
            Email = account.Email,
        };
        return accountRoles;
    }

    public async Task AdminChangePasswordAsync(string email, AdminChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetbyEmail(email, cancellationToken);
        var passwordChangeRequest = new ChangePasswordRequest
        {
            NewPassword = request.NewPassword,
            OldPassword = ""
        };
        await _keycloakServiceClient.ChangePasswordAsync(passwordChangeRequest, account.KeycloakId, cancellationToken);
        
        var credetialsUpdateDto = new UpdateAccountCredentialsRequest
        {
            IsTemporaryPassword = true,
            IsTemporaryPasswordUsed = true,
            ExpiresAt = DateTime.UtcNow.AddHours(_temporaryPasswordSettings.ExpirationHours)
        };
        await _accountRepository.UpdateCredentialsAsync(account.Email, credetialsUpdateDto, cancellationToken);
    }

    public async Task BlockAsync(Guid id, BlockRequest request, CancellationToken cancellationToken)
    {
        await _accountRepository.BlockAsync(id, request, cancellationToken);
    }

    public async Task ApplyForAJobAsync(string email, JobApplicationDto jobApplication,
        CancellationToken cancellationToken)
    {
        var accountKycs = await CheckKycStatusAsync(email, cancellationToken);
        var dontHaveActive = accountKycs.All(k => k.Status != KycStatusEnum.Pending);
        if (!dontHaveActive)
        {
            throw new ArgumentException("you've already asked for a job");
        }
        var identityDocumentPhotoUrl = await _accountImageService.UploadImageAsync(jobApplication.IdentityDocumentPhoto, cancellationToken);
        var identityDocumentSelfieUrl = await _accountImageService.UploadImageAsync(jobApplication.IdentityDocumentSelfie, cancellationToken);
        var medicalCertificatePhotoUrl = await _accountImageService.UploadImageAsync(jobApplication.MedicalCertificatePhoto, cancellationToken);
           
        var account = await _accountRepository.GetbyEmail(email, cancellationToken);
        Console.WriteLine(account==null);
        Console.WriteLine(identityDocumentPhotoUrl);
        Console.WriteLine(identityDocumentSelfieUrl);
        Console.WriteLine(medicalCertificatePhotoUrl);
        var kycCreateRequest = new KycCreateRequest
        {
            IdentityDocumentPhoto = identityDocumentPhotoUrl,
            IdentityDocumentSelfie = identityDocumentSelfieUrl,
            MedicalCertificatePhoto = medicalCertificatePhotoUrl,
            AccountId = account.Id,
            
        };
        
        await _kycRepository.CreateAsync(kycCreateRequest, cancellationToken);
    }

    public async Task<IEnumerable<KycListItemDto>> CheckKycStatusAsync(string email, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetbyEmail(email, cancellationToken);
        var kycList = await _kycRepository.GetByAccountIdAsync(account.Id, cancellationToken);
        return kycList;
    }

    public async Task<IEnumerable<KycListItemDto>> GetAllJobApplicationsAsync(CancellationToken cancellationToken)
    {
        var jobApplications = await _kycRepository.GetAllAsync(cancellationToken);
        return jobApplications;
    }

    public async Task<JobApplication> GetDetailedJobApplicationAsync(Guid id, CancellationToken cancellationToken)
    {
        var kycDetails = await _kycRepository.GetByIdAsync(id, cancellationToken);
        var memoryStream = new MemoryStream();
        var identityDocumentPhoto =await  _accountImageService.GetImageAsync(kycDetails.IdentityDocumentPhoto, cancellationToken);
        var medicalCertificatePhoto =await  _accountImageService.GetImageAsync(kycDetails.MedicalCertificatePhoto, cancellationToken);
        var identityDocumentSelfie =await  _accountImageService.GetImageAsync(kycDetails.IdentityDocumentSelfie, cancellationToken);
        
        
        identityDocumentPhoto.Content.CopyTo(memoryStream);
        
        var identityFileDto = new FileDto
        {
            ContentBase64 = Convert.ToBase64String(memoryStream.ToArray()),
            ContentType = identityDocumentPhoto.ContentType
        };
        
        identityDocumentSelfie.Content.CopyTo(memoryStream);
        
        var identitySelfieFileDto = new FileDto
        {
            ContentBase64 = Convert.ToBase64String(memoryStream.ToArray()),
            ContentType = identityDocumentPhoto.ContentType
        };
        
        medicalCertificatePhoto.Content.CopyTo(memoryStream);
        
        var MedicalFileDto = new FileDto
        {
            ContentBase64 = Convert.ToBase64String(memoryStream.ToArray()),
            ContentType = identityDocumentPhoto.ContentType
        };
        
        var jobApplication = new JobApplication
        {
            Id = kycDetails.Id,
            AccountGuid = kycDetails.AccountGuid,
            CreatedAt = kycDetails.CreatedAt,
            UpdatedAt = kycDetails.UpdatedAt,
            Status = kycDetails.Status,
            RejectReason = kycDetails.RejectReason,
            AdminReviewId = kycDetails.AdminReviewId,
            IdentityDocumentPhoto = identityFileDto,
            MedicalCertificatePhoto =MedicalFileDto,
            IdentityDocumentSelfie =identitySelfieFileDto,
        };
        return jobApplication;
    }

    public async Task ProcessJobApplicationAsync(KycProcessDto kycProcessDto, CancellationToken cancellationToken)
    {
        var kyc = await _kycRepository.GetByIdAsync(kycProcessDto.KycId, cancellationToken);
        var admin = await _accountRepository.GetbyEmail(kycProcessDto.AdminEmail, cancellationToken);
        var account = await _accountRepository.GetByIdAsync(kyc.AccountGuid, cancellationToken);

        var kycUpdate = new KycUpdateRequest
        {
            AdminReviewId = admin.Id,
            RejectReason = kyc.RejectReason,
            Status = kycProcessDto.Status,
        };

        await _kycRepository.UpdateAsync(kycProcessDto.KycId, kycUpdate, cancellationToken);

        var status = "rejected";
        if (kycProcessDto.Status == KycStatusEnum.Approved)
        {
            status = "approved";
            account.Roles.Add("paramedic");
            var accountUpdate = new AccountDto
            {
                Email = account.Email,
                KeycloakId = account.KeycloakId,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Phone = account.Phone,
                IsBlocked = account.IsBlocked,
                BlockedUntil = account.BlockedUntil,
                Roles = account.Roles,
            };
            await _accountRepository.UpdateAsync(account.Id, accountUpdate, cancellationToken);
        }

        var mailMessage = new EmailMessage
        {
            Content = new EmailСontent
            {
                Body = $"your job application is {status}",
                Subject = "Job application reviewed",
            },
            RecipientEmail = account.Email,
        };

        await _mailSendService.SendMailAsync(mailMessage, cancellationToken);
    }
}

