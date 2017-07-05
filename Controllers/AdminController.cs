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
            string binaryString;
            Console.WriteLine(value);

            if(value == 913) {
                if (model.Image.Length > 0)
                {
                    using (var fileStream = model.Image.OpenReadStream())
                    using (var ms = new MemoryStream())
                    {
                        fileStream.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        binaryString = Convert.ToBase64String(fileBytes);
                    }
                    var spot = await CreateSpotAsync(model.Name, model.CategoryIndex, model.Latitude, model.Longitude, model.Address, binaryString, model.Description);
                    return RedirectToAction(nameof(Spots));  
                } else {
                    return RedirectToAction(nameof(Login));
                }  
            } else {
                return RedirectToAction(nameof(Login));
            }
        }

        public async Task<Spot> CreateSpotAsync(string name, int categoryIndex, double latitude, double longitude, string address, string pictureBinary, string description) {
            
            var spot = new Spot {
                Name = name,
                CategoryIndex = categoryIndex,
                Latitude = latitude,
                Longitude = longitude, 
                Address = address,
                PictureBinary = pictureBinary,
                Description = description
            };

            _database.Spots.Add(spot);
            await _database.SaveChangesAsync();

            return spot;
        }
    }
}