using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SSUMAP.Models.Request
{
    public class SpotRequestModel
    {
        public IFormFile Image { get; set; }
        public string Name { get; set; }
        public int CategoryIndex { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
    }
}