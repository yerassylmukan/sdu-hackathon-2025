using Microsoft.EntityFrameworkCore;
using WebApplication1.Common;
using WebApplication1.Common.Interfaces;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<CategoryModel>>> GetCategoriesAsync()
    {
        var categories = await _context.Categories.Include(c => c.Foods).ToListAsync();
        var categoryModels = categories.Select(c => MapToCategoryModel(c));
        return Result<IEnumerable<CategoryModel>>.Success(categoryModels);
    }

    public async Task<Result<CategoryModel>> GetCategoryAsync(Guid id)
    {
        var category = await _context.Categories.Include(c => c.Foods).FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return Result<CategoryModel>.Failure("Category not found");

        var categoryModel = MapToCategoryModel(category);
        return Result<CategoryModel>.Success(categoryModel);
    }

    public async Task<Result<CategoryModel>> CreateCategoryAsync(string categoryName)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = categoryName
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var categoryModel = MapToCategoryModel(category);
        return Result<CategoryModel>.Success(categoryModel);
    }

    public async Task<Result<CategoryModel>> UpdateCategoryNameAsync(Guid id, string categoryName)
    {
        var existing = await _context.Categories.FindAsync(id);
        if (existing == null) return Result<CategoryModel>.Failure("Category not found");

        existing.Name = categoryName;
        await _context.SaveChangesAsync();

        var categoryModel = MapToCategoryModel(existing);
        return Result<CategoryModel>.Success(categoryModel);
    }

    public async Task<Result> DeleteCategoryAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return Result.Failure("Category not found");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    private CategoryModel MapToCategoryModel(Category category)
    {
        return new CategoryModel
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}