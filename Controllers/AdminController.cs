using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using SSUMAP.Models.Request;
using SSUMAP.Models.Response;
using SSUMAP.Models.Data;
using SSUMAP.Services;

namespace SSUMAP.Controllers {
    public class AdminController : Controller {
        const string SessionId = "_Id";
        const string SessionPassword = "_Password";

        private readonly DatabaseContext _database;
        private readonly IHostingEnvironment _hostingEnvironment;

        public AdminController(DatabaseContext context, IHostingEnvironment hostenv)
        {
            this._database = context;
            this._hostingEnvironment = hostenv;
        }

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        public IActionResult Spots() {
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

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> CreateSpot(SpotRequestModel model) {
            Int32 value = HttpContext.Session.GetInt32(SessionId) ?? 0;

            Console.WriteLine(value);
            string webRootPath = _hostingEnvironment.WebRootPath;
            string contentRootPath = _hostingEnvironment.ContentRootPath;

            if(value == 913) {
                var spot = await CreateSpotAsync(model.Name, model.CategoryIndex, model.Latitude, model.Longitude, model.Address, model.Image, model.Description);
                return RedirectToAction(nameof(Spots));
            } else {
                return RedirectToAction(nameof(Login));
            }
        }

        public async Task<Spot> CreateSpotAsync(string name, int categoryIndex, double latitude, double longitude, string address, IFormFile picture, string description) {
            var filePath = _hostingEnvironment.WebRootPath + "/upload/" + name;

            if (!Directory.Exists(filePath)) {
                Directory.CreateDirectory(filePath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await picture.CopyToAsync(stream);
            }

            var spot = new Spot {
                Name = name,
                CategoryIndex = categoryIndex,
                Latitude = latitude,
                Longitude = longitude, 
                Address = address,
                PictureUrl = filePath,
                Description = description
            };

            _database.Spots.Add(spot);
            await _database.SaveChangesAsync();

            return spot;
        }
    }
}