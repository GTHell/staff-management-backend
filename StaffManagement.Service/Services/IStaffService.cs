using StaffManagement.Models;

namespace StaffManagement.Services;

public interface IStaffService
{
	Task<List<Staff>> GetAll(string? staffId, int? gender, DateTime? from, DateTime? to);

	ValueTask<Staff?> Find(int id);

	Task Add(Staff staff);

	Task Update(Staff staff);

	Task Remove(Staff staff);
}