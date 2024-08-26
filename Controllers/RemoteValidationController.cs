using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Controllers
{
    public class RemoteValidationController : Controller
    {
        // GET: RemoteValidationController
        public ActionResult Index()
        {
            return View();
        }

        // GET: RemoteValidationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RemoteValidationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RemoteValidationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RemoteValidationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RemoteValidationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RemoteValidationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RemoteValidationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public IActionResult ValidateCountry(string country)
        {
            bool result;

            if (country == "USA" || country == "UK" || country == "India")
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return Json(result);
        }
    }
}
