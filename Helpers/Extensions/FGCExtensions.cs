using ArdantOffical.Data;
using ArdantOffical.Data.ModelVm;
using ArdantOffical.Helpers.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ArdantOffical.Helpers.Extensions
{
    public static class FGCExtensions
    {
        public static string DomaiName ;
        private static Dictionary<string, string> contentTypes;
        //[Inject]
        //public static  AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        /// <summary>
        /// file Stream format Convert To Base64
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Base64 string</returns>
        public static string ConvertToBase64(this Stream stream)
        {
            byte[] bytes = new Byte[(int)stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, (int)stream.Length);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// file path convert base64 string
        /// </summary>
        /// <param name="file path"></param>
        /// <returns> baase64 string </returns>
        public static string ConvertToBase64Stringpath(this string path)
        {

            string result = Path.GetFileName(path);
            using (FileStream fileStream = File.OpenRead(path))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);



                string fff = fileStream.ConvertToBase64();

                string[] tt = result.Split('.');

                int ii = tt.Length - 1;
                dynamic last = (dynamic)null;
                string rawformate = "raw,cr2,nef,orf,sr2";
                string Images = "jpg,tif,gif,jpeg,png,bmp,eps";

                if (tt[ii].Contains(Images))
                {
                    last = "data:image/" + tt[ii] + ";base64 ," + fff;
                }
                else if (tt[ii].Contains(rawformate))
                {
                    last = "data:image/" + tt[ii] + ";base64 ," + fff;
                }

                else if (tt[ii].ToLower() == "html")
                {
                    last = "data:text/" + tt[ii] + ";base64 ," + fff;
                }
                else if (tt[ii].ToLower() == "pdf")
                {
                    last = "data:application/" + tt[ii] + ";base64 ," + fff;
                }

                return last;
            };




        }
        public static DateTime UsDateTime(string dateVal)
        {
           
                DateTime parsed;
                bool valid = DateTime.TryParseExact(dateVal, "dd-MM-YYYY",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out parsed);
                if(valid)
                {
                    string[] DateArray = dateVal.Split('-');
                    string UsFormat = DateArray[1] + "/" + DateArray[0] + "/" + DateArray[2];
                    return Convert.ToDateTime(UsFormat);
                } else
                {
                    valid = DateTime.TryParseExact(dateVal, "dd/MM/YYYY",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out parsed);
                    if(valid)
                    {
                        string[] DateArray = dateVal.Split('/');
                        string UsFormat = DateArray[1] + "/" + DateArray[0] + "/" + DateArray[2];
                        return Convert.ToDateTime(UsFormat);
                    } else
                    {
                        valid = DateTime.TryParseExact(dateVal, "MM/dd/YYYY",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out parsed);
                        if(valid)
                        {
                            return Convert.ToDateTime(dateVal);
                        }
                    }
                }

                return Convert.ToDateTime(dateVal);

        }

        public static string UKDateTime(string dateVal)
        {
            string[] DateArray = dateVal.Split('/');
            string UKFormat = DateArray[1] + "/" + DateArray[0] + "/" + DateArray[2];
            return UKFormat;
        }
        /// <summary>
        /// convert Datetime to UK TimeZone
        /// </summary>
        /// <param name="DateTime"></param>
        /// <returns> UK DateTime </returns>
        public static DateTime DateTime_UK(this DateTime serverDate)
        {
            TimeZoneInfo britishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            DateTime UKDateTime = TimeZoneInfo.ConvertTime(serverDate, TimeZoneInfo.Local, britishZone);
            return UKDateTime;
        }

        public static DateTime DatetimeRemoveTime(this DateTime date)
        {
            DateTime NewDateTime = Convert.ToDateTime(date.ToString("MM/dd/yyyy")); ;
            return NewDateTime;

        }
        /// <summary>
        /// date time format 10 Nov, 2021 22:35:55
        /// </summary>
        /// <param name="date"></param>
        /// <returns>  Format 10 Nov, 2021 22:35:55 </returns>
        public static DateTime FGCFormatDatetime(this DateTime date)
        {
            DateTime NewDateTime = Convert.ToDateTime(date.ToString("dd MMM, yyyy HH:mm:ss")); ;
            return NewDateTime;

        }

        public static DateTime ToDateTime(this string strDate)
        {
            return DateTime.Parse(strDate);
        }

        public static async Task<ClaimsPrincipal> UserRole(this AuthenticationStateProvider AuthenticationStateProvider)
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            ClaimsPrincipal user = authState.User;

            //Userr = authState;
            return user;
        }


        public static string CheckFileExtension(this string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string result = Path.GetFileName(path);
                string[] Splitpath = result.Split('.');
                int pathLength = Splitpath.Length - 1;
                dynamic last = (dynamic)null;
                string Images = "jpg,jpeg,png";
                if (Images.Contains(Splitpath[pathLength]))
                {
                    last = "fa fa-file-photo-o fa-2x";
                }

                else if (Splitpath[pathLength].ToLower() == "pdf")
                {
                    last = "fa fa-file-pdf-o fa-2x";
                }
                else if (Splitpath[pathLength].ToLower() == "doc")
                {
                    last = "fa fa-file-word-o fa-2x";
                }
                else if (Splitpath[pathLength].ToLower() == "csv")
                {
                    last = "fa fa-file-word-o fa-2x";
                }
                else if (Splitpath[pathLength].ToLower() == "xls")
                {
                    last = "fa fa-file-excel-o fa-2x";
                }
                else if (Splitpath[pathLength].ToLower() == "xlsx")
                {
                    last = "fa fa-file-excel-o fa-2x";
                }
                else if (Splitpath[pathLength].ToLower() == "zip")
                {
                    last = "fa fa-file-zip-o fa-2x";
                }
                else
                {
                    last = "";
                }
                return last;
            }
            else
                return "";
        }
        public static DateTime ConverTostringDate(this string TransactionTime)
        {
            DateTime formatedDate = DateTime.Parse(TransactionTime);
            return formatedDate;
        }
        public static DateTime ConvertStringToDate(this string TransactionTime)
        {

            DateTime newdate = Convert.ToDateTime(TransactionTime);
            TransactionTime = newdate.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime formatedDate = DateTime.ParseExact(TransactionTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            return formatedDate;
        }
        public static string GetFileExtension(this string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string result = Path.GetFileName(path);
                string[] Splitpath = result.Split('.');
                int pathLength = Splitpath.Length - 1;

                return Splitpath[pathLength];
            }
            else
                return "";

        }
        public static double GetdoubleType(this int count)
        {
            double Count = count;
            return Count;
        }
        public static DateTime DateConvertString(this string ServerDate)
        {
            if (ServerDate != null && ServerDate != "")
            {
                DateTime yy = Convert.ToDateTime(ServerDate);
                return Convert.ToDateTime(ServerDate);

            }
            else
            {
                return DateTime.Now.DateTime_UK();
            }

        }


        public static DateTime DateConvertToShortString(this string ServerDate)
        {
            if (ServerDate != null && ServerDate != "")
            {
                //DateTime datetiem = Convert.ToDateTime(ServerDate);
                //var newdate= datetiem.ToShortDateString();
                return Convert.ToDateTime(ServerDate);

            }
            else
            {
                return DateTime.Now.DateTime_UK();
            }

        }




        /// <summary>
        /// Render string as HTML
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static MarkupString ShowMarkupString(string content)
        {
            MarkupString ms = (MarkupString)content;
            return ms;
        }

        public static string FileExtension()
        {

            return ".png,.jpg ,.jpeg ,.jfif,.pjpeg ,.pjp,.bmp,.pdf,.zip,.Doc,.docx,.xls,.xlsx";
        }
        public static string FilesFormatsAllowedShowFoUser()
        {

            return "png, jpg , jpeg , jfif, pjpeg , pjp,.bmp, pdf, zip,.Doc, docx, xls, xlsx";
        }
        public static string[] FileExtensionShow()
        {

            string[] CheckExtension = { "zip", "doc", "csv", "xls", ".xlsx", "docx", "docs" };

            return CheckExtension;
        }
        public static string GetCurrencySymbol(this string Currency)
        {
            string GetSymbol = Currency;

            //var GBP = "GBP";
            //var Eur = "EUR";

            //if (!string.IsNullOrEmpty(Currency))
            //{
            //    if (Currency.ToUpper().Contains(GBP))
            //    {
            //        GetSymbol = "£";
            //    }
            //    else if (Currency.ToUpper().Contains(Eur))
            //    {
            //        GetSymbol = "€";
            //    }

            //}

            return GetSymbol;

        }
        public static string GetSymbolToCurrency(this string Currency)
        {
            string GetSymbol = "";

            var GBP = "£";
            var Eur = "€";

            if (!string.IsNullOrEmpty(Currency))
            {
                if (Currency.ToUpper().Contains(GBP))
                {
                    GetSymbol = "GBP";
                }
                else if (Currency.ToUpper().Contains(Eur))
                {
                    GetSymbol = "EUR";
                }

            }

            return GetSymbol;

        }

        //public static List<TResult> ToList<TResult>(this IQueryable source)
        //{
        //    return new List<TResult>(source);
        //}
        public static List<TResult> ToListTahir<TResult>(this IQueryable<TResult> source)
        {
            return new List<TResult>(source);
        }

        //public static IEnumerable<TblPrc> FullTextSearch(string text, FGCDbContext context)
        //{
        //    return (LinqUtilities.GenericFullTextSearch<TblPrc>(text, context) as IEnumerable<TblPrc>);
        //}

        public static string GetClientRiskScore(this decimal? clientRiskScore)//css call
        {
            var RiskIndicatorCssclass = "";
            if (clientRiskScore != null)
            {
                switch (clientRiskScore)
                {
                    case < 3:
                        RiskIndicatorCssclass = "Low";
                        break;
                    case < 5:
                        RiskIndicatorCssclass = "Low-Medium";
                        break;
                    case < 6:
                        RiskIndicatorCssclass = "Medium";
                        break;
                    case < 7:
                        RiskIndicatorCssclass = "Medium-Higher";
                        break;
                    case < 9:
                        RiskIndicatorCssclass = "High";
                        break;
                    case < 10:
                        RiskIndicatorCssclass = "Ultra-High";
                        break;
                    case 10:
                        RiskIndicatorCssclass = "Unacceptable";
                        break;
                    default:
                        RiskIndicatorCssclass = "";
                        break;
                }
            }


            return RiskIndicatorCssclass;
        }


        public static TostModel AlertSuccessMessage(this string SuccessMessage)
        {
            if (string.IsNullOrEmpty(SuccessMessage))
            {
                SuccessMessage = "Record added Successfully...";
            }
            else
            {
                if (SuccessMessage == "2")
                    SuccessMessage = "Record update Successfully...";
                else if (SuccessMessage == "3")
                {
                    SuccessMessage = "Record delete Successfully...";
                }
                else if (!string.IsNullOrEmpty(SuccessMessage))
                {
                    // Success message already have value provided from parent component.
                    SuccessMessage = "Record added Successfully...";
                }
                else
                    SuccessMessage = "Record added Successfully...";
            }
            TostModel TostModelclass = new TostModel();
            TostModelclass.AlertMessageShow = true;
            TostModelclass.AlertMessagebody = SuccessMessage;
            TostModelclass.Msgstyle = MessageColor.Success;
            return TostModelclass;
        }
        public static TostModel AlertErrorMessage(this string ErrorMessage)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = "Record not added due to some Error...";
            }
            TostModel TostModelclass = new TostModel();
            TostModelclass.AlertMessageShow = true;
            TostModelclass.AlertMessagebody = ErrorMessage;
            TostModelclass.Msgstyle = MessageColor.Error;
            return TostModelclass;
        }

        //public static List<ChangeTrackerProperties> ChangeTrackerDatabase<T>(this FGCDbContext _context) where T : class
        //{
        //    _context.ChangeTracker.DetectChanges();
        //    List<ChangeTrackerProperties> ListOfChangeProperties = new();
        //    foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> entry in _context.ChangeTracker.Entries<T>().Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified))
        //    {
        //        try
        //        {

        //            foreach (var entityProperty in entry.Entity.GetType().GetProperties())
        //            {
        //                string propertyName = entityProperty.Name;
        //                if (!propertyName.Contains("RmCategory"))
        //                {
        //                    string currentValue = entry.Property(propertyName).CurrentValue?.ToString();
        //                    string originalValue = entry.Property(propertyName).OriginalValue?.ToString();
        //                    if (currentValue != originalValue)
        //                    {
        //                        if (string.IsNullOrEmpty(currentValue))
        //                        {
        //                            currentValue = "-";
        //                        }

        //                        if (string.IsNullOrEmpty(originalValue))
        //                        {
        //                            originalValue = "-";
        //                        }

        //                        ChangeTrackerProperties fillingData = new ChangeTrackerProperties
        //                        {
        //                            NewValue = currentValue,
        //                            OldValue = originalValue,
        //                            PropertyName = propertyName
        //                        };
        //                        ListOfChangeProperties.Add(fillingData);

        //                    }
        //                }

        //            }
        //        }
        //        catch (Exception)
        //        {


        //        }


        //    }
        //    return ListOfChangeProperties;
        //}

        public static string GetIpAddress()
        {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] ipAddressWithText = response.Split(':');
            string ipAddressWithHTMLEnd = ipAddressWithText[1].Substring(1);
            string[] ipAddress = ipAddressWithHTMLEnd.Split('<');
            string mainIP = ipAddress[0];

            return mainIP;
        }
        public static string GetClientIPAddress(this HttpContext context)
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.Headers["X-Forwarded-For"]))
            {
                ip = context.Request.Headers["X-Forwarded-For"];
            }
            else
            {
                ip = context.Request.HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString();
            }
            return ip;
        }
        public static string GetClientIPAddress2(HttpContext context)
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.Headers["X-Forwarded-For"]))
            {
                ip = context.Request.Headers["X-Forwarded-For"];

            }
            else
            {
                ip = context.Request.HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString();
            }
            return ip;
        }


        public static bool DeleteFileRootFolder(this IWebHostEnvironment Environment, List<string> listofPath)
        {
            try
            {
                string rootFolder = Path.Combine(Environment.ContentRootPath, "wwwroot/UploadImages");
                foreach (string item in listofPath)
                {
                    if (System.IO.File.Exists(Path.Combine(rootFolder, item)))
                    {
                        // If file found, delete it    
                        System.IO.File.Delete(Path.Combine(rootFolder, item));
                        //  File.Delete(Path.Combine(rootFolder, split[2]));
                    }
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

        public static IEnumerable<TSource> Custom_DistinctBy<TSource, TKey>
         (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static string GetEnumDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()?
                            .GetMember(enumValue.ToString())?
                            .First()?
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name;
        }
    }

    public static class Reference
    {
        public static string GetUniqueReference(string predecessor)
        {
            StringBuilder builder = new();
            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(15)
                .ToList().ForEach(e => builder.Append(e));
            string id = $"{predecessor}-{builder}";
            return id;
        }
    }

   
}
