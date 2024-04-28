using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using StaffManagement;
using StaffManagement.Models;
using Tests.IntegrationTests.Helpers;

namespace Tests.IntegrationTests;

[Collection("integration test")]
public class StaffServiceIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
	private readonly CustomWebApplicationFactory<Program> _factory;
	private readonly HttpClient _client;

	public StaffServiceIntegrationTests(CustomWebApplicationFactory<Program> factory)
	{
		_factory = factory;
		_client = factory.CreateClient();
	}

	[Fact]
	[Trait("Category", "Integration")]
	public async Task AddStaff_ReturnsCreatedResponse()
	{
		// Arrage
		var staffData = new Staff
		{
			StaffId = "S007",
			Fullname = "Jame Bond",
			Birthdate = new DateTime(1990, 1, 1),
			Gender = 1
		};

		// Act
		var response = await _client.PostAsJsonAsync("/api/staffs", staffData);

		// Assert
		response.EnsureSuccessStatusCode();
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Verify the id of the created staff record is 1
		var createdStaff = await response.Content.ReadFromJsonAsync<Staff>();
		Assert.Equal(3, createdStaff.Id);
		Assert.Equal(staffData.StaffId, createdStaff.StaffId);
		Assert.Equal(staffData.Fullname, createdStaff.Fullname);
		Assert.Equal(staffData.Birthdate, createdStaff.Birthdate);
		Assert.Equal(staffData.Gender, createdStaff.Gender);

		Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
	}

	[Fact]
	[Trait("Category", "Integration")]
	public async Task EditStaff_ShouldUpdateStaffRecord()
	{
		// Arrange
		var updatedStaffData = new 
		{
			Id = 1,
			StaffId = "S007",
			Fullname = "Jame Bond",
			Birthdate = new DateTime(1990, 1, 1),
			Gender = 1
		};

		// Act
		var response = await _client.PutAsJsonAsync($"/api/staffs/1", updatedStaffData);

		// Assert
		response.EnsureSuccessStatusCode();
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

		// Verify that the staff record has been updated
		var updatedStaff = await _client.GetFromJsonAsync<Staff>($"/api/staffs/1");

		Assert.Equal(updatedStaffData.Id, updatedStaff.Id);
		Assert.Equal(updatedStaffData.StaffId, updatedStaff.StaffId);
		Assert.Equal(updatedStaffData.Fullname, updatedStaff.Fullname);
		Assert.Equal(updatedStaffData.Birthdate, updatedStaff.Birthdate);
		Assert.Equal(updatedStaffData.Gender, updatedStaff.Gender);
	}

	[Fact]
	[Trait("Category", "Integration")]
	public async Task DeleteStaff_ShouldRemoveStaffRecord()
	{
		// Arrange

		// Act
		var response = await _client.DeleteAsync($"/api/staffs/1");

		// Assert
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

		// Verify that the staff record has been deleted
		var deletedStaffResponse = await _client.GetAsync($"/api/staffs/1");
		Assert.Equal(HttpStatusCode.NotFound, deletedStaffResponse.StatusCode);
	}
}