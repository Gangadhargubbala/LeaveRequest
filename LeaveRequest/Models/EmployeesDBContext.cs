using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace LeaveRequest.Models
{
    public class EmployeesDBContext : DbContext
    {
        public EmployeesDBContext() : base("Connection")
        {
        }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Request> Requests { get; set; }
    }
    
    
}