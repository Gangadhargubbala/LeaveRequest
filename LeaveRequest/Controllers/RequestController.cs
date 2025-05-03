using LeaveRequest.Models;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Configuration;
using System.Web.Security;

namespace LeaveRequest.Controllers
{
    [Authorize]
    public class RequestController : ApiController
    {
        private readonly EmployeesDBContext _context = new EmployeesDBContext();

        
        [AllowAnonymous]
        [HttpPost]
        [Route("token")] //Generate Token
        public IHttpActionResult GenerateToken(Login Cred)
        {
            if ((Cred.Username == "adminuser" || Cred.Username == "EmployeeUser") && Cred.Password == "Password")
            {
                var token = JWTTokenManager.GenerateToken(Cred.Username);
                return Ok(new { token });
            }
            return Unauthorized();
        }

        
        [Authorize(Roles = "Employee,Admin")]
        [Route("gettoken")] //Check Whether Token is valid or not
        public IHttpActionResult ValidtokenOrnot()
        {
            
            return Ok("AUTHENTACTED");
        }


        [Authorize(Roles = "Employee")]
        [HttpPost]
        [Route("ApplyLeave")]
        public IHttpActionResult getemployedata(employerquest dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (DateTime.TryParse(dto.FromDate, out DateTime fromDate))
                {
                    if (fromDate.Date < DateTime.Today)
                    {
                        return BadRequest("FromDate cannot be in the past.");
                    }
                }
                else
                {
                    return BadRequest("FromDate is not a valid date.");
                }

                if (string.IsNullOrWhiteSpace(dto.Reason))
                {
                    return BadRequest("Reason should not be empty.");
                }

                var employee = new Employees
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Department = dto.Department
                };

                _context.Employees.Add(employee);
                _context.SaveChanges(); // this assigns EmployeeId automatically


                bool employeeExists = _context.Employees.Any(e => e.EmployeeId == employee.EmployeeId);
                if (!employeeExists)
                {
                    return BadRequest("Employee ID does not exist.");
                }

                // Step 2: Insert into Request table with the new EmployeeId
                var request = new Request
                {
                    EmployeeId = employee.EmployeeId,
                    FromDate = dto.FromDate,
                    ToDate = dto.ToDate,
                    Reason = dto.Reason,
                    Status = dto.Status
                };

                _context.Requests.Add(request);
                _context.SaveChanges();

                return Ok(new
                {
                    employeeId = employee.EmployeeId,
                    employeeName = employee.Name,
                    leaveId = request.LeaveId,
                    requestStatus = request.Status
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("leaverequest")]
        public async Task<IHttpActionResult> GetleaveRequest()
        {
            try
            {
                var leaverequest = await _context.Requests
                    .Include(r => r.Employee)
                    .Select(r => new LeaveRequestStatus
                    {
                        LeaveId = r.LeaveId,
                        EmployeeId = r.EmployeeId,
                        EmployeeName = r.Employee.Name,
                        FromDate = r.FromDate,
                        ToDate = r.ToDate,
                        Reason = r.Reason,
                        Status = r.Status
                    })
                    .ToListAsync();

                return Ok(leaverequest);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("leaveStauts/{id}")]
        public IHttpActionResult updateLeaveRequest(int id, LeaveRequestStatus lrs)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var leaverequest = _context.Requests.SingleOrDefault(r => r.LeaveId == id);
                if (leaverequest == null)
                {
                    return BadRequest($"No record found with request {id} ");
                }

                if (lrs.Status != "Approved" && lrs.Status != "Rejected")
                {
                    return BadRequest("Status must be either 'Approved' or 'Rejected'.");
                }

                leaverequest.Status = lrs.Status;
                _context.SaveChanges();

                return Ok(new { Message = $"Leave request {id} has been {lrs.Status.ToLower()}." });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [Authorize(Roles = "Employee")]
        [HttpDelete]
        [Route("DeleteLeave/{id}")]
        public IHttpActionResult deleateRequest(int id)
        {
            try
            {
                var leaverequest = _context.Requests.SingleOrDefault(r => r.LeaveId == id);
                if (leaverequest == null)
                {
                    return NotFound();
                }

                _context.Requests.Remove(leaverequest);
                _context.SaveChanges();
                return (Ok(new { Message = $"Leave Request {id} has been deleted." }));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }
}
