﻿using ContactsManager.Core.Domain.IdentityEntities;
using CRUD.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace CRUD;

static public class ConfigureServiceExtension 
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration) 
    {
        services.AddTransient<ResponseHeaderActionFilter>();


        services.AddControllersWithViews(op =>
        {
            var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

            op.Filters.Add(new ResponseHeaderActionFilter(logger) { Key = "My-Key-From-Global", Value = "My-Value-From-Global", Order = 2 });
        });

        //add services into IoC container

        services.AddScoped<ICountriesRepository, CountriesRepository>();
        services.AddScoped<IPersonsRepository, PersonsRepository>();

        services.AddScoped<ICountriesAdderService, CountriesAdderService>();
        services.AddScoped<ICountriesGetterService, CountriesGetterService>();
        services.AddScoped<ICountriesUploaderService, CountriesUploadService>();

        services.AddScoped<IPersonGetterService, PersonGetterService>();
        services.AddScoped<IPersonAdderService, PersonAdderService>();
        services.AddScoped<IPersonSorterService, PersonSorterService>();
        services.AddScoped<IPersonUpdaterService, PersonUpdaterService>();
        services.AddScoped<IPersonDeleterService, PersonDeleterService>();
        services.AddTransient<PersonsListActionFilter>();

        services.AddDbContext<ApplicationDbContext>(option =>
        {
            option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        

        //Enable Identuty in this project
        services.AddIdentity<ApplicationUser, ApplicationRole>(op =>
        { 
            //Requirements for password
            op.Password.RequiredLength = 8;
            op.Password.RequireNonAlphanumeric = false;
            op.Password.RequireUppercase = false;
            op.Password.RequireLowercase = true;
            op.Password.RequireDigit= false;
            op.Password.RequiredUniqueChars = 3;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
            .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

        services.AddAuthorization(otp =>
        {
            otp.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            //enforces authoriation policy (user must be authenticated for all the action methods)
            otp.AddPolicy("NotAuthorized", policy =>
            {
                policy.RequireAssertion(context =>
                {
                    return !context.User.Identity.IsAuthenticated;
                });
            });
        });

        services.ConfigureApplicationCookie(op =>
        {
            op.LoginPath = "/Account/Login";
        });

        services.AddHttpLogging(op => { op.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders; });

        return services;
    }
}
