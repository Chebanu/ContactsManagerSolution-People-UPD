﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Controllers;

public class HomeController : Controller
{
    [AllowAnonymous]
    [Route("Error")]
    public IActionResult Error()
    {
        IExceptionHandlerFeature? exceptionHandler = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if(exceptionHandler != null && exceptionHandler.Error!= null)
        {
            ViewBag.ErrorMessage = exceptionHandler.Error.Message;
        }
        
        return View();
    }
}
