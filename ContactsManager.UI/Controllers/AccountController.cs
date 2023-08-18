﻿using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using CRUD.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.Enums;

namespace ContactsManager.UI.Controllers;

[Route("[controller]/[action]")]
//[AllowAnonymous]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AccountController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    [Authorize("NotAuthorized")]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [Authorize("NotAuthorized")]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDTO registerDTO)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
            return View(registerDTO);
        }

        ApplicationUser user = new ApplicationUser()
        {
            Email = registerDTO.Email,
            PhoneNumber = registerDTO.Phone,
            UserName = registerDTO.Email,
            PersonName = registerDTO.PersonName
        };

        IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);
        if (result.Succeeded)
        {
            //Check status
            if (registerDTO.UserType == UserTypeOptions.Admin)
            {
                //Create admin role
                if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                {
                    var applicationRole = new ApplicationRole()
                    {
                        Name = UserTypeOptions.Admin.ToString()
                    };
                    await _roleManager.CreateAsync(applicationRole);
                }
                await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
            }
            else
            {
                //Create user role
                if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                {
                    var applicationRole = new ApplicationRole()
                    {
                        Name = UserTypeOptions.User.ToString()
                    };
                    await _roleManager.CreateAsync(applicationRole);
                }

                await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
            }
            //Sign in
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }
        else
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("Register", error.Description);
            }
            return View(registerDTO);
        }
    }

    [HttpGet]
    [Authorize("NotAuthorized")]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [Authorize("NotAuthorized")]
    public async Task<IActionResult> Login(LoginDTO loginDTO, string? ReturnUrl)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
            return View(loginDTO);
        }

        var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: true, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if(user != null)
            {
                if(await _userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
            }

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return LocalRedirect(ReturnUrl);
            }
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        ModelState.AddModelError("Login", "Invalid email or password");

        return View(loginDTO);
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(PersonsController.Index), "Persons");
    }

    [AllowAnonymous]
    public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
    {
        ApplicationUser user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Json(true); //valid
        }
        else
        {
            return Json(false); //invalid
        }

    }
}
