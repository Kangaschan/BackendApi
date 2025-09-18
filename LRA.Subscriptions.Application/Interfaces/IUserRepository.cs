using LRA.Subscriptions.Application.DTOs;
using LRA.Subscriptions.Application.DTOs.Request;
using LRA.Subscriptions.Application.DTOs.Response;

namespace LRA.Subscriptions.Application.Interfaces;

public interface IUserRepository
{
    Task<UserResopnseDto?> GetByMailAsync(string mail, CancellationToken cancellationToken);
    Task<UserResopnseDto?> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken);
    Task CreateUserAsync(UserCreateDto user, CancellationToken cancellationToken);
}
