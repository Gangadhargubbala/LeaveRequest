namespace LeaveRequest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        Department = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        LeaveId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        FromDate = c.String(),
                        ToDate = c.String(),
                        Reason = c.String(),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.LeaveId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Requests", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Requests", new[] { "EmployeeId" });
            DropTable("dbo.Requests");
            DropTable("dbo.Employees");
        }
    }
}
