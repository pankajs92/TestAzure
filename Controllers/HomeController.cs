using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InsuranceClient.Models;
using InsuranceClient.Models.ViewModel;
using System.IO;
using Microsoft.Extensions.Configuration;
using InsuranceClient.helpers;

namespace InsuranceClient.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration Configuraion;

        public HomeController(IConfiguration configuration)
        {
            this.Configuraion = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                //save date to azure table
                var custId = Guid.NewGuid();
                StorageHelpers SH = new StorageHelpers();
                SH.ConnectionString = Configuraion.GetConnectionString("StorageConnection");
                var tempFile = Path.GetTempFileName();
                using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                {
                    await model.image.CopyToAsync(fs);
                }
                var filename = Path.GetFileName(model.image.FileName);
                var tempPath = Path.GetDirectoryName(tempFile);
                var imagePath = Path.Combine(tempPath, string.Concat(custId, "_", filename));
                System.IO.File.Move(tempFile, imagePath); //rename temp file
               var imageUrl = await SH.UploadCustomerImageAsynch("images", imagePath);
                //save customer date to table
                Customer cust = new Customer(custId.ToString(),model.insuranceType);
                cust.Name = model.Name;
                cust.Email = model.Email;
                cust.Amount = model.Amount;
                cust.appDate = model.appDate;
                cust.endDate = model.endDate;
                cust.premium = model.premium;
                cust.imageUrl = imageUrl;
                await SH.InsertCustAsync("Customers", cust);

                //add confirmation message to azure queue
                await SH.AddMessageAsync("insurance-request", cust);
                return RedirectToAction("Index");

            }
            else
            {
                return View();
            }
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
