using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Models;

namespace ORM.Services
{
    public class DBManager : DbContext
    {
        // DbSet<User> Users erzeugen
        public DbSet<User> Users { get; set; }

        // OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;port=3306;database=mauidb;user=root;password=root;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        // CRUD: CreateUserAsync
        public async Task<bool> CreateUserAsync(User user, bool save = false)
        {
            if (user is null)
            {
                return false;
            }

            this.Users.Add(user);
            
            if (save)
            {
                return await SaveToDbAsync();
            }
            
            return true;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await this.Users.FindAsync(userId);
        }
        
        public async Task<User?> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            return await Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public async Task<bool> SaveToDbAsync()
        {
            try
            {
                return await this.SaveChangesAsync() > 0;
            }
            catch (Exception e)
            {
                // TODO: Fehler loggen
                return false;
            }
        }
    }
}
