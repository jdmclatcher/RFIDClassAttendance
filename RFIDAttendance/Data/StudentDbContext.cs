using Microsoft.EntityFrameworkCore;
using RFIDAttendance.Models;

namespace RFIDAttendance.Data
{
	public class StudentDbContext : DbContext
	{
		public StudentDbContext(DbContextOptions<StudentDbContext> options)
			: base(options)
		{
		}

		public DbSet<Student> Student { get; set; }
	}
}
