using EmpMngtAPI.DataModel;
using EmpMngtAPI.Model.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmpMngtAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JobDetailsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetJobLocation")]
        public IActionResult GetJobLocation()
        {
            var jobLocations = _context.locationTbls
                .Select(l => new JobLocationDTO
                {
                    Id = l.Id,
                    Value = l.Value,
                    Description = l.Description ?? "NA"
                }).Distinct()
                .ToList();
            return Ok(jobLocations);
        }
    }
}
