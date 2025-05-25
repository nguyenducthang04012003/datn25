using AutoMapper;
using PharmaDistiPro.DTO.ProductLots;
using PharmaDistiPro.DTO.StorageRooms;
using PharmaDistiPro.Helper.Enums;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Impl;
using PharmaDistiPro.Repositories.Interface;
using PharmaDistiPro.Services.Interface;
using System.Collections.Generic;

namespace PharmaDistiPro.Services.Impl
{
    public class ProductLotService : IProductLotService
    {
        private readonly IProductLotRepository _productLotRepository;
        private readonly IStorageRoomRepository _storageRoomRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper; // Khai báo _mapper
        private readonly ILotRepository _lotRepository;

        public ProductLotService(IProductLotRepository productLotRepository, IStorageRoomRepository storageRoomRepository, IProductRepository productRepository, IMapper mapper, ILotRepository lotRepository)
        {
            _productLotRepository = productLotRepository;
            _storageRoomRepository = storageRoomRepository;
            _productRepository = productRepository;
            _lotRepository = lotRepository;
        }

        public async Task<Response<int>> CheckQuantityProduct(int productId)
        {
            var response = new Response<int>();
            try
            {
                var checkQuantity = await _productLotRepository.CheckQuantityProduct(productId);

                if (checkQuantity == 0)
                {
                    return new Response<int>
                    {
                        StatusCode = 200,
                        Message = "Không tìm thấy sản phẩm trong kho"
                    };
                }
                response.Success = true;
                response.Data = checkQuantity;
                response.Message = "Lấy tổng số lượng sản phẩm thành công.";
                return response;
            }
            catch (Exception ex)
            {
                return new Response<int>
                {
                    StatusCode = 400,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response<List<ProductLotResponse>>> CreateProductLot(List<ProductLotRequest> productLots)
        {
            try
            {
                if (productLots == null || !productLots.Any())
                {
                    return new Response<List<ProductLotResponse>>
                    {
                        StatusCode = 400,
                        Message = "Danh sách lô hàng không có giá trị!"
                    };
                }
             
                
               foreach (var productLot in productLots)
                {

                    StorageRoom room = _storageRoomRepository.GetById(productLot.StorageRoomId);
                    Product product  =  await _productRepository.GetByIdAsyncProduct(productLot.ProductId);
                    if(room.Type.HasValue != product.Storageconditions.HasValue)
                    {
                        return new Response<List<ProductLotResponse>>
                        {
                            StatusCode = 400,
                            Message = $" Sản phẩm {product.ProductName} không phù hợp với môi trường phòng {room.StorageRoomName}, cần điều kiện {ConvertTypeToString(product.Storageconditions)}"
                        };
                    }
                    if(room.RemainingRoomVolume - Math.Round((double)((productLot.OrderQuantity * product.VolumePerUnit) / 1000000), 3) < 0)
                    {
                        return new Response<List<ProductLotResponse>>
                        {
                            StatusCode = 400,
                            Message = $"Phòng {room.StorageRoomName} không chứa được lô hàng  {product.ProductName}này"
                        };
                    }
                }
                var productLotResponses = new List<ProductLotResponse>();
                
                var storageRoomsToUpdate = new Dictionary<int, StorageRoom>();

                foreach (var productLotRequest in productLots)
                {
                    if (productLotRequest.LotId == null)
                    {
                        return new Response<List<ProductLotResponse>>
                        {
                            StatusCode = 400,
                            Message = "LotId không có giá trị!"
                        };
                    }

                    var lot = await _productLotRepository.GetLotById(productLotRequest.LotId);
                    if (lot == null)
                    {
                        return new Response<List<ProductLotResponse>>
                        {
                            StatusCode = 404,
                            Message = $"Không tìm thấy lô có ID {productLotRequest.LotId}!"
                        };
                    }

                    var product = await _productRepository.GetByIdAsync(productLotRequest.ProductId);
                    if (product == null)
                    {
                        return new Response<List<ProductLotResponse>>
                        {
                            StatusCode = 404,
                            Message = $"Không tìm thấy sản phẩm có ID {productLotRequest.ProductId}!"
                        };
                    }

                    if (productLotRequest.StorageRoomId == null)
                    {
                        return new Response<List<ProductLotResponse>>
                        {
                            StatusCode = 400,
                            Message = $"StorageRoomId không có giá trị cho lô {productLotRequest.LotId}!"
                        };
                    }

                    if (productLotRequest.OrderQuantity == null || productLotRequest.OrderQuantity <= 0)
                    {
                        return new Response<List<ProductLotResponse>>
                        {
                            StatusCode = 400,
                            Message = $"Số lượng đặt hàng của lô {productLotRequest.LotId} không hợp lệ!"
                        };
                    }

                    if (product.VolumePerUnit == null || product.VolumePerUnit <= 0)
                    {
                        return new Response<List<ProductLotResponse>>
                        {
                            StatusCode = 400,
                            Message = $"Dung tích đơn vị của sản phẩm {productLotRequest.ProductId} không hợp lệ!"
                        };
                    }

                    
                    double requiredVolume =   Math.Round((double)((productLotRequest.OrderQuantity * product.VolumePerUnit) / 1000000), 3);

                    // Lấy phòng từ cache hoặc DB một lần
                    StorageRoom? storageRoom;
                    if (storageRoomsToUpdate.ContainsKey(productLotRequest.StorageRoomId.Value))
                    {
                        storageRoom = storageRoomsToUpdate[productLotRequest.StorageRoomId.Value];
                    }
                    else
                    {
                        storageRoom = await _storageRoomRepository.GetByIdAsync(productLotRequest.StorageRoomId.Value);
                        if (storageRoom == null)
                        {
                            // Không kiểm tra lỗi ở đây như yêu cầu, chỉ tạo mới đối tượng để cập nhật
                            storageRoom = new StorageRoom
                            {
                                StorageRoomId = productLotRequest.StorageRoomId.Value,
                                RemainingRoomVolume = 0 // Mặc định nếu không có trong DB
                            };
                        }

                        storageRoomsToUpdate[productLotRequest.StorageRoomId.Value] = storageRoom;
                    }

                    // Cập nhật RemainingRoomVolume (dù có đủ dung tích hay không)
                    storageRoom.RemainingRoomVolume = (storageRoom.RemainingRoomVolume ?? 0) - requiredVolume;

                    // Map ProductLotRequest to ProductLot
                    var productLot = new ProductLot
                    {
                        LotId = productLotRequest.LotId,
                        ProductId = productLotRequest.ProductId,
                        ManufacturedDate = productLotRequest.ManufacturedDate,
                        ExpiredDate = productLotRequest.ExpiredDate,
                        OrderQuantity = productLotRequest.OrderQuantity,
                        SupplyPrice = productLotRequest.SupplyPrice,
                        Status = productLotRequest.Status,
                        StorageRoomId = productLotRequest.StorageRoomId,
                        Quantity = 0
                    };

                    var createdProductLot = await _productLotRepository.CreateProductLot(productLot);

                    var data = new ProductLotResponse
                    {
                        Id = createdProductLot.ProductLotId,
                        LotId = createdProductLot.LotId,
                        ProductId = createdProductLot.ProductId,
                        ProductName = product.ProductName,
                        LotCode = lot.LotCode,
                        OrderQuantity = createdProductLot.OrderQuantity,
                        StorageRoomId = createdProductLot.StorageRoomId,
                        SupplyPrice = createdProductLot.SupplyPrice,
                        ManufacturedDate = createdProductLot.ManufacturedDate,
                        ExpiredDate = createdProductLot.ExpiredDate,
                        Status = createdProductLot.Status,
                        Quantity = 0
                    };
                    productLotResponses.Add(data);
                }

                foreach (var storageRoom in storageRoomsToUpdate.Values)
                {
                    await _storageRoomRepository.UpdateAsync(storageRoom);
                }
                await _storageRoomRepository.SaveAsync();

                return new Response<List<ProductLotResponse>>
                {
                    StatusCode = 200,
                    Success = true,
                    Message = "Cập nhật dung tích phòng kho thành công!",
                    Data = productLotResponses
                };
            }
            catch (Exception ex)
            {
                return new Response<List<ProductLotResponse>>
                {
                    StatusCode = 500,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }


        public async Task<Response<ProductLotResponse>> GetProductLotById(int id)
        {
            ProductLot productlot = await _productLotRepository.GetProductLotById(id);
            var response = new Response<ProductLotResponse>();
            if (productlot == null)
            {
                response = new Response<ProductLotResponse>
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy lô hàng!"
                };
                return response;
            }
            ProductLotResponse data = new ProductLotResponse()
            {
                Id = productlot.ProductLotId,
                SupplyPrice = productlot.SupplyPrice,
                Quantity = productlot.Quantity,
                ManufacturedDate = productlot.ManufacturedDate,
                ExpiredDate = productlot.ExpiredDate,
                ProductId = productlot.ProductId,
                LotId = productlot.LotId,
                Status = productlot.Status,
                ProductName = productlot.Product.ProductName,
                LotCode = productlot.Lot.LotCode,
                StorageRoomId = productlot.StorageRoomId
            };
            response = new Response<ProductLotResponse>
            {
                StatusCode = 200,
                Data = data
            };
            return response;
        }

        public async Task<Response<List<ProductLotResponse>>> GetProductLotList()
        {
            var productlots = await _productLotRepository.GetProductLotList();

            var response = new Response<List<ProductLotResponse>>();
            if (productlots == null)
            {
                response = new Response<List<ProductLotResponse>>
                {
                    StatusCode = 500,
                    Message = "Lỗi khi lấy dữ liệu lô hàng!"
                };
                return response;
            }
            if (productlots.Count <= 0)
            {
                response = new Response<List<ProductLotResponse>>
                {
                    StatusCode = 404,
                    Message = "Không có lô hàng nào!"
                };
                return response;
            }
            productlots = productlots.OrderByDescending(x => x.ProductLotId).ToList();
            List<ProductLotResponse> dataSet = new List<ProductLotResponse>();
            foreach (ProductLot item in productlots)
            {
                ProductLotResponse data = new ProductLotResponse()
                {
                    Id = item.ProductLotId,
                    SupplyPrice = item.SupplyPrice,
                    Quantity = item.Quantity,
                    ManufacturedDate = item.ManufacturedDate,
                    ExpiredDate = item.ExpiredDate,
                    ProductId = item.ProductId,
                    LotId = item.LotId,
                    Status = item.Status,
                    ProductName = item.Product.ProductName,
                    OrderQuantity = item.OrderQuantity,
                    StorageRoomId = item.StorageRoomId,
                    LotCode = item.Lot.LotCode
                };
                dataSet.Add(data);
            }
            response = new Response<List<ProductLotResponse>>
            {
                StatusCode = 200,
                Data = dataSet
            };
            return response;
        }

        public async Task<Response<ProductLotResponse>> UpdateProductLot(ProductLotRequest ProductLot)
        {
            var response = new Response<ProductLotResponse>();
            if (ProductLot == null)
            {
                response = new Response<ProductLotResponse>
                {
                    StatusCode = 400,
                    Message = "Dữ liệu không hợp lệ!"
                };
                return response;
            }
            if(ProductLot.LotId == null)
            {
                response = new Response<ProductLotResponse>
                {
                    StatusCode = 400,
                    Message = "LotId không có giá trị!"
                };
                return response;
            }
            Lot lot = await _productLotRepository.GetLotById(ProductLot.LotId);
            if (lot == null)
            {
                response = new Response<ProductLotResponse>
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy lô!"
                };
                return response;
            }
            ProductLot OldProductLot = await _productLotRepository.GetProductLotById(ProductLot.ProductLotId);
            if (OldProductLot == null)
            {
                response = new Response<ProductLotResponse>
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy lô hàng!"
                };
                return response;
            }
            OldProductLot.ProductId = ProductLot.ProductId;
            OldProductLot.ManufacturedDate = ProductLot.ManufacturedDate;
            OldProductLot.ExpiredDate = ProductLot.ExpiredDate;
            OldProductLot.SupplyPrice = ProductLot.SupplyPrice;
            var updatedProductLot = await _productLotRepository.UpdateProductLot(OldProductLot);
            if (updatedProductLot == null)
            {
                response = new Response<ProductLotResponse>
                {
                    StatusCode = 500,
                    Message = "Lỗi khi cập nhật lô hàng!"
                };
                return response;
            }
            ProductLotResponse data = new ProductLotResponse()
            {
                Id = updatedProductLot.ProductLotId,
                SupplyPrice = updatedProductLot.SupplyPrice,
                Quantity = updatedProductLot.Quantity,
                ManufacturedDate = updatedProductLot.ManufacturedDate,
                ExpiredDate = updatedProductLot.ExpiredDate,
                ProductId = updatedProductLot.ProductId,
                LotId = updatedProductLot.LotId,
                Status = updatedProductLot.Status,
                ProductName = updatedProductLot.Product.ProductName,
                LotCode = updatedProductLot.Lot.LotCode
            };
            response = new Response<ProductLotResponse>
            {
                StatusCode = 200,
                Data = data
            };
            return response;
        }


        public async Task<Response<List<ProductLotResponse>>> AutoUpdateProductLotStatusAsync()
        {
            var response = new Response<List<ProductLotResponse>>();
            var changedLots = new List<ProductLotResponse>(); // Danh sách sản phẩm thay đổi trạng thái
            try
            {
                var productLots = await _productLotRepository.GetAllAsync();
                var now = DateTime.Now;

                foreach (var lot in productLots)
                {
                    var originalStatus = lot.Status; // Lưu trạng thái cũ

                    if (lot.ExpiredDate <= now) 
                    {
                        lot.Status = 3; // Hết hạn
                    }
                    else if (lot.ExpiredDate <= now.AddMonths(3))
                    {
                        lot.Status = 0; // Sắp hết hạn – ngừng bán
                    }
                    else if (lot.ExpiredDate <= now.AddMonths(5))
                    {
                        lot.Status = 2; // Sắp hết hạn – ưu tiên xuất
                    }
                    else
                    {
                        lot.Status = 1; // Còn hạn – có thể bán
                    }


                    // Kiểm tra nếu trạng thái thay đổi
                    if (originalStatus != lot.Status)
                    {
                        // Lấy thông tin sản phẩm (ProductName) và lưu vào danh sách
                        string productName = "Unknown";
                        var product = await _productRepository.GetByIdAsync(lot.ProductId);
                        if (product != null)
                        {
                            productName = product.ProductName ?? "Unknown";
                        }

                        changedLots.Add(new ProductLotResponse
                        {
                            Id = lot.ProductLotId,
                            ProductId = lot.ProductId,
                            LotId = lot.LotId,
                            Status = lot.Status,
                            ProductName = productName,
                            ExpiredDate = lot.ExpiredDate,
                            ManufacturedDate = lot.ManufacturedDate
                        });
                    }

                    await _productLotRepository.UpdateAsync(lot);
                }

                await _productLotRepository.SaveAsync();

                // Nếu có sản phẩm thay đổi trạng thái thì trả về danh sách
                if (changedLots.Any())
                {
                    response.Success = true;
                    response.Data = changedLots;
                }
                else
                {
                    response.Success = true;
                    response.Data = new List<ProductLotResponse>(); // Nếu không có sản phẩm thay đổi, trả về danh sách rỗng
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi cập nhật trạng thái sản phẩm: {ex.Message}";
            }

            return response;
        }

        public async Task SendProductLotStatusChangeNotificationAsync(List<ProductLotResponse> changedLots)
        {
            // Tạo thông báo về danh sách sản phẩm thay đổi trạng thái
            var message = $"Có {changedLots.Count} sản phẩm thay đổi trạng thái hôm nay:\n";
            foreach (var lot in changedLots)
            {
                message += $"- Sản phẩm: {lot.ProductName}, Trạng thái mới: {lot.Status}, Ngày hết hạn: {lot.ExpiredDate.ToString()}\n";
            }



        }

        public async Task<Response<List<ProductLotResponse>>> GetProductLotsByStatusAsync(int status)
        {
            var response = new Response<List<ProductLotResponse>>();

            try
            {
                // Kiểm tra giá trị status hợp lệ (0, 1, 2)
                if (status != 0 && status != 1 && status != 2)
                {
                    response.Success = false;
                    response.Message = "Trạng thái không hợp lệ. Chỉ chấp nhận giá trị 0 (ngừng bán), 1 (còn hạn), hoặc 2 (ưu tiên xuất).";
                    response.StatusCode = 400;
                    return response;
                }

                // Lấy danh sách ProductLot theo status
                var productLots = await _productLotRepository.GetByConditionAsync(x => x.Status == status);

                // Kiểm tra nếu danh sách rỗng
                if (productLots == null || !productLots.Any())
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy lô sản phẩm nào với trạng thái {status}.";
                    response.StatusCode = 404;
                    return response;
                }

                // Mapping thủ công sang ProductLotResponse
                var productLotDtos = new List<ProductLotResponse>();
                foreach (var lot in productLots)
                {
                    // Lấy ProductName từ ProductId
                    string productName = "Unknown";
                    var product = await _productRepository.GetByIdAsync(lot.ProductId);
                    if (product != null)
                    {
                        productName = product.ProductName ?? "Unknown";
                    }

                    // Lấy LotCode từ LotId
                    string lotCode = "Unknown";
                    var lotRecord = await _lotRepository.GetByIdAsync(lot.LotId);
                    if (lotRecord != null)
                    {
                        lotCode = lotRecord.LotCode ?? "Unknown";
                    }

                    productLotDtos.Add(new ProductLotResponse
                    {
                        Id = lot.ProductLotId,
                        LotId = lot.LotId,
                        ProductId = lot.ProductId,
                        Quantity = lot.Quantity,
                        ManufacturedDate = lot.ManufacturedDate,
                        ExpiredDate = lot.ExpiredDate,
                        SupplyPrice = lot.SupplyPrice,
                        OrderQuantity = lot.OrderQuantity,
                        Status = lot.Status,
                        StorageRoomId = lot.StorageRoomId,
                        LotCode = lotCode,
                        ProductName = productName
                    });
                }

                response.Success = true;
                response.Data = productLotDtos;
                response.Message = $"Lấy danh sách lô sản phẩm với trạng thái {status} thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi lấy danh sách lô sản phẩm: {ex.Message}";
                response.StatusCode = 500;
            }

            return response;
        }





        // Get quantity product can bye
        public async Task<Response<int>> GetQuantityByProductIdAsync(int productId)
        {
            var response = new Response<int>();
            try
            {
                var productLots = await _productLotRepository
                    .GetAllAsync(); 
                var totalQuantity = productLots
                    .Where(lot => lot.ProductId == productId &&( lot.Status == 1|| lot.Status ==2)) //khong het han
                    .Sum(lot => lot.Quantity ?? 0); 

                response.Success = true;
                response.Data = totalQuantity;
                response.Message = "Lấy tổng số lượng sản phẩm thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi khi tính tổng số lượng: {ex.Message}";
            }

            return response;
        }


        public async Task<Response<IEnumerable<StorageRoomDTO>>> ListCompatibleStorageRoomsAsync(int productLotId)
        {
            var response = new Response<IEnumerable<StorageRoomDTO>>();

            try
            {
                // Bước 1: Lấy ProductLot
                var productLot = await _productLotRepository.GetByIdAsync(productLotId);
                if (productLot == null)
                {
                    response.Success = false;
                    response.Message = "ProductLotId không hợp lệ";
                    return response;
                }

                // Bước 2: Lấy Product
                var product = await _productRepository.GetByIdAsync(productLot.ProductId);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Sản phẩm liên quan đến lô không tồn tại";
                    return response;
                }

                // Bước 3: Kiểm tra Storageconditions
                if (!product.Storageconditions.HasValue)
                {
                    response.Success = false;
                    response.Message = "Điều kiện bảo quản của sản phẩm không được xác định";
                    return response;
                }

                // Bước 4: Tính dung tích cần thiết
                double requiredVolume = Math.Round(
      ((double)(product.VolumePerUnit.GetValueOrDefault(0) / 1000000)) * (productLot.OrderQuantity.GetValueOrDefault(0)),
      2
  );
                if (requiredVolume <= 0)
                {
                    response.Success = false;
                    response.Message = "Số lượng đặt hàng hoặc dung tích đơn vị không hợp lệ";
                    return response;
                }

                // Bước 5: Ánh xạ StorageCondition sang StorageRoomType (int)
                var compatibleRoomType = MapToStorageRoomType((StorageCondition)product.Storageconditions.Value);

                // Bước 6: Tìm các phòng kho phù hợp
                var storageRooms = await _storageRoomRepository.GetAllAsync();
                if (storageRooms == null)
                {
                    response.Success = false;
                    response.Message = "Không thể truy xuất danh sách phòng kho";
                    return response;
                }

                var compatibleRooms = storageRooms
                    .Where(r => r != null
                             && r.Type == compatibleRoomType
                             && r.Status == true
                             && (r.RemainingRoomVolume ?? 0) >= requiredVolume)
                    .ToList();

                if (!compatibleRooms.Any())
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy phòng kho phù hợp";
                    return response;
                }

                // Bước 7: Chuyển đổi sang DTO thủ công
                var dtos = new List<StorageRoomDTO>();
                foreach (var room in compatibleRooms)
                {
                    dtos.Add(new StorageRoomDTO
                    {
                        StorageRoomId = room.StorageRoomId,
                        StorageRoomCode = room.StorageRoomCode,
                        StorageRoomName = room.StorageRoomName,
                        Type = ConvertTypeToString(room.Type),
                        Capacity = room.Capacity,
                        RemainingRoomVolume = Math.Round((double)(room.Capacity ?? 0) - requiredVolume, 2),
                        Status = room.Status,
                        CreatedBy = room.CreatedBy,
                        CreatedDate = room.CreatedDate
                    });
                }

                response.Success = true;
                response.Data = dtos;
                response.Message = "Tìm thấy các phòng kho phù hợp";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi: {ex.Message}";
                return response;
            }
        }

        private static int MapToStorageRoomType(StorageCondition condition)
        {
            return condition switch
            {
                StorageCondition.Normal => (int)StorageRoomType.Normal, // 1 -> 1
                StorageCondition.Cold => (int)StorageRoomType.Freezer,  // 2 -> 3
                StorageCondition.Cool => (int)StorageRoomType.Cool,    // 3 -> 2
                _ => throw new ArgumentException("Điều kiện bảo quản không hợp lệ")
            };
        }
        private string ConvertTypeToString(int? type)
        {
            if (!type.HasValue) return "Không xác định";

            return ((StorageRoomType)type.Value) switch
            {
                StorageRoomType.Normal => "Phòng thường (Nhiệt độ: 15-30 ; Độ ẩm < 75%)",
                StorageRoomType.Cool => "Phòng mát (Nhiệt độ: 8-15 ; Độ ẩm < 70%)",
                StorageRoomType.Freezer => "Phòng đông lạnh (Nhiệt độ: 2-8 ; Độ ẩm < 45%)",
                _ => "Không xác định"
            };
        }
    }
}

