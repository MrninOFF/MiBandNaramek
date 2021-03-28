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
        public async Task<IActionResult> InsertMeasuredData([FromBody] MeasuredDataRequest measuredDataList)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = await _userManager.FindByNameAsync(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                    foreach (MeasuredDataFormat measuredData in measuredDataList.MeasuredData)
                    {
                        MeasuredData newMeasuredData = new MeasuredData()
                        {
                            UserId = userId.Id,
                            HeartRate = measuredData.HeartRate,
                            Steps = measuredData.Steps,
                            Intensity = measuredData.Intensity,
                            Kind = measuredData.Kind,
                            Date = DateTimeOffset.FromUnixTimeSeconds(measuredData.Timestamp).DateTime,
                            UploadDate = DateTimeOffset.Now.ToUnixTimeSeconds()
                        };
                        await _applicationDbContext.MeasuredData.AddAsync(newMeasuredData);
                    }

                    // Vložíme tam všechny data z tokenu naráz
                    await _applicationDbContext.SaveChangesAsync();

                    return Ok(new MeasuredDataResult()
                    {
                        Success = true
                    });

                }
                catch
                {
                    return Ok(new MeasuredDataResult()
                    {
                        Success = false,
                        Message = "Error during a save process"
                    });
                }
            }
            else
            {
                return Ok(new MeasuredDataResult()
                {
                    Success = false,
                    Message = "Model is Invalid"
                });
            }
        }
        
    }
}
