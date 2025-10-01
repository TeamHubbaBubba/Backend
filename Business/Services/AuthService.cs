
﻿using Business.Dtos;
using Business.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Business.Models;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services;

public class AuthService(SignInManager<UserEntity> signInManager) : IAuthService
{
    private readonly SignInManager<UserEntity> _signInManager = signInManager;


    public async Task<bool> SignInAsync(UserSignInDto form)
    {
        var userSignIn = await _signInManager.PasswordSignInAsync(form.Email, form.Password, false, false);
        return userSignIn.Succeeded;
    }

    public async Task<ResponseResult> SignOutAsync()
    {
        try
            {
                await _signInManager.SignOutAsync();
                return ResponseResult.Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error signing out: {ex.Message}");
                return ResponseResult.Error("Something went wrong signing out.");
            }
    }
}