using PharmaDistiPro.DTO.Categorys;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.Services.Interface
{

    public interface ICategoryService
    {
        Task<Response<CategoryDTO>> CreateCategoryAsync(CategoryInputRequest categoryInputRequest);
        Task<Response<IEnumerable<CategoryDTO>>> FilterCategoriesAsync(string? searchTerm);
        Task<Response<IEnumerable<CategoryDTO>>> GetAllSubCategoriesAsync();
        Task<Response<IEnumerable<CategoryDTO>>> GetCategoryTreeAsync();
       
        Task<Response<CategoryDTO>> UpdateCategoryAsync(int id, CategoryInputRequest categoryUpdateRequest);
    }


}
