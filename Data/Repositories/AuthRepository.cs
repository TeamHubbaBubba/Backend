using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class AuthRepository(SignInManager<UserEntity> signInManager): IAuthRepository
    {
        private readonly SignInManager<UserEntity> _signInManager = signInManager;

        public async Task<bool> SignOutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error signing out: {ex.Message}");
                return false;
            }
        }
    }
}
