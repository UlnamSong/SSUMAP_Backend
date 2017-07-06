using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using SSUMAP.Models.Request;
using SSUMAP.Models.Response;
using SSUMAP.Models.Data;
using SSUMAP.Models.Dul;
using SSUMAP.Services;

namespace SSUMAP.Controllers {
    public class AdminController : Controller {
        const string SessionId = "_Id";
        const string SessionPassword = "_Password";
        private IHostingEnvironment _environment;

        private readonly DatabaseContext _database;

        public AdminController(DatabaseContext context, IHostingEnvironment hostenv)
        {
            this._database = context;
            this._environment = hostenv;
        }

        [HttpGet]
        public IActionResult Login() {
            return View();
        }
        public IActionResult Spots() {
            return View();
        }
        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public IActionResult Login(AdminLoginRequestModel model) {
            if(model.Id == "admin" && model.Password == "85477125") {
                HttpContext.Session.SetInt32(SessionId, 913);
                return RedirectToAction(nameof(Spots));
            } else {
                return Content($"Auth Failed.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpot(SpotRequestModel model, ICollection<IFormFile> files) {
            Int32 value = HttpContext.Session.GetInt32(SessionId) ?? 0;
            
            Console.WriteLine(value);

            string fileName = string.Empty;
            int fileSize = 0;
            var uploadFolder = Path.Combine(_environment.WebRootPath, "files");

            if(value == 913) {
                foreach (var file in files) {
                    if (file.Length > 0) {
                        fileSize = Convert.ToInt32(file.Length);
                        fileName = FileUtility.GetFileNameWithNumbering(uploadFolder, Path.GetFileName(
                            ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"')));

                        using(var fileStream = new FileStream(
                            Path.Combine(uploadFolder, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }
                }

                // Spot spot = new Spot();

                // spot.Name = model.Name;
                // spot.Address = model.Address;
                // spot.Longitude = model.Longitude;
                // spot.Latitude = model.Latitude;
                // spot.Description = model.Description;
                // spot.CategoryIndex = model.CategoryIndex;
                // spot.FileName = fileName;

                // 비동기로 Spot Object 업로드하여 추가하기
                await CreateSpotAsync(model.Name, model.CategoryIndex, model.Latitude, model.Longitude, model.Address, fileName, model.Description);                    
                return RedirectToAction(nameof(Spots));
            } else {
                return RedirectToAction(nameof(Login));
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