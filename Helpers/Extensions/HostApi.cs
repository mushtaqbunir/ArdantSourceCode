using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ArdantOffical.Data;
using ArdantOffical.Helpers.Extensions;
using ArdantOffical.IService;
using ArdantOffical.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
 
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ArdantOffical.Areas.Identity;
using ArdantOffical.Areas.Identity.Pages;
using Microsoft.AspNetCore.Components;

namespace ArdantOffical.Helpers.Extensions
{
    public static class HostApi
    {
        public static NavigationManager nv;
        public static string Checkhostapi()
        {
           // return nv.BaseUri.TrimEnd('/');
             return "https://localhost:44366";

        }
        public static string LiveUrl = "https://www.fgcerp.com";
        public static string LocalUrl = "https://localhost:44366";
        public static string DivUrl = "https://www.fgcerpdev.com";
    }
}
    
