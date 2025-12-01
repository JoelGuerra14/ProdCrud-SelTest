using Microsoft.EntityFrameworkCore;
using SGP.Application.Interfaces;
using SGP.Domain.Base;
using SGP.Domain.Entities;
using SGP.Persistence.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGP.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;

        public UserRepository(Context context)
        {
            _context = context;
        }

        public async Task<OperationResult> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user == null)
                return OperationResult.Failure("Usuario o contraseña incorrectos.");

            return OperationResult.Success("Login exitoso", user);
        }

        public async Task<OperationResult> RegisterAsync(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                return OperationResult.Failure("El usuario ya existe.");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return OperationResult.Success("Usuario registrado");
        }
    }
}
