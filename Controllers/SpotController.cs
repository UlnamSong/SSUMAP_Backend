using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SSUMAP.Models.Response;
using SSUMAP.Models.Request;
using SSUMAP.Models.Data;
using SSUMAP.Services;

namespace SSUMAP.Controllers
{
    [Route("api/spots")]
    public class SpotController : Controller
    {
        private readonly DatabaseContext _database;
        public SpotController(DatabaseContext context)
        {
            this._database = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetSpots(int categoryIndex = -1, int page = 0, int take = 30) {
            if(categoryIndex < 0) {
                var spots = await GetSpotsAsync(page, take);
                return Ok(spots.Select(spot => new SpotResponseModel(spot)));
            } else {
                var spots = await GetSpotsAsyncCond(categoryIndex, page, take);
                return Ok(spots.Select(spot => new SpotResponseModel(spot)));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpot(int SpotId) {
            var spot = await GetSpotAsync(SpotId);
            if(spot == null ) {
                return NotFound(new ErrorResponseModel("존재하지 않는 장소입니다."));
            }

            return Ok(new SpotResponseModel(spot));
        }



        public async Task<Spot> GetSpotAsync (int SpotId) {
            var spot = await _database.Spots.FindAsync(SpotId);
            return spot;
        }

        public async Task<Spot[]> GetSpotsAsync(int page = 0, int take = 30) {
            return await _database.Spots.Skip(page * take).Take(take).ToArrayAsync();
        }

        public async Task<Spot[]> GetSpotsAsyncCond(int categoryIndex, int page, int take) {
            return await _database.Spots.FromSql("SELECT * FROM dbo.Spots WHERE CategoryIndex=" + categoryIndex).Skip(page * take).Take(take).ToArrayAsync();
        }
    }
}