using Microsoft.EntityFrameworkCore;
using RPA_Web_Portal_Prototype_v2.Model;

namespace RPA_Web_Portal_Prototype_v2.Repositories;

public class AppDbContext : DbContext
{
    public DbSet<User> UserTable { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public AppDbContext(){}
    
    public AppDbContext(DbContextOptions<AppDbContext> opt):base(opt){}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectString = "Server=localhost;Database=Skaila;User=root;Password=1234";
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(connectString, ServerVersion.AutoDetect(connectString));
        }
    }
}