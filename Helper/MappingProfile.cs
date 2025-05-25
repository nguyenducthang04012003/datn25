using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using PharmaDistiPro.DTO.Categorys;
using PharmaDistiPro.DTO.IssueNote;
using PharmaDistiPro.DTO.IssueNoteDetails;
using PharmaDistiPro.DTO.NoteCheckDetails;
using PharmaDistiPro.DTO.NoteChecks;
using PharmaDistiPro.DTO.Orders;
using PharmaDistiPro.DTO.OrdersDetails;
using PharmaDistiPro.DTO.Products;
using PharmaDistiPro.DTO.PurchaseOrders;
using PharmaDistiPro.DTO.PurchaseOrdersDetails;
using PharmaDistiPro.DTO.ReceivedNotes;
using PharmaDistiPro.DTO.StorageRooms;
using PharmaDistiPro.DTO.Suppliers;
using PharmaDistiPro.DTO.Units;
using PharmaDistiPro.DTO.Users;
using PharmaDistiPro.Helper.Enums;
using PharmaDistiPro.Models;
namespace PharmaDistiPro.Helper
{
    public class MappingProfile : Profile
    {
        private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Windows
        public MappingProfile()
        {
             

            #region Supplier
            CreateMap<Supplier, SupplierDTO>();
            CreateMap<SupplierDTO, Supplier>();
            CreateMap<SupplierInputRequest, Supplier>()
                  .ForMember(dest => dest.SupplierName, opt => opt.Condition(src => src.SupplierName != null))
                  .ForMember(dest => dest.SupplierCode, opt => opt.Condition(src => src.SupplierCode != null))
                  .ForMember(dest => dest.SupplierAddress, opt => opt.Condition(src => src.SupplierAddress != null))
                  .ForMember(dest => dest.SupplierPhone, opt => opt.Condition(src => src.SupplierPhone != null))
                  .ForMember(dest => dest.Status, opt => opt.Condition(src => src.Status.HasValue))
                  .ForMember(dest => dest.CreatedBy, opt => opt.Condition(src => src.CreatedBy.HasValue))
                  .ForMember(dest => dest.CreatedDate, opt => opt.Condition(src => src.CreatedDate.HasValue))
                  .ForMember(dest => dest.Id, opt => opt.Ignore()); 
            #endregion

            #region StorageRoom
            CreateMap<StorageRoom, StorageRoomDTO>();
            CreateMap<StorageRoomDTO, StorageRoom>();

            CreateMap<StorageRoomInputRequest, StorageRoom>()
           .ForMember(dest => dest.StorageRoomId, opt => opt.MapFrom(src => src.StorageRoomId))
          
   
           .ForMember(dest => dest.StorageRoomCode, opt => opt.MapFrom(src => src.StorageRoomCode))
           .ForMember(dest => dest.StorageRoomName, opt => opt.MapFrom(src => src.StorageRoomName))
           .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Capacity))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
           .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
           .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
           .ReverseMap(); // Để hỗ trợ ánh xạ 2 chiều

