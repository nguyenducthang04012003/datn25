using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using PharmaDistiPro.DTO.Categorys;
using PharmaDistiPro.DTO.StorageRooms;
using PharmaDistiPro.DTO.Users;
using PharmaDistiPro.Helper;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Interface;
using PharmaDistiPro.Services.Interface;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

namespace PharmaDistiPro.Services.Impl
{

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryService(ICategoryRepository categoryRepository, Cloudinary cloudinary, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _httpContextAccessor = httpContextAccessor;
        }

        //category tree list
        public async Task<Response<IEnumerable<CategoryDTO>>> GetCategoryTreeAsync()
        {
            var response = new Response<IEnumerable<CategoryDTO>>();

            try
            {
                var categories = await _categoryRepository.GetAllAsync();

                if (!categories.Any())
                {
                    response.Success = false;
                    response.Message = "Không có dữ liệu danh mục";
                    return response;
                }

         
                var categoryDTOs = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

                // build tree
                var categoryTree = BuildCategoryTree(categoryDTOs.ToList());

                response.Success = true;
                response.Data = categoryTree;
                response.Message = "Lấy danh sách danh mục thành công";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi: {ex.Message}";
            }

            return response;
        }

        //build tree class
        private List<CategoryDTO> BuildCategoryTree(List<CategoryDTO> categories)
        {
            var categoryDict = categories.ToDictionary(c => c.Id, c => c);
            var rootCategories = new List<CategoryDTO>();

            foreach (var category in categories)
            {
                if (category.CategoryMainId == null)
                {
                    rootCategories.Add(category);
                }
                else
                {
                    if (categoryDict.TryGetValue(category.CategoryMainId.Value, out var parent))
                    {
                        parent.SubCategories.Add(category);
                    }
                }
            }

            return rootCategories;
        }

        public async Task<Response<IEnumerable<CategoryDTO>>> GetAllSubCategoriesAsync()
        {
            var response = new Response<IEnumerable<CategoryDTO>>();

            try
            {
                // Lấy tất cả danh mục từ repository
                var categories = await _categoryRepository.GetAllAsync();

                if (!categories.Any())
                {
                    response.Success = false;
                    response.Message = "Không có dữ liệu danh mục.";
                    return response;
                }

                // Lọc các danh mục có CategoryMainId không null
                var subCategories = categories.Where(c => c.CategoryMainId != null);

                if (!subCategories.Any())
                {
                    response.Success = false;
                    response.Message = "Không có danh mục con nào.";
                    return response;
                }

                // Chuyển sang DTO
                var subCategoryDTOs = _mapper.Map<IEnumerable<CategoryDTO>>(subCategories);

                response.Success = true;
                response.Message = "Lấy danh sách tất cả danh mục con thành công.";
                response.Data = subCategoryDTOs;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi: {ex.Message}";
            }

            return response;
        }

        // Filter by name
        public async Task<Response<IEnumerable<CategoryDTO>>> FilterCategoriesAsync(string? searchTerm)
        {
            var response = new Response<IEnumerable<CategoryDTO>>();

            try
            {
                // Create condition filter
                Expression<Func<Category, bool>> filter = c => true; 

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    filter = c => c.CategoryName.ToLower().Contains(searchTerm) || (c.CategoryCode != null && c.CategoryCode.ToLower().Contains(searchTerm));
                }


                
                var categories = await _categoryRepository.GetByConditionAsync(filter);

                if (!categories.Any())
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy danh mục phù hợp";
                    return response;
                }

                var categoryDTOs = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

