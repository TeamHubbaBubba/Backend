using Business.Dtos;
﻿using Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IAuthService
    {
        Task<bool> SignInAsync(UserSignInDto form);

        Task<ResponseResult> SignOutAsync();
    }
}