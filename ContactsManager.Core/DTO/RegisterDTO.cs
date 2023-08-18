﻿using Microsoft.AspNetCore.Mvc;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DTO;

public class RegisterDTO
{
    [Required(ErrorMessage = "Name cannot be blank")]
    public string PersonName { get; set; }

    [Required(ErrorMessage = "Email cannot be blank")]
    [EmailAddress(ErrorMessage = "Email should be in a proper email address")]
    [Remote(action: "IsEmailAlreadyRegistered", controller: "Account", ErrorMessage = "Email is already in use")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone cannot be blank")]
    [RegularExpression("[0-9]*$", ErrorMessage = "Phone number should contain numbers only")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Password cannot be blank")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Password cannot be blank")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password doesnot match")]
    public string ConfirmPassword { get; set; }

    public UserTypeOptions UserType { get; set; } = UserTypeOptions.User;
}