                response.Success = true;
                response.Data = categoryDTOs;
                response.Message = "Lọc danh mục thành công";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi: {ex.Message}";
            }

            return response;
        }

        // Create Category
        public async Task<Response<CategoryDTO>> CreateCategoryAsync(CategoryInputRequest categoryInputRequest)
        {
            var response = new Response<CategoryDTO>();
            string imageUrl = null;

            try
            {
                // Kiểm tra danh mục có tồn tại không
                var existingCategory = await _categoryRepository.GetSingleByConditionAsync(
                    c => c.CategoryName.Equals(categoryInputRequest.CategoryName)
                );

                if (existingCategory != null)
                {
                    response.Success = false;
                    response.Message = "Tên danh mục đã tồn tại.";
                    return response;
                }

                string categoryCode;

                if (categoryInputRequest.CategoryMainId.HasValue)
                {
                    var parentCategory = await _categoryRepository.GetByIdAsync(categoryInputRequest.CategoryMainId.Value);
                    if (parentCategory == null)
                    {
                        response.Success = false;
                        response.Message = "Danh mục cha không tồn tại.";
                        return response;
                    }

                    categoryCode = $"{parentCategory.CategoryCode}_{GenerateCategoryCode(categoryInputRequest.CategoryName)}";
                }
                else
                {
                    categoryCode = GenerateCategoryCode(categoryInputRequest.CategoryName);
                }

                // Kiểm tra xem mã danh mục đã tồn tại chưa
                var isCodeExist = await _categoryRepository.GetSingleByConditionAsync(c => c.CategoryCode == categoryCode);
                if (isCodeExist != null)
                {
                    response.Success = false;
                    response.Message = $"Mã danh mục '{categoryCode}' đã tồn tại.";
                    return response;
                }

                if (categoryInputRequest.Image != null)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(categoryInputRequest.Image.FileName, categoryInputRequest.Image.OpenReadStream()),
                        PublicId = Path.GetFileNameWithoutExtension(categoryInputRequest.Image.FileName)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    imageUrl = uploadResult.SecureUri.ToString();
                }

                // Tạo danh mục mới
                var newCategory = _mapper.Map<Category>(categoryInputRequest);
                newCategory.CreatedDate = DateTime.Now;
                newCategory.Image = imageUrl;
                newCategory.CategoryCode = categoryCode;
                //newCategory.CreatedBy = UserHelper.GetUserIdLogin(_httpContextAccessor.HttpContext);

                await _categoryRepository.InsertAsync(newCategory);
                await _categoryRepository.SaveAsync();

                response.Success = true;
                response.Data = _mapper.Map<CategoryDTO>(newCategory);
                response.Message = "Tạo danh mục thành công";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi: {ex.Message} - {ex.InnerException?.Message}";
            }

            return response;
        }

        private string GenerateCategoryCode(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return "UNKNOWN";

            // Chuẩn hóa chuỗi: Xóa khoảng trắng dư thừa, loại bỏ dấu tiếng Việt
            string normalizedString = RemoveDiacritics(categoryName).Trim();

            // Tách các từ và lấy chữ cái đầu của mỗi từ viết hoa
            var words = normalizedString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Nếu chỉ có 1 từ, lấy 2-3 ký tự đầu thay vì chỉ lấy chữ cái đầu
            if (words.Length == 1)
                return words[0].Substring(0, Math.Min(3, words[0].Length)).ToUpper();

            return string.Join("", words.Select(w => w.Substring(0, 1).ToUpper()));
        }

        // Hàm loại bỏ dấu tiếng Việt
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
   


        public async Task<Response<CategoryDTO>> UpdateCategoryAsync(int id, CategoryInputRequest categoryUpdateRequest)
        {
            var response = new Response<CategoryDTO>();
            string imageUrl = null;

            try
            {
                // Kiểm tra danh mục có tồn tại không
                var categoryToUpdate = await _categoryRepository.GetByIdAsync(id);
                if (categoryToUpdate == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy danh mục.";
                    return response;
                }

                // Kiểm tra danh mục cha hợp lệ
                if (categoryUpdateRequest.CategoryMainId.HasValue)
                {
                    var parentCategory = await _categoryRepository.GetByIdAsync(categoryUpdateRequest.CategoryMainId.Value);
                    if (parentCategory == null)
                    {
                        response.Success = false;
                        response.Message = "Danh mục cha không tồn tại.";
                        return response;
                    }

                    if (categoryUpdateRequest.CategoryMainId == id)
                    {
                        response.Success = false;
                        response.Message = "Danh mục không thể là cha của chính nó.";
                        return response;
                    }
                }

                // Kiểm tra trùng tên hoặc mã danh mục
                var duplicateCategory = await _categoryRepository.GetSingleByConditionAsync(
                    c => ((categoryUpdateRequest.CategoryName != null && c.CategoryName.Equals(categoryUpdateRequest.CategoryName)) ||
                          (categoryUpdateRequest.CategoryCode != null && c.CategoryCode != null && c.CategoryCode.Equals(categoryUpdateRequest.CategoryCode)))
                          && c.Id != id
                );

                if (duplicateCategory != null)
                {
                    response.Success = false;
                    response.Message = "Tên hoặc mã danh mục đã tồn tại.";
                    return response;
                }

                // Cập nhật thông tin danh mục
                if (!string.IsNullOrEmpty(categoryUpdateRequest.CategoryName))
                    categoryToUpdate.CategoryName = categoryUpdateRequest.CategoryName;

                if (!string.IsNullOrEmpty(categoryUpdateRequest.CategoryCode))
                    categoryToUpdate.CategoryCode = categoryUpdateRequest.CategoryCode;

                if (categoryUpdateRequest.CategoryMainId.HasValue)
                    categoryToUpdate.CategoryMainId = categoryUpdateRequest.CategoryMainId;

                if (categoryUpdateRequest.Image != null)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(categoryUpdateRequest.Image.FileName, categoryUpdateRequest.Image.OpenReadStream()),
                        PublicId = Path.GetFileNameWithoutExtension(categoryUpdateRequest.Image.FileName)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    imageUrl = uploadResult.SecureUri.ToString();
                    categoryToUpdate.Image = imageUrl;
                }

                await _categoryRepository.UpdateAsync(categoryToUpdate);
                await _categoryRepository.SaveAsync();

                response.Success = true;
                response.Data = _mapper.Map<CategoryDTO>(categoryToUpdate);
                response.Message = "Cập nhật danh mục thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi hệ thống: {ex.Message} (Chi tiết: {ex.StackTrace})";
            }

            return response;
        }

    


    }








}
