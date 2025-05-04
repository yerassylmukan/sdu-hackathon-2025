using WebApplication1.Models;

namespace WebApplication1.Common.Interfaces;

public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryModel>>> GetCategoriesAsync();
    Task<Result<CategoryModel>> GetCategoryAsync(Guid id);
    Task<Result<CategoryModel>> CreateCategoryAsync(string categoryName);
    Task<Result<CategoryModel>> UpdateCategoryNameAsync(Guid id, string categoryName);
    Task<Result> DeleteCategoryAsync(Guid id);
}