            #endregion
            #region StorageHistory
            CreateMap<StorageHistory, StorageHistoryDTO>()
                .ForMember(dest => dest.StorageRoom, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate)); // Ánh xạ trực tiếp, không chuyển đổi
            CreateMap<StorageHistoryDTO, StorageHistory>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate)); // Ánh xạ trực tiếp
            CreateMap<StorageHistoryInputRequest, StorageHistory>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate)); // Giữ nguyên hoặc null
            CreateMap<StorageHistory, StorageHistoryInputRequest>();
            #endregion



            #region Unit
            CreateMap<Unit, UnitDTO>();
            CreateMap<UnitDTO, Unit>();
            #endregion


            #region Category
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();


            CreateMap<CategoryInputRequest, Category>()
                .ForMember(dest => dest.CreatedByNavigation, opt => opt.Ignore());

            CreateMap<Category, CategoryInputRequest>();
            #endregion

            #region User
            CreateMap<User, UserDTO>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));


            CreateMap<UserDTO, User>()
            .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<UserInputRequest, UserDTO>();
            CreateMap<UserDTO, UserInputRequest>();

            CreateMap<User, UserInputRequest>();
            CreateMap<UserInputRequest, User>();
            #endregion

            #region Product

            CreateMap<Product, ProductDTO>()
       .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
       .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty))
       .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ImageProducts != null && src.ImageProducts.Any()
           ? src.ImageProducts.Select(ip => ip.Image).ToList()
           : new List<string>()))
       .ForMember(dest => dest.Storageconditions, opt => opt.MapFrom(src =>
           src.Storageconditions == (int)StorageCondition.Normal ? "Bảo quản thường" :
           src.Storageconditions == (int)StorageCondition.Cold ? "Bảo quản lạnh" :
           src.Storageconditions == (int)StorageCondition.Cool ? "Bảo quản mát" :
           "Không xác định"
       ));
            // Ánh xạ từ ImageProduct sang ImageProductDTO
            CreateMap<ImageProduct, ImageProductDTO>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

            // Ánh xạ từ ImageProductInputRequest sang ImageProduct
            CreateMap<ImageProductInputRequest, ImageProduct>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

            // Mapping từ ProductInputRequest sang ProductDTO
            CreateMap<ProductInputRequest, ProductDTO>()
                .ReverseMap();

            // Mapping từ ProductInputRequest sang Product (cho các thao tác cập nhật)
            CreateMap<ProductInputRequest, Product>()
                .ForMember(dest => dest.ProductName, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.ProductName)))
                .ForMember(dest => dest.ProductCode, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.ProductCode)))
                .ForMember(dest => dest.ManufactureName, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.ManufactureName)))
                .ForMember(dest => dest.Description, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Description)))
                .ForMember(dest => dest.Storageconditions, opt => opt.MapFrom(src => (int?)src.Storageconditions)) // Ánh xạ từ StorageCondition? sang int?
                .ForMember(dest => dest.Storageconditions, opt => opt.Condition(src => src.Storageconditions.HasValue))
                .ForMember(dest => dest.Unit, opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Unit)))
                .ForMember(dest => dest.CategoryId, opt => opt.Condition(src => src.CategoryId.HasValue))
                .ForMember(dest => dest.Vat, opt => opt.Condition(src => src.Vat.HasValue))
                .ForMember(dest => dest.SellingPrice, opt => opt.Condition(src => src.SellingPrice.HasValue))
                .ForMember(dest => dest.Weight, opt => opt.Condition(src => src.Weight.HasValue))
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Không ánh xạ CreatedBy
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()) // Không ánh xạ CreatedDate
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // Không ánh xạ Status
                .ForMember(dest => dest.ImageProducts, opt => opt.Ignore()) // Bỏ qua ánh xạ ImageProducts
                .ReverseMap()
                .ForMember(dest => dest.Storageconditions, opt => opt.MapFrom(src => (StorageCondition?)src.Storageconditions));

            // Mapping từ Product sang ProductOrderDto
            CreateMap<Product, ProductOrderDto>()
                .ForMember(dest => dest.Storageconditions, opt => opt.MapFrom(src => (StorageCondition?)src.Storageconditions)); // Ánh xạ từ int? sang StorageCondition?

            CreateMap<ProductOrderDto, Product>()
                .ForMember(dest => dest.Storageconditions, opt => opt.MapFrom(src => (int?)src.Storageconditions)); // Ánh xạ từ StorageCondition? sang int?;

            #endregion
            #region ImageProduct
            CreateMap<ImageProduct, ImageProductDTO>()
                
                .ReverseMap()
                .ForMember(dest => dest.Product, opt => opt.Ignore());
            #endregion

            #region NoteCheck


            // Ánh xạ từ Model -> DTO
            CreateMap<NoteCheck, NoteCheckDTO>()

      .ForMember(dest => dest.NoteCheckCode, opt => opt.MapFrom(src => src.NoteCheckCode)) // <-- thêm dòng này
      .ForMember(dest => dest.NoteCheckDetails, opt => opt.MapFrom(src => src.NoteCheckDetails))
      .ReverseMap(); // Cho phép ánh xạ ngược từ DTO -> Model

            // Ánh xạ từ RequestDTO -> Model
            CreateMap<NoteCheckRequestDTO, NoteCheck>()
                .ForMember(dest => dest.NoteCheckDetails, opt => opt.MapFrom(src => src.NoteCheckDetails))
                .ReverseMap(); // Cho phép ánh xạ ngược từ Model -> RequestDTO

            // Ánh xạ giữa NoteCheckDetail và DTO
            CreateMap<NoteCheckDetail, NoteCheckDetailsDTO>()
                .ForMember(dest => dest.ProductLot, opt => opt.MapFrom(src => src.ProductLot))
                .ReverseMap();

            // Ánh xạ từ RequestDTO -> Model cho NoteCheckDetail
            CreateMap<NoteCheckDetailRequestDTO, NoteCheckDetail>()
                .ReverseMap();

            // Ánh xạ ProductLot -> ProductLotCheckNoteDetailsDTO
            CreateMap<ProductLot, ProductLotCheckNoteDetailsDTO>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ReverseMap();




            #endregion

            #region productlot
            CreateMap<ProductLot, ProductLotIssueNoteDetailsDto>().ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ForMember(dest => dest.StorageRoomId, opt => opt.MapFrom(src => src.StorageRoomId))
                .ForMember(dest => dest.StorageRoomName, opt => opt.MapFrom(src => src.StorageRoom.StorageRoomName));

            CreateMap<ProductLotIssueNoteDetailsDto, ProductLot>();
            #endregion

            #region Order
            CreateMap<Order, OrderDto>()
                // lấy thông tin người mua hàng
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId)) // Lấy đúng ID khách hàng
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer)) // Chỉ lấy Customer nếu thỏa điều kiện

                // Lấy thông tin người tạo đơn hàng
                .ForMember(dest => dest.ConfirmedBy, opt => opt.MapFrom(src => src.ConfirmedBy)) // Người tạo đơn hàng
                .ForMember(dest => dest.ConfirmBy, opt => opt.MapFrom(src => src.ConfirmedByNavigation))// Người xác nhận đơn hàng

                // Lấy thông tin của warehouse được giao
                .ForMember(dest => dest.AssignTo, opt => opt.MapFrom(src => src.AssignTo))
                .ForMember(dest => dest.AssignToNavigation, opt => opt.MapFrom(src => src.AssignToNavigation));
            CreateMap<OrderDto, Order>();

            CreateMap<OrdersDetail, OrdersDetailDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductId : (int?)null))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));  // No need for _mapper.Map<ProductOrdersDetailDto>()

            CreateMap<OrdersDetailDto, OrdersDetail>()
                .ForMember(dest => dest.Product, opt => opt.Ignore());


            CreateMap<Order, OrderRequestDto>();
            CreateMap<OrderRequestDto, Models.Order>()
                .ForMember(dest => dest.OrdersDetails, opt => opt.Ignore()); // Tránh lặp mapping

            CreateMap<OrdersDetail, OrdersDetailsRequestDto>();
            CreateMap<OrdersDetailsRequestDto, OrdersDetail>()
                 .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId));

            #endregion

            #region issue note
            CreateMap<IssueNote, IssueNoteRequestDto>();
            CreateMap<IssueNoteRequestDto, IssueNote>();

            CreateMap<IssueNoteRequestDto, IssueNoteDetail>();
            CreateMap<IssueNoteDetail, IssueNoteRequestDto>();

            CreateMap<IssueNoteDto, IssueNoteRequestDto>();
            CreateMap<IssueNoteRequestDto, IssueNoteDto>();

            CreateMap<IssueNoteDto, IssueNote>();

            CreateMap<IssueNote, IssueNoteDto>();

            CreateMap<IssueNoteDetail, IssueNoteDetailDto>()
                .ForMember(dest => dest.ProductLotId, opt => opt.MapFrom(src => src.ProductLotId))
                .ForMember(dest => dest.ProductLot, opt => opt.MapFrom(src => src.ProductLot));

            CreateMap<IssueNoteDetailDto, IssueNoteDetail>();
            #endregion

            #region purchase order
            CreateMap<PurchaseOrder, PurchaseOrdersDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CreatedByNavigation, opt => opt.MapFrom(src => src.CreatedByNavigation))

                .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.SupplierId))
                .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.Supplier));
            CreateMap<PurchaseOrdersDto, PurchaseOrder>();

            CreateMap<PurchaseOrdersDetail, PurchaseOrdersDetailDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductId : (int?)null))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));  // No need for _mapper.Map<ProductOrdersDetailDto>()

            CreateMap<PurchaseOrdersDetailDto, PurchaseOrdersDetail>()
                .ForMember(dest => dest.Product, opt => opt.Ignore());


            CreateMap<PurchaseOrder, PurchaseOrdersRequestDto>();
            CreateMap<PurchaseOrdersRequestDto, PurchaseOrder>()
                .ForMember(dest => dest.PurchaseOrdersDetails, opt => opt.Ignore()); // Tránh lặp mapping

            CreateMap<PurchaseOrdersDetail, PurchaseOrdersDetailsRequestDto>();
            CreateMap<PurchaseOrdersDetailsRequestDto, PurchaseOrdersDetail>()
                 .ForMember(dest => dest.PurchaseOrderId, opt => opt.MapFrom(src => src.PurchaseOrderId));
            #endregion

            #region ReceivedNote
            CreateMap<ReceivedNote, ReceivedNoteDto>()
                // lấy thông tin người mua hàng
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy)) // Lấy đúng ID nguoi tao
                .ForMember(dest => dest.CreatedByNavigation, opt => opt.MapFrom(src => src.CreatedByNavigation)) // Chỉ lấy ID nguoi tao nếu thỏa điều kiện


                // Lấy thông tin của warehouse được giao
                .ForMember(dest => dest.PurchaseOrderId, opt => opt.MapFrom(src => src.PurchaseOrderId))
                .ForMember(dest => dest.PurchaseOrder, opt => opt.MapFrom(src => src.PurchaseOrder));

            CreateMap<ReceivedNoteDto, ReceivedNote>();
            #endregion

        }
    }
}