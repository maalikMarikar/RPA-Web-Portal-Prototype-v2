using Microsoft.AspNetCore.Identity;
using RPA_Web_Portal_Prototype_v2.Model;

namespace RPA_Web_Portal_Prototype_v2.Services;

public class ThePasswordHasher
{
    private readonly PasswordHasher<User> _hasher;

    public ThePasswordHasher(PasswordHasher<User> _hasher)
    {
        this._hasher = _hasher;
    }

    public string HashThePassword(User theUser, string newUserUnhashedPw)
    {
        var newHashedPw = _hasher.HashPassword(theUser, newUserUnhashedPw);
        return newHashedPw;
    }

    public bool VerifyPw(User theUser, string userProvidedPw)
    {
        var result = _hasher.VerifyHashedPassword(theUser, theUser.PasswordHash, userProvidedPw);
        return result != PasswordVerificationResult.Failed;
    }
    
}