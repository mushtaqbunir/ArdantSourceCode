using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace ArdantOffical.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnboardingUploads : Controller
    {
        public IWebHostEnvironment Environment;
        public OnboardingUploads(IWebHostEnvironment exc)
        {
            this.Environment = exc;
        }
        public string rootFolder;
        //[HttpPost]

        //public IActionResult UploadFiles(OtherData Objdata)
        //{
        //    //   var files =new FormFileCollection();
        //    List<string> MyFileArray = new List<string>();
        //    List<string> DisplayFileNames = new List<string>();
        //    //   var httpRequest = HttpContext.cu.Request;
        //    IFormFileCollection files = Request.Form.Files;

        //    foreach (IFormFile formFile in files)
        //    {
        //        //string uref = Reference.GetUniqueReference("EMR");
        //        string FileName = DateTime.Now.ToString("dd-MM-yyy-hh-mm-ss") + "-" + formFile.FileName;
        //        DisplayFileNames.Add(formFile.FileName);
        //        FileName = FileName.Replace(" ", String.Empty);
        //        MyFileArray.Add(FileName);
        //        //string fulPath = Path.Combine(Environment.ContentRootPath, "wwwroot\\UploadedDocs", FileName);
        //        string fulPath = Path.Combine("D:\\Inetpub\\fgconboarding.com\\httpdocs\\", "wwwroot\\UploadedDocs", FileName);  //for onboarding docs.
        //        using (FileStream fs = System.IO.File.Create(fulPath))
        //        {
        //            formFile.CopyTo(fs);
        //            fs.Flush();
        //        }
        //    }

        //    //string message = MyFileName;
        //    return Ok(new { MyFileArray = MyFileArray, DisplayFileNames = DisplayFileNames });
        //}
    }

}
