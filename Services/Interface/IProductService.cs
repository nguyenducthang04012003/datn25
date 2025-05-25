using PharmaDistiPro.DTO.Products;

namespace PharmaDistiPro.Services.Interface
{
    public interface IProductService
    {
        Task<Response<ProductDTO>> ActivateDeactivateProduct(int productId, bool update);
        Task<Response<ProductDTO>> CreateNewProduct(ProductInputRequest productInputRequest);
        Task<Response<ProductDTO>> GetProductById(int productId);
        Task<Response<IEnumerable<ProductDTO>>> GetProductList();
  
        Task<Response<IEnumerable<ProductDTO>>> GetProductListCustomer();
        Task<Response<ProductDTO>> UpdateProduct(ProductInputRequest productUpdateRequest);

    }
}
