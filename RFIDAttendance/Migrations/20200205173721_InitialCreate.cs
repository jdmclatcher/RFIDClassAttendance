using Microsoft.EntityFrameworkCore.Migrations;

namespace RFIDAttendance.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    InClass = table.Column<bool>(nullable: false),
                    TimeLastCheckedIn = table.Column<string>(nullable: true),
                    TimeLastCheckedOut = table.Column<string>(nullable: true),
                    AttendaceStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Student");
        }
    }
}
