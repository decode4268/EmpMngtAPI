using EmpMngtAPI.DataModel;
using EmpMngtAPI.Helper;
using EmpMngtAPI.Model.RequestModel;
using EmpMngtAPI.Model.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.AppConfig;

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

        //[HttpPost("AddNewJob")]
        //public async Task<IActionResult> AddNewJob(AddNewJobPosition request)
        //{
        //    JobPositionTbl payload = new JobPositionTbl();

        //    payload.PositionName = request.JobDetails;
        //    payload.CTC = request.CTC;
        //    payload.LocationId = request.LocationId;
        //    payload.Status = JobPositionStatus.Pending;
        //    payload.IsActive = true;
        //    payload.CreateDate = DateTime.Now;
        //    payload.UpdateDate = DateTime.Now;

        //    await _context.jobPositionTbls.AddAsync(payload);
        //    _context.SaveChanges();



        //}

        [HttpPost("AddNewJob")]
        public async Task<IActionResult> AddNewJob([FromBody] AddNewJobPosition request)
        {
            if (request == null)
                return BadRequest("Request body cannot be null.");

            if (string.IsNullOrWhiteSpace(request.JobDetails))
                return BadRequest("Job details are required.");

            try
            {
                var payload = new JobPositionTbl
                {
                    PositionName = request.JobDetails,
                    CTC = request.CTC,
                    LocationId = request.LocationId,
                    Status = JobPositionStatus.Pending,
                    IsActive = true,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow
                };
                await _context.jobPositionTbls.AddAsync(payload);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    status = 200,
                    message = "Job created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    //message = "Something went wrong while creating the job"
                    message = ex.Message
                });
            }
        }

    }
}
