using System.Globalization;
using Microsoft.EntityFrameworkCore;
using StaffManagement.Models;

namespace StaffManagement.Services;

public class StaffService: IStaffService
{
	private readonly StaffDb _db;

	public StaffService(StaffDb db)
	{
		_db = db;
	}

	public async Task<List<Staff>> GetAll(string? staffId, int? gender, DateTime? from, DateTime? to)
	{
		// Create IQueryable<Staff> for doing the query term
		var query = _db.Staffs.AsQueryable();

		// Query term
		if (!string.IsNullOrEmpty(staffId)) query = query.Where(s => s.StaffId.Contains(staffId));
		if (gender is >= 1 and <= 2 ) query = query.Where(s => s.Gender.Equals(gender));

        // Set default date to fall between 1955 to current date
		if (!from.HasValue) from = new DateTime(1955, 1, 1);
		if (!to.HasValue) to = DateTime.Today;

		query = query.Where(s => s.Birthdate >= from && s.Birthdate <= to);

        // Load data
		return await query.ToListAsync();
	}

	public async ValueTask<Staff?> Find(int id)
	{
		return await _db.Staffs.FindAsync(id);
	}

	public async Task Add(Staff staff)
	{
		await _db.Staffs.AddAsync(staff);
		await _db.SaveChangesAsync();
	}
 
	public async Task Update(Staff staff)
	{
		_db.Staffs.Update(staff);
		await _db.SaveChangesAsync();
	}

	public async Task Remove(Staff staff) 
	{
		_db.Staffs.Remove(staff);
		await _db.SaveChangesAsync();
	}
}