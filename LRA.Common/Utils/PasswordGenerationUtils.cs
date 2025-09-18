using System.Security.Cryptography;

namespace LRA.Common.Utils;

public static class PasswordGenerationUtils
{
    private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
    private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string NumericChars = "0123456789";
    private const string SpecialChars = "!@#$%^&*()_+-=[]{};':|,.<>?";
    private const string AllAllowedChars = LowercaseChars + UppercaseChars + NumericChars + SpecialChars;
    
    private static char GetRandomChar(RandomNumberGenerator rng, string charSet)
    {
        return charSet[GetRandomInt(rng, charSet.Length)];
    }
    
    private static int GetRandomInt(RandomNumberGenerator rng, int max = int.MaxValue)
    {
        var data = new byte[4];
        rng.GetBytes(data);
        var randomValue = BitConverter.ToUInt32(data, 0);
        return (int)(randomValue % max);
    }
    public static string GenerateSecurePassword()
    {
        using var rng = RandomNumberGenerator.Create();
        
        var passwordChars = new char[12]; 
        passwordChars[0] = GetRandomChar(rng, LowercaseChars);
        passwordChars[1] = GetRandomChar(rng, UppercaseChars);
        passwordChars[2] = GetRandomChar(rng, NumericChars);
        passwordChars[3] = GetRandomChar(rng, SpecialChars);
        
        for (int i = 4; i < passwordChars.Length; i++)
        {
            passwordChars[i] = GetRandomChar(rng, AllAllowedChars);
        }
        
        return new string(passwordChars.OrderBy(c => GetRandomInt(rng)).ToArray());
    }
}
