using ArdantOffical.Helpers.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace ArdantOffical.Controllers
{


    [Route("api/[controller]")]
    [ApiController]

    public class FileUploadsss : ControllerBase
    {
        public IWebHostEnvironment Environment;
        public FileUploadsss(IWebHostEnvironment exc)
        {
            this.Environment = exc;
        }
        public string rootFolder;
        [HttpPost]
        [DisableRequestSizeLimit,
    RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
        ValueLengthLimit = int.MaxValue)]
        public IActionResult UploadFiles(List<IFormFile> filesArray)
        {
            long size = 0;
            //   var files =new FormFileCollection();
            List<string> MyFileArray = new List<string>();
            //   var httpRequest = HttpContext.cu.Request;
            IFormFileCollection files = Request.Form.Files;

            string filename = "";
            int count = 0;
            foreach (var file in files)
            {
                filename = ContentDispositionHeaderValue
                               .Parse(file.ContentDisposition)
                               .FileName
                               .Trim('"');
                filename = filename.Replace("_", " ");
              
                var extension = FGCExtensions.GetFileExtension(filename);
                extension = extension.ToLower();
                if (!FGCExtensions.FileExtension().Contains(extension)) return BadRequest(new List<string>() { "Invalid extension" });
            }
            foreach (IFormFile file in files)
            {
                count += 1;
                filename = ContentDispositionHeaderValue
                               .Parse(file.ContentDisposition)
                               .FileName
                               .Trim('"');
                filename = filename.Replace("_", " ");
                var extension = FGCExtensions.GetFileExtension(filename);
                extension = extension.ToLower();
                if (!FGCExtensions.FileExtension().Contains(extension)) return BadRequest("Invalid extension");
                Regex reg = new Regex("[*'\",_&#^@%()+]");
                //str1 = reg.Replace(str1, string.Empty);
                filename = reg.Replace(filename, "_");



                //  MyFileName = filename;
                int startIndex = filename.LastIndexOf('.');
                string filenamewithoutextension = filename.Substring(0, startIndex);
                string FileExtension = filename.Substring(startIndex, 4);



                filename = filenamewithoutextension + "-" + DateTime.Now.ToString("ddmmyyyyhhmmss") + count + FileExtension;
                filename = filename.Replace(" ", "_");

                MyFileArray.Add(filename);

                filename = rootFolder = Path.Combine(Environment.ContentRootPath, "wwwroot/UploadImages" + $@"\{filename}");
                size += file.Length;
                using (FileStream fs =System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            //FileUploaderComponent obj = new FileUploaderComponent();
            //obj.RefreshAsync();


            //string message = MyFileName;
            return Ok(MyFileArray);
        }


        
        [Route("[controller]/[action]")]
        [HttpPost("UploadFilesOnboarding")]
        [DisableRequestSizeLimit,
   RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue,
       ValueLengthLimit = int.MaxValue)]
        public IActionResult UploadFilesOnboarding(List<IFormFile> filesArray)
        {
            long size = 0;
            //   var files =new FormFileCollection();
            List<string> MyFileArray = new List<string>();
            //   var httpRequest = HttpContext.cu.Request;
            IFormFileCollection files = Request.Form.Files;

            string filename = "";
            int count = 0;
            //foreach (var file in files)
            //{
            //    filename = ContentDispositionHeaderValue
            //                   .Parse(file.ContentDisposition)
            //                   .FileName
            //                   .Trim('"');
            //    filename = filename.Replace("_", " ");
            //    var extension = FGCExtensions.GetFileExtension(filename);
            //    extension = extension.ToLower();
            //    if (!FGCExtensions.FileExtension().Contains(extension)) return BadRequest(new List<string>() { "Invalid extension" });
            //}
            

            foreach (IFormFile file in files)
            {
                count += 1;
                filename = ContentDispositionHeaderValue
                               .Parse(file.ContentDisposition)
                               .FileName
                               .Trim('"');
                filename = filename.Replace("_", " ");
                var extension = FGCExtensions.GetFileExtension(filename);
                extension = extension.ToLower();
                if (!FGCExtensions.FileExtension().Contains(extension)) return BadRequest("Invalid extension");
                Regex reg = new Regex("[*'\",_&#^@%()+]");
                //str1 = reg.Replace(str1, string.Empty);
                filename = reg.Replace(filename, "_");



                //  MyFileName = filename;
                int startIndex = filename.LastIndexOf('.');
                string filenamewithoutextension = filename.Substring(0, startIndex);
                string FileExtension = filename.Substring(startIndex, 4);
                var fileTitle = filename;
                filename = filenamewithoutextension + "-" + DateTime.Now.ToString("ddmmyyyyhhmmss") + count + FileExtension;
                filename = filename.Replace(" ", "_");

                

                //filename = rootFolder = Path.Combine(Environment.ContentRootPath, "wwwroot/UploadedDocs" + $@"\{filename}");
                string rootFolder = Path.Combine(Environment.ContentRootPath, "wwwroot/Documents");
                // Create a directory with the current year if it doesn't exist
                int currentYear = DateTime.Now.Year;
                string currentYearDirectory = Path.Combine(rootFolder, $"{currentYear}_documents");
                var folder = Path.GetFileName(currentYearDirectory);
                MyFileArray.Add(filename);
                MyFileArray.Add(currentYearDirectory);
                MyFileArray.Add(fileTitle);
                MyFileArray.Add(folder);

                if (!Directory.Exists(currentYearDirectory))
                {
                    Directory.CreateDirectory(currentYearDirectory);
                }

                // Combine the directory path with the filename to get the full path
                string fullPath = Path.Combine(currentYearDirectory, filename);

                size += file.Length;
                using (FileStream fs = System.IO.File.Create(fullPath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            //FileUploaderComponent obj = new FileUploaderComponent();
            //obj.RefreshAsync();


            //string message = MyFileName;
            return Ok(MyFileArray);
        }


    }


}
