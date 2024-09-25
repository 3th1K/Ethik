using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Ethik.Utility.Data.Repository.Tests;

[TestFixture]
public class BaseRepositoryTests
{
    private Mock<IDbContextFactory<TestDbContext>> _dbContextFactoryMock;
    private Mock<TestDbContext> _dbContextMock;
    private Mock<DbSet<TestEntity>> _dbSetMock;
    private Mock<ILogger<BaseRepository<TestEntity, TestDbContext>>> _loggerMock;
    private BaseRepository<TestEntity, TestDbContext> _repository;

    [SetUp]
    public void SetUp()
    {
        _dbContextFactoryMock = new Mock<IDbContextFactory<TestDbContext>>();
        _dbContextMock = new Mock<TestDbContext>();
        _dbSetMock = new Mock<DbSet<TestEntity>>();
        _loggerMock = new Mock<ILogger<BaseRepository<TestEntity, TestDbContext>>>();

        // Setup DbContext to return a DbSet when requested
        _dbContextMock.Setup(db => db.Set<TestEntity>()).Returns(_dbSetMock.Object);

        // Setup DbContextFactory to return the mocked DbContext
        _dbContextFactoryMock.Setup(factory => factory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_dbContextMock.Object);

        _repository = new TestRepository(_dbContextFactoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task AddAsync_ValidEntity_ReturnsSuccess()
    {
        // Arrange
        var entity = new TestEntity();

        // Mock AddAsync to return a real EntityEntry for the entity
        var entityEntry = new Mock<EntityEntry<TestEntity>>();
        entityEntry.Setup(e => e.Entity).Returns(entity);

        // Set up AddAsync to return the entity wrapped in a ValueTask<EntityEntry<TestEntity>>
        _dbSetMock.Setup(m => m.AddAsync(entity, It.IsAny<CancellationToken>()))
                  .ReturnsAsync((TestEntity e, CancellationToken ct) =>
                  {
                      // Create an actual EntityEntry manually (or mock the behavior realistically)
                      return _dbContextMock.Object.Entry(entity);
                  });

        // Act
        var result = await _repository.AddAsync(entity);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task AddAsync_ExceptionThrown_ReturnsFailure()
    {
        // Arrange
        var entity = new TestEntity();
        _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _repository.AddAsync(entity);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.That(result.ErrorStack[0].ErrorCode, Is.EqualTo(RepositoryErrorCodes.AddEntityFailure));
    }

    [Test]
    public async Task DeleteAsync_EntityFound_ReturnsSuccess()
    {
        // Arrange
        var entity = new TestEntity { Id = "123" };
        _dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _repository.DeleteAsync("123");

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_EntityNotFound_ReturnsFailure()
    {
        // Arrange
        _dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestEntity)null);

        // Act
        var result = await _repository.DeleteAsync("123");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.That(result.ErrorStack[0].ErrorCode, Is.EqualTo(RepositoryErrorCodes.EntityNotFound));
    }

    [Test]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Arrange
        _dbSetMock.Setup(m => m.CountAsync(It.IsAny<Expression<Func<TestEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        var result = await _repository.CountAsync(e => e.Id == "123");

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.That(result.Data, Is.EqualTo(5));
    }

    [Test]
    public async Task ExistsAsync_EntityExists_ReturnsTrue()
    {
        // Arrange
        _dbSetMock.Setup(m => m.AnyAsync(It.IsAny<Expression<Func<TestEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _repository.ExistsAsync(e => e.Id == "123");

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.Data);
    }

    [Test]
    public async Task FindAsync_EntitiesFound_ReturnsEntities()
    {
        // Arrange
        var entities = new List<TestEntity> { new TestEntity { Id = "123" } }.AsQueryable();
        _dbSetMock.As<IQueryable<TestEntity>>().Setup(m => m.Provider).Returns(entities.Provider);
        _dbSetMock.As<IQueryable<TestEntity>>().Setup(m => m.Expression).Returns(entities.Expression);
        _dbSetMock.As<IQueryable<TestEntity>>().Setup(m => m.ElementType).Returns(entities.ElementType);
        _dbSetMock.As<IQueryable<TestEntity>>().Setup(m => m.GetEnumerator()).Returns(entities.GetEnumerator());

        // Act
        var result = await _repository.FindAsync(e => e.Id == "123");

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.That(result!.Data!.Count(), Is.EqualTo(1));
    }
}

// Mocked test entity
public class TestEntity : IBaseEntity
{
    public TestEntity()
    {
    }

    public string Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastModified { get; set; }
    public bool? IsDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}

// Mocked DbContext
public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
}

// Concrete class for testing abstract BaseRepository
public class TestRepository : BaseRepository<TestEntity, TestDbContext>
{
    public TestRepository(IDbContextFactory<TestDbContext> contextFactory, ILogger logger) : base(contextFactory, logger)
    {
    }
}