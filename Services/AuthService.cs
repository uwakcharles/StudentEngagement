using Microsoft.EntityFrameworkCore;
using StudentEngagement.Data;
using StudentEngagement.Models;
using StudentEngagement.Services.Interfaces;

namespace StudentEngagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user is Student student)
            {
                await _context.Entry(student).Reference(s => s.PersonalSupervisor).LoadAsync();
            }

            // If you still need other user types, you must also query them separately.

            if (user == null || !user.Authenticate(password))
                return null;

            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !user.Authenticate(currentPassword))
                return false;

            user.SetPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}