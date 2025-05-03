using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveRequest.Models
{

    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class employerquest
    {

        public string Name { get; set; }

        public string Email { get; set; }

        public string Department { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; }

    }

    public class LeaveRequestStatus
    {
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
    }

    public class LeaveRequestUpdate
    {
        public string Status { get; set; }
    }
}