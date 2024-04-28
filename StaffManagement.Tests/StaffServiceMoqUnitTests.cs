using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using StaffManagement;
using StaffManagement.Services;
using StaffManagement.Models;

namespace Tests;
public class StaffServiceMoqTests
{
	[Fact]
	public async Task GetStaff_RetursNotFoundIfNotExists()
	{
		// Arrange
		var mock = new Mock<IStaffService>();

		mock.Setup(m => m.Find(It.Is<int>(id => id == 1)))
		.ReturnsAsync((Staff?)null);

		// Act
		var result = await StaffEndpoints.GetStaff(1, mock.Object);

		// Assert
		Assert.IsType<Results<Ok<Staff>, NotFound>>(result);

		var notFoundResult = (NotFound)result.Result;

		Assert.NotNull(notFoundResult);
	}

	[Fact]
	public async Task GetAll_ReturnsStaffsFromDatabase()
	{
		// Arrange
		var mock = new Mock<IStaffService>();

		mock.Setup(m => m.GetAll(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
		.ReturnsAsync(new List<Staff> {
			new Staff
			{
				StaffId = "0001",
				Fullname = "John Doe",
				Gender = 1,
				Birthdate = new DateTime(1996, 5, 1)
			},
			new Staff
			{
				StaffId = "0002",
				Fullname = "Jane Doe",
				Gender = 2,
				Birthdate = new DateTime(1996, 3, 15)
			}
		});

		var staffService = mock.Object;

		// Act
		var result = await StaffEndpoints.GetAllStaffs(null, null, null, null, staffService);

		// Assert
		Assert.IsType<Ok<List<Staff>>>(result);

		Assert.NotNull(result.Value);
		Assert.NotEmpty(result.Value);
		Assert.Collection(result.Value,
		staff1 =>
		{
			Assert.Equal("0001", staff1.StaffId);
			Assert.Equal("John Doe", staff1.Fullname);
			Assert.Equal(1, staff1.Gender);
			Assert.Equal(new DateTime(1996, 5, 1), staff1.Birthdate);
		},
		staff2 =>
		{
			Assert.Equal("0002", staff2.StaffId);
			Assert.Equal("Jane Doe", staff2.Fullname);
			Assert.Equal(2, staff2.Gender);
			Assert.Equal(new DateTime(1996, 3, 15), staff2.Birthdate);
		});
	}

	[Fact]
	public async Task GetStaff_ReturnsStaffFromDatabase()
	{
		// Arrange
		var mock = new Mock<IStaffService>();

		mock.Setup(m => m.Find(It.Is<int>(id => id == 1)))
		.ReturnsAsync(new Staff
		{
			Id = 1,
			StaffId = "J007",
			Fullname = "Jame Bond",
			Gender = 1,
			Birthdate = new DateTime(1977, 1, 5)
		});


		// Act
		var result = await StaffEndpoints.GetStaff(1, mock.Object);

		// Assert
		Assert.IsType<Results<Ok<Staff>, NotFound>>(result);

        var okResult = (Ok<Staff>)result.Result;

		Assert.NotNull(okResult.Value);
		Assert.Equal(1, okResult.Value.Id);
	}

	[Fact]
	public async Task CreateStaff_CreatesStaffInDatabase()
	{
		// Arrange
		var staffs = new List<Staff>();

		var mock = new Mock<IStaffService>();

		var newStaff = new StaffDto { 
			StaffId = "0001",
			Fullname = "Oudam Jone",
			Gender = 1,
			Birthdate = new DateTime(1997, 10, 15)
		};

		mock.Setup(m => m.Add(It.Is<Staff>(t => t.StaffId == newStaff.StaffId && t.Fullname == newStaff.Fullname && t.Gender == newStaff.Gender && t.Birthdate == t.Birthdate)))
		.Callback<Staff>(staff => staffs.Add(staff))
		.Returns(Task.CompletedTask);

		// Act
		var result = await StaffEndpoints.CreateStaff(newStaff, mock.Object);

		// Assert
		Assert.IsType<Created<Staff>>(result);

		Assert.NotNull(result);

		Assert.NotEmpty(staffs);
	}

	[Fact]
	public async Task UpdateStaff_UpdatesStaffInDatabase()
	{
		// Arrange
		var oldStaff = new Staff { 
			Id = 1,
			StaffId = "008",
			Fullname = "Jame Henry",
			Gender = 1,
			Birthdate = new DateTime(1998, 3, 3)
		 };

		var updateStaff = new Staff
		{
			Id = 1,
			StaffId = "008",
			Fullname = "Jame William",
			Gender = 1,
			Birthdate = new DateTime(1997, 3, 3)
		};

		var mock = new Mock<IStaffService>();

		mock.Setup(m => m.Find(It.Is<int>(id => id == 1)))
		.ReturnsAsync(oldStaff);

		mock.Setup(m => m.Update(It.Is<Staff>(s => s.Id == updateStaff.Id && s.StaffId == updateStaff.StaffId && s.Fullname == updateStaff.Fullname && s.Gender == updateStaff.Gender && s.Birthdate == updateStaff.Birthdate)))
		.Callback<Staff>(staff => oldStaff = staff)
		.Returns(Task.CompletedTask);

		// Act
		var result = await StaffEndpoints.UpdateStaff(updateStaff, mock.Object);

		// Assert
		Assert.IsType<Results<Created<Staff>, NotFound>>(result);

		var createdResult = (Created<Staff>)result.Result;

		Assert.NotNull(createdResult);

		Assert.Equal("Jame William", oldStaff.Fullname);
		Assert.Equal(new DateTime(1997, 3, 3), oldStaff.Birthdate);
	}

	[Fact]
	public async Task DeleteStaff_DeletesStaffInDatabase()
	{
		// Arrange
		var oldStaff = new Staff
		{
			Id = 1,
			StaffId = "J007",
			Fullname = "Jame Bond",
			Gender = 1,
			Birthdate = new DateTime(1977, 3, 3)
		};

		var staffs = new List<Staff> { oldStaff };

		var mock = new Mock<IStaffService>();

		mock.Setup(m => m.Find(It.Is<int>(id => id == 1)))
		.ReturnsAsync(oldStaff);

		mock.Setup(m => m.Remove(It.Is<Staff>(s => s.Id == 1)))
		.Callback<Staff>(staff => staffs.Remove(staff))
		.Returns(Task.CompletedTask);

		// Act
		var result = await StaffEndpoints.DeleteStaff(oldStaff.Id, mock.Object);

		// Assert
		Assert.IsType<Results<NoContent, NotFound>>(result);

		var noContentResult = (NoContent)result.Result;

		Assert.NotNull(noContentResult);
		Assert.Empty(staffs);
	}

	[Fact]
	public async Task SearchStaff_ReturnsMatchingStaffs()
	{
		// Arrange
		var mock = new Mock<IStaffService>();

		// search query
		var staffId = "000";
		var gender = 1;
		var from = new DateTime(1990, 1, 1);
		var to = new DateTime(2000, 1, 1);

		var matchingStaff = new List<Staff>
		{
			new Staff {
				StaffId = "0001",
				Fullname = "Jame Bond",
				Gender = 1,
				Birthdate = new DateTime(1998, 3, 28)
			},
			new Staff {
				StaffId = "0002",
				Fullname = "John Cunning",
				Gender = 1,
				Birthdate = new DateTime(1999, 1, 31)
			}
		};

		mock.Setup(m => m.GetAll(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
		.ReturnsAsync(matchingStaff);

		// Act
		var result = await StaffEndpoints.GetAllStaffs(staffId, gender, from, to, mock.Object);

		// Assert
		Assert.IsType<Ok<List<Staff>>>(result);

		Assert.NotNull(result.Value);
		Assert.NotEmpty(result.Value);
		Assert.Collection(result.Value,
		   staff1 => 
		   {
			   Assert.Equal("0001", staff1.StaffId);
			   Assert.Equal("Jame Bond", staff1.Fullname);
			   Assert.Equal(1, staff1.Gender);
			   Assert.Equal(new DateTime(1998, 3, 28), staff1.Birthdate);
		   },
		   staff2 => 
		   {
			   Assert.Equal("0002", staff2.StaffId);
			   Assert.Equal("John Cunning", staff2.Fullname);
			   Assert.Equal(1, staff2.Gender);
			   Assert.Equal(new DateTime(1999, 1, 31), staff2.Birthdate);
		   }
		);
	}
}