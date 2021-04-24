using MiBandNaramek.Areas.Identity.Data;
using MiBandNaramek.Configuration;
using MiBandNaramek.Data;
using MiBandNaramek.Models;
using MiBandNaramek.Models.API.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiBandNaramek.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasuredDataController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        private readonly UserManager<MiBandNaramekUser> _userManager;

        public MeasuredDataController(ApplicationDbContext applicationDbContext, UserManager<MiBandNaramekUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        
        [HttpPost]
        [Route("InsertMeasuredData")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> InsertMeasuredData([FromBody] JsonElement postData)
        {
            
             //                                   =========== LOG DO DB ===========
                                             
            await _applicationDbContext.RequestData.AddAsync(new RequestData() { Request = postData.ToString() });
            if (await _applicationDbContext.SaveChangesAsync() > 0)
            {

            }
            
            MeasuredDataRequest measuredDataList = JsonSerializer.Deserialize<MeasuredDataRequest>(postData.ToString());
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = await _userManager.FindByNameAsync(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                    long uploadDate = DateTimeOffset.Now.ToUnixTimeSeconds();

                    if (measuredDataList.MeasuredData != null && measuredDataList.MeasuredData.Count > 0)
                    {
                        measuredDataList.MeasuredData.ForEach(option => option.Date = DateTimeOffset.FromUnixTimeSeconds(option.Timestamp).DateTime);
                        measuredDataList.MeasuredData.ForEach(option => option.UserId = userId.Id);
                        measuredDataList.MeasuredData.ForEach(option => option.UploadDate = uploadDate);

                        measuredDataList.MeasuredData.ForEach(option => option.Id = _applicationDbContext.MeasuredData.FirstOrDefault(find => find.UserId == option.UserId && find.Timestamp == option.Timestamp) != null ? 1 : 0);
                        await _applicationDbContext.MeasuredData.AddRangeAsync(measuredDataList.MeasuredData.Where(option => option.Id == 0));
                    }

                    if (measuredDataList.BatteryData != null && measuredDataList.BatteryData.Count > 0)
                    {
                        measuredDataList.BatteryData.ForEach(option => option.Date = DateTimeOffset.FromUnixTimeSeconds(option.Timestamp).DateTime);
                        measuredDataList.BatteryData.ForEach(option => option.UserId = userId.Id);
                        measuredDataList.BatteryData.ForEach(option => option.UploadDate = uploadDate);

                        measuredDataList.BatteryData.ForEach(option => option.Id = _applicationDbContext.BatteryData.FirstOrDefault(find => find.UserId == option.UserId && find.Timestamp == option.Timestamp) != null ? 1 : 0);
                        await _applicationDbContext.BatteryData.AddRangeAsync(measuredDataList.BatteryData.Where(option => option.Id == 0));
                    }

                    if (measuredDataList.ActivityData != null && measuredDataList.ActivityData.Count > 0)
                    {
                        measuredDataList.ActivityData.ForEach(option => option.DateStart = DateTimeOffset.FromUnixTimeSeconds(option.TimestampStart).DateTime);
                        measuredDataList.ActivityData.ForEach(option => option.DateEnd = DateTimeOffset.FromUnixTimeSeconds(option.TimestampEnd).DateTime);
                        measuredDataList.ActivityData.ForEach(option => option.UserId = userId.Id);
                        measuredDataList.ActivityData.ForEach(option => option.UploadDate = uploadDate);

                        measuredDataList.ActivityData.ForEach(option => option.Id = _applicationDbContext.ActivityData.FirstOrDefault(find => find.UserId == option.UserId && find.TimestampStart == option.TimestampStart) != null ? 1 : 0);
                        await _applicationDbContext.ActivityData.AddRangeAsync(measuredDataList.ActivityData.Where(option => option.Id == 0));
                    }


                    // Vložíme tam všechny data z tokenu naráz
                    await _applicationDbContext.SaveChangesAsync();

                    return Ok();

                }
                catch
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }
        
    }
}
