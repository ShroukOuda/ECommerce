using AutoMapper;
using ECommerce.Application.DTO.Category;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities.Category;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace ECommerce.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IValidator<AddCategoryDTO>> _addValidatorMock;
    private readonly Mock<IValidator<UpdateCategoryDTO>> _updateValidatorMock;
    private readonly ICategoryService _categoryService;

    public CategoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>(); 
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _addValidatorMock = new Mock<IValidator<AddCategoryDTO>>();
        _updateValidatorMock = new Mock<IValidator<UpdateCategoryDTO>>();
        _categoryService = new CategoryService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _fileStorageServiceMock.Object,
            _addValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnCategories()
    {
        // Arrange
        var categories = new List<Category> { new() { Id = TestGuid.FromInt(1), Name = "Electronics" } };

        _unitOfWorkMock.Setup(u => u.CategoryRepository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _categoryService.GetAllCategoriesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WhenNotFound_ShouldReturnNull()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(TestGuid.FromInt(999), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(TestGuid.FromInt(999));

        // Assert
        result.Should().BeNull();
    }
}
