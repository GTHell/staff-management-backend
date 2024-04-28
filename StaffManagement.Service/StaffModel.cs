using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StaffManagement.Models
{
	public class StaffDto{
		public string StaffId { get; set; } = string.Empty;
		public string Fullname { get; set; } = string.Empty;
		public DateTime Birthdate { get; set; }
		public int Gender { get; set; } = 1;
	}

	// Model class
	public class Staff
	{
		public int Id { get; set; }
		[MaxLength(8)]
		public string StaffId { get; set; } = string.Empty;
		[MaxLength(100)]
		public string Fullname { get; set; } = string.Empty;
		public DateTime Birthdate { get; set; }
		public int Gender { get; set; }
	}

	// Context
	public class StaffDb : DbContext
	{
		public StaffDb(DbContextOptions options) : base(options) { }
		public DbSet<Staff> Staffs { get; set; } = null!;
	}
}