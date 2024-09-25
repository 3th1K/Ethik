using NUnit.Framework;
using Ethik.Utility.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Ethik.Utility.Data.Extensions.Tests;

[TestFixture]
public class DbContextExtensionsTests
{
    // Sample entity class
    public class TestEntity
    {
        public string Id { get; set; } = null!;
    }

    private Mock<DbContext> _mockDbContext = null!;

    [SetUp]
    public void Setup()
    {
        // Setup the mock DbContext
        _mockDbContext = new Mock<DbContext>();
    }

    [Test]
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
        Assert.IsTrue(testEntity.Id.StartsWith("TST"));
    }
}