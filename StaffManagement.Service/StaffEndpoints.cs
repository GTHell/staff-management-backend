using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using StaffManagement.Models;
using StaffManagement.Services;

namespace StaffManagement;

public static class StaffEndpoints
{
	public static RouteGroupBuilder MapStaffApi(this RouteGroupBuilder group)
	{
		group.MapGet("/", GetAllStaffs);
		group.MapGet("/{id}", GetStaff);
		group.MapPost("/", CreateStaff);
		group.MapPut("/{id}", UpdateStaff);
		group.MapDelete("/{id}", DeleteStaff);

		return group;
	}

	// get all staffs
	public static async Task<Ok<List<Staff>>> GetAllStaffs(string? staffId, int? gender, DateTime? from, DateTime? to, IStaffService staffService)
	{
		// set default value
		// start = DateTime.Parse("01/01/2000");

		var staffs = await staffService.GetAll(staffId, gender, from, to);
		return TypedResults.Ok(staffs);
	}

	// get staff
	public static async Task<Results<Ok<Staff>, NotFound>> GetStaff(int id, IStaffService staffService)
	{
		var staff = await staffService.Find(id);

		if (staff is null) return TypedResults.NotFound();

		return TypedResults.Ok(staff);
	}

	// create staff
	public static async Task<Created<Staff>> CreateStaff(StaffDto staff, IStaffService staffService)
	{
		var newStaff = new Staff
		{
			StaffId = staff.StaffId,
			Fullname = staff.Fullname,
			Birthdate = staff.Birthdate,
			Gender = staff.Gender
		};

		await staffService.Add(newStaff);

		return TypedResults.Created($"/staffs/{newStaff.Id}", newStaff);
	}

	// update staff
	public static async Task<Results<Created<Staff>, NotFound>> UpdateStaff(Staff staff, IStaffService staffService)
	{
		var currStaff = await staffService.Find(staff.Id);

		if (currStaff is null) return TypedResults.NotFound();

		currStaff.StaffId = staff.StaffId;
		currStaff.Fullname = staff.Fullname;
		currStaff.Birthdate = staff.Birthdate;
		currStaff.Gender = staff.Gender;

		await staffService.Update(currStaff);


		return TypedResults.Created($"/staffs/{currStaff.Id}", currStaff);
	}

	// delete staff
	public static async Task<Results<NoContent, NotFound>> DeleteStaff(int id, IStaffService staffService)
	{
		var staff = await staffService.Find(id);

		if (staff is null) return TypedResults.NotFound();

		await staffService.Remove(staff);
		return TypedResults.NoContent();
	}
}