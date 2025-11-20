using Microsoft.EntityFrameworkCore;
using RPA_Web_Portal_Prototype_v2.Model;

namespace RPA_Web_Portal_Prototype_v2.Repositories;

public class MySqlRepository
{
    private readonly AppDbContext _context;

    public MySqlRepository(AppDbContext _context)
    {
        this._context = _context;
    }
    
    //Admin Rel Stuff starts here//
    
    //Adding a new user//
    public async Task AddUser(User newUser)
    {
        await _context.UserTable.AddAsync(newUser);
        await _context.SaveChangesAsync();
    }
    
    //Admin Rel Stuff ends here//
    
    
    //Login related stuff starts here//

    public async Task<User?> GetUserByName(string name)
    {
        var user = await _context.UserTable.FirstOrDefaultAsync(theUser => theUser.Username == name);
        return user;
    }

    public async Task<User?> GetUserById(int id)
    {
        var user = await _context.UserTable.FirstOrDefaultAsync(theUser => theUser.Id == id);
        return user;
    }
    
    public async Task<User?> GetUserByRt(string rt)
    {
        var user = await _context.UserTable.FirstOrDefaultAsync(theUser => theUser.RefreshToken == rt);
        return user;
    }
    //Login related stuff ends here//
    
    
    //RefreshToken related stuff starts here//

    //get token by name//
    public Task<RefreshToken?> GetRefreshTokenByToken(string tokenString)
    {
        var token = _context.RefreshTokens.FirstOrDefaultAsync(tkn => tkn.Token == tokenString);
        return token;
    }
    
    //user token binding//

    public async Task UserTokenBinding(User user, RefreshToken refreshToken)
    {
        var retrievedUser = await GetUserByName(user.Username);
        retrievedUser!.RefreshToken = refreshToken.Token;
        retrievedUser!.RefreshTokenExpiry = refreshToken.Expires;
        refreshToken.UserId = retrievedUser.Id;
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task UserTokenBinding(string oldRefreshToken, RefreshToken newRefreshToken)
    {
        var retrievedOldToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == oldRefreshToken);
        var retrievedUser = await GetUserById(retrievedOldToken!.UserId);
        await UserTokenBinding(retrievedUser!, newRefreshToken);
        retrievedOldToken!.ReplacedByToken = newRefreshToken.Id;
        retrievedOldToken!.Revoked = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }



    public async Task SaveTransactionToDb(TransactionBase theTransaction)
    {
        await _context.Transactions.AddAsync(theTransaction);
        await _context.SaveChangesAsync();
    }
    
    
    
    
    
    
    
    
    
    
}