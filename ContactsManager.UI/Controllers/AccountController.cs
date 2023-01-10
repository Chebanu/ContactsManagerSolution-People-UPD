﻿using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using CRUD.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x=>x.ErrorMessage);
                return View(registerDTO);
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                UserName = registerDTO.Email,
                PersonName= registerDTO.PersonName
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                //Sign in
                await _signInManager.SignInAsync(user, false);

                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }
            else
            {
                foreach(IdentityError error in result.Errors) 
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                return View(registerDTO);
            }


        }
    }
}