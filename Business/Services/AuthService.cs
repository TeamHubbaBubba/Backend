using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class AuthService(IAuthRepository authRepository) : IAuthService
    {
        private readonly IAuthRepository _authRepository = authRepository;


        public async Task<ResponseResult> SignOutAsync()
        {
            var result = await _authRepository.SignOutAsync();

            return result 
                ? ResponseResult.Ok()
                : ResponseResult.Error("Something went wrong signing out.");
        }
    }
}
