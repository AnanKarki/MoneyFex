namespace FAXER.PORTAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableStaffTraining : DbMigration
    {
        public override void Up()
        {
            
            
            CreateTable(
                "dbo.StaffTraining",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StaffInformationId = c.Int(nullable: false),
                        Title = c.String(),
                        Link = c.String(),
                        Deadline = c.DateTime(nullable: false),
                        CompleteTraining = c.String(),
                        OutstandingTraining = c.String(),
                        isDeleted = c.Boolean(nullable: false),
                        TrainingAddedByStaff = c.Int(nullable: false),
                        TrainingAddedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StaffInformation", t => t.StaffInformationId, cascadeDelete: true)
                .Index(t => t.StaffInformationId);
            
            
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StaffTraining", "StaffInformationId", "dbo.StaffInformation");
            DropIndex("dbo.StaffTraining", new[] { "StaffInformationId" });
            DropTable("dbo.StaffTraining");
        }
    }
}
