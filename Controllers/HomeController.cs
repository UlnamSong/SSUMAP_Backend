using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using SSUMAP.Models.Request;
using SSUMAP.Models.Response;
using SSUMAP.Controllers;
using SSUMAP.Models.Data;
using SSUMAP.Models.Dul;
using SSUMAP.Services;

namespace SSUMAP.Controllers {
    public class HomeController : Controller {
        const string SessionId = "_Id";
        const string SessionPassword = "_Password";
        private IHostingEnvironment _environment;

        private readonly DatabaseContext _database;

        public HomeController(DatabaseContext context, IHostingEnvironment hostenv)
        {
            this._database = context;
            this._environment = hostenv;
        }

        [HttpGet]
        public IActionResult Index() {
            return View();
        }
        public IActionResult Spots() {
            return View();
        }

        [HttpGet]
        public IActionResult Create() {
            Int32 value = HttpContext.Session.GetInt32(SessionId) ?? 0;
            if(value == 913) {
                return View();
            } else {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult Index(AdminLoginRequestModel model) {
            if(model.Id == "admin" && model.Password == "85477125") {
                HttpContext.Session.SetInt32(SessionId, 913);
                return RedirectToAction(nameof(Create));
            } else {
                return Content($"Auth Failed.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(SpotRequestModel model) {
            Int32 value = HttpContext.Session.GetInt32(SessionId) ?? 0;
            
            Console.WriteLine(value);

            string fileName = string.Empty;
            int fileSize = 0;
            var uploadFolder = Path.Combine(_environment.WebRootPath, "files");

            if(value == 913) {
                if (!ModelState.IsValid)
                    return View();

                if (model.FileName.Length > 0) {
                    fileSize = Convert.ToInt32(model.FileName.Length);
                    fileName = FileUtility.GetFileNameWithNumbering(uploadFolder, Path.GetFileName(
                        ContentDispositionHeaderValue.Parse(model.FileName.ContentDisposition).FileName.Trim('"')));

                    using(var fileStream = new FileStream(
                        Path.Combine(uploadFolder, fileName), FileMode.Create))
                    {
                        await model.FileName.CopyToAsync(fileStream);
                    }
                }

                // 비동기로 Spot Object 업로드하여 추가하기
                await CreateSpotAsync(System.Net.WebUtility.UrlEncode(model.Name), model.CategoryIndex, model.Latitude, model.Longitude, System.Net.WebUtility.UrlEncode(model.Address), System.Net.WebUtility.UrlEncode(fileName), System.Net.WebUtility.UrlEncode(model.Description));                    
                return RedirectToAction(nameof(Create));
            } else {
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<Spot> CreateSpotAsync(string name, int categoryIndex, double latitude, double longitude, string address, string fileName, string description) {
            
            var spot = new Spot {
                Name = name,
                CategoryIndex = categoryIndex,
                Latitude = latitude,
                Longitude = longitude, 
                Address = address,
                FileName = fileName,
                Description = description
            };

            _database.Spots.Add(spot);
            await _database.SaveChangesAsync();

            return spot;
        }
    }
}