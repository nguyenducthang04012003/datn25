using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using PharmaDistiPro.DTO.Products;
using PharmaDistiPro.Helper.Enums;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Impl;
using PharmaDistiPro.Repositories.Interface;
using PharmaDistiPro.Services.Interface;
using System.Linq.Expressions;

namespace PharmaDistiPro.Services.Impl
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly Cloudinary _cloudinary;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository; 


        public ProductService(IProductRepository product, IMapper mapper, Cloudinary cloudinary, ICategoryRepository category)
        {
            _productRepository = product;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _categoryRepository = category;
        }

        
        
        public async Task<Response<IEnumerable<ProductDTO>>> GetProductList()
        {
            var response = new Response<IEnumerable<ProductDTO>>();

            try
            {
                var products = await _productRepository.GetAllAsyncProduct() ?? new List<Product>();

                var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(products).ToList();

                foreach (var productDto in productDtos)
                {
                    var product = products.FirstOrDefault(p => p.ProductId == productDto.ProductId);

                    if (product != null)
                    {
                        // Gán danh sách ảnh
                        productDto.Images = product.ImageProducts?.Select(ip => ip.Image).ToList();
                
                        // Gán tên điều kiện bảo quản
                        productDto.Storageconditions = product.Storageconditions switch
                        {
                            (int)StorageCondition.Normal => "Bảo quản thường( Nhiệt độ: 15-30 ; Độ ẩm < 75%)",
                            (int)StorageCondition.Cold => "Bảo quản lạnh( Nhiệt độ: 2-8 ; Độ ẩm < 45%)",
                            (int)StorageCondition.Cool => "Bảo quản mát( Nhiệt độ: 8-15 ; Độ ẩm < 70%)",
                            _ => "Không xác định"
                        };
                    }
                }

                response.Data = productDtos;
                response.Success = true;
                response.Message = products.Any() ? "Lấy danh sách sản phẩm thành công" : "Không có sản phẩm nào.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách sản phẩm.";
                Console.WriteLine($"Lỗi: {ex}");
            }

            return response;
        }
        public async Task<Response<IEnumerable<ProductDTO>>> GetProductListCustomer()
        {
            var response = new Response<IEnumerable<ProductDTO>>();

            try
            {
                var products = await _productRepository.GetAllAsyncCustomerProduct() ?? new List<Product>();

                var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(products).ToList();

                foreach (var productDto in productDtos)
                {
                    var product = products.FirstOrDefault(p => p.ProductId == productDto.ProductId);

                    if (product != null)
                    {
                        productDto.Images = product.ImageProducts?.Select(ip => ip.Image).ToList();

                        productDto.Storageconditions = product.Storageconditions switch
                        {
                            (int)StorageCondition.Normal => "Bảo quản thường",
                            (int)StorageCondition.Cold => "Bảo quản lạnh",
                            (int)StorageCondition.Cool => "Bảo quản mát",
                            _ => "Không xác định"
                        };
                    }
                }

                response.Data = productDtos;
                response.Success = true;
                response.Message = products.Any() ? "Lấy danh sách sản phẩm thành công" : "Không có sản phẩm nào.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách sản phẩm.";
                Console.WriteLine($"Lỗi: {ex}");
            }

            return response;
        }


        // Deactivate product
        public async Task<Response<ProductDTO>> ActivateDeactivateProduct(int productId, bool update)
        {
            var response = new Response<ProductDTO>();
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    response.Success = false;
                    response.Data = null; 
                    response.Message = "Không tìm thấy sản phẩm";
                    return response;
                }

                product.Status = update;
                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveAsync();
                response.Success = true;
                response.Data = _mapper.Map<ProductDTO>(product);
                response.Message = "Cập nhật thành công";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<Response<ProductDTO>> CreateNewProduct(ProductInputRequest productInputRequest)
        {
            var response = new Response<ProductDTO>();
            try
            {
                // Kiểm tra CategoryId
                if (!productInputRequest.CategoryId.HasValue)
                {
                    response.Success = false;
                    response.Message = "CategoryId là bắt buộc.";
                    return response;
                }

                // Lấy category
                var category = await _categoryRepository.GetByIdAsync(productInputRequest.CategoryId.Value);
                if (category == null)
                {
                    response.Success = false;
                    response.Message = "Danh mục không tồn tại.";
                    return response;
                }

                // Chỉ cho phép chọn danh mục con
                var existingCategory = await _categoryRepository.GetSingleByConditionAsync(c => c.CategoryMainId == category.CategoryMainId);
                if (existingCategory == null)
                {
                    response.Success = false;
                    response.Message = "Chỉ được chọn danh mục con.";
                    return response;
                }

                // Kiểm tra điều kiện bảo quản hợp lệ
                if (!Enum.IsDefined(typeof(StorageCondition), productInputRequest.Storageconditions))
                {
                    response.Success = false;
                    response.Message = "Điều kiện bảo quản không hợp lệ. Chỉ được chọn: Bảo quản thường (1), Bảo quản lạnh (2), hoặc Bảo quản mát (3).";
                    return response;
                }

                // Tạo sản phẩm mới từ input request
                var newProduct = _mapper.Map<Product>(productInputRequest);

                // ⚠️ Gán Storageconditions đúng kiểu int?
                newProduct.Storageconditions = productInputRequest.Storageconditions.HasValue
                    ? (int?)productInputRequest.Storageconditions.Value
                    : null;

                newProduct.CreatedDate = DateTime.Now;
                newProduct.Status = true;

                // Upload ảnh (tối đa 5 ảnh)
                if (productInputRequest.Images != null && productInputRequest.Images.Any())
                {
                    int currentImageCount = 0;

                    foreach (var image in productInputRequest.Images)
                    {
                        if (currentImageCount >= 5)
                        {
                            response.Success = false;
                            response.Message = "Mỗi sản phẩm chỉ được tối đa 5 ảnh.";
                            return response;
                        }

                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(image.FileName, image.OpenReadStream()),
                            PublicId = Path.GetFileNameWithoutExtension(image.FileName)
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        var imageUrl = uploadResult.SecureUri.ToString();

                        newProduct.ImageProducts.Add(new ImageProduct
                        {
                            Image = imageUrl
                        });

                        currentImageCount++;
                    }
                }

                // Lưu sản phẩm để lấy ProductId
                await _productRepository.InsertAsync(newProduct);
                await _productRepository.SaveAsync();

                // Tạo mã sản phẩm
                string categoryCode = category.CategoryCode ?? "DEFAULT";
                string productNumberFormatted = newProduct.ProductId.ToString("D4");
                string productCode = $"{categoryCode}_{productNumberFormatted}";

                newProduct.ProductCode = productCode;
                await _productRepository.UpdateAsync(newProduct);
                await _productRepository.SaveAsync();

                // Gán ProductId cho hình ảnh
                if (newProduct.ImageProducts.Any())
                {
                    foreach (var imageProduct in newProduct.ImageProducts)
                    {
                        imageProduct.ProductId = newProduct.ProductId;
                    }
                    await _productRepository.SaveAsync();
                }

                // Convert ra DTO
                var dto = _mapper.Map<ProductDTO>(newProduct);

                dto.Storageconditions = newProduct.Storageconditions switch
                {
                    (int)StorageCondition.Normal => "Bảo quản thường",
                    (int)StorageCondition.Cold => "Bảo quản lạnh",
                    (int)StorageCondition.Cool => "Bảo quản mát",
                    _ => "Không xác định"
                };

                response.Message = $"Tạo mới sản phẩm thành công. Điều kiện bảo quản: {dto.Storageconditions}";
                response.Success = true;
                response.Data = dto;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi: {ex.Message}";
                return response;
            }
        }



        // Get product by Id
        public async Task<Response<ProductDTO>> GetProductById(int productId)
        {
            var response = new Response<ProductDTO>();
            try
            {
                var product = await _productRepository.GetByIdAsyncProduct(productId);
                if (product == null)
                {
                    response.Success = false;
                    response.Data = null;
                    response.Message = "Không tìm thấy sản phẩm";
                    return response;
                }

                var productDto = _mapper.Map<ProductDTO>(product);

           
                productDto.Images = product.ImageProducts?.Select(ip => ip.Image).ToList();

                response.Success = true;
                response.Data = productDto;
                response.Message = "Product found";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<Response<ProductDTO>> UpdateProduct(ProductInputRequest productUpdateRequest)
        {
            var response = new Response<ProductDTO>();
            try
            {
          
                var productToUpdate = await _productRepository.GetByIdAsync(productUpdateRequest.ProductId);
                if (productToUpdate == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy sản phẩm.";
                    return response;
                }

           
                var originalProductCode = productToUpdate.ProductCode;

               
                _mapper.Map(productUpdateRequest, productToUpdate);
                productToUpdate.ProductCode = originalProductCode; 

            
                var currentImages = productToUpdate.ImageProducts?.ToList() ?? new List<ImageProduct>();
                int currentImageCount = currentImages.Count;

                if (productUpdateRequest.Images != null && productUpdateRequest.Images.Any())
                {
                    if (currentImageCount + productUpdateRequest.Images.Count > 5)
                    {
                        response.Success = false;
                        response.Message = "Mỗi sản phẩm chỉ được tối đa 5 ảnh.";
                        return response;
                    }

                    foreach (var image in productUpdateRequest.Images)
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(image.FileName, image.OpenReadStream()),
                            PublicId = Path.GetFileNameWithoutExtension(image.FileName)
                        };

                        try
                        {
                            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                            var imageUrl = uploadResult.SecureUri.ToString();

                            productToUpdate.ImageProducts.Add(new ImageProduct
                            {
                                Image = imageUrl,
                                ProductId = productToUpdate.ProductId
                            });

                            currentImageCount++;
                        }
                        catch (Exception uploadEx)
                        {
                            response.Success = false;
                            response.Message = $"Lỗi khi tải ảnh lên: {uploadEx.Message}";
                            return response;
                        }
                    }
                }

            
                await _productRepository.UpdateAsync(productToUpdate);
                await _productRepository.SaveAsync();

                response.Success = true;
                var updatedProductDto = _mapper.Map<ProductDTO>(productToUpdate);

             
                updatedProductDto.Storageconditions = productToUpdate.Storageconditions switch
                {
                    (int)StorageCondition.Normal => "Bảo quản thường",
                    (int)StorageCondition.Cold => "Bảo quản lạnh",
                    (int)StorageCondition.Cool => "Bảo quản mát",
                    _ => "Không xác định"
                };

                response.Data = updatedProductDto;
                response.Message = "Cập nhật thành công.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Đã xảy ra lỗi trong quá trình cập nhật sản phẩm: {ex.Message}";
                return response;
            }
        }


    }
}
