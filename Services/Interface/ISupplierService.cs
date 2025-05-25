using PharmaDistiPro.DTO.Suppliers;

namespace PharmaDistiPro.Services.Interface
{
    public interface ISupplierService
    {
        #region Supplier Management
        Task<Services.Response<SupplierDTO>> ActivateDeactivateSupplier(int supplierId, bool update);
        Task<Services.Response<SupplierDTO>> CreateNewSupplier(SupplierInputRequest supplierInputRequest);
        Task<Services.Response<SupplierDTO>> GetSupplierById(int supplierId);
     

        Task<Services.Response<IEnumerable<SupplierDTO>>> GetSupplierList();
        Task<Services.Response<SupplierDTO>> UpdateSupplier(SupplierInputRequest supplierUpdateRequest);
      

        #endregion

    }
}
