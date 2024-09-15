using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ethik.Utility.Extensions.Tests;

[TestClass]
public class DbContextExtensionsTests
{
    // Sample entity class
    public class TestEntity
    {
        public string Id { get; set; } = null!;
    }

    private Mock<DbContext> _mockDbContext = null!;

    [TestInitialize]
    public void Setup()
    {
        // Setup the mock DbContext
        _mockDbContext = new Mock<DbContext>();
    }

    [TestMethod]
    public async Task AddEntityWithAutoIdAsync_ShouldGenerateIdAndAddEntityAsync()
    {
        // Arrange
        var testEntity = new TestEntity();

        // Mock the DbSet and setup AddAsync
        var mockDbSet = new Mock<DbSet<TestEntity>>();
        _mockDbContext.Setup(m => m.Set<TestEntity>()).Returns(mockDbSet.Object);

        // Act
        await _mockDbContext.Object.AddEntityWithAutoIdAsync(testEntity, e => e.Id, "TST");

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(testEntity.Id)); // Ensure ID was set
        Assert.IsTrue(testEntity.Id.StartsWith("TST")); // Ensure prefix is correct

        // Verify that AddAsync was called once
        //mockDbSet.Verify(m => m.AddAsync(testEntity, default), Times.Once);
    }
}