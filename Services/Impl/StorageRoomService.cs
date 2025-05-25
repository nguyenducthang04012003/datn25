using AutoMapper;
using PharmaDistiPro.DTO.StorageRooms;
using PharmaDistiPro.Helper.Enums;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Interface;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Services.Impl
{
    public class StorageRoomService : IStorageRoomService
    {
        private readonly IStorageRoomRepository _storageRoomRepository;
        private readonly IMapper _mapper;

        public StorageRoomService(IStorageRoomRepository storageRoom, IMapper mapper)
        {
            _storageRoomRepository = storageRoom;
            _mapper = mapper;
        }

        private string ConvertTypeToString(int? type)
        {
            if (!type.HasValue) return "Không xác định";

            return ((StorageRoomType)type.Value) switch
            {
                StorageRoomType.Normal => "Phòng thường( Nhiệt độ: 15-30 ; Độ ẩm < 75%)",
                StorageRoomType.Cool => "Phòng mát( Nhiệt độ: 8-15 ; Độ ẩm < 70%)",
                StorageRoomType.Freezer => "Phòng đông lạnh( Nhiệt độ: 2-8 ; Độ ẩm < 45%)",
                _ => "Không xác định"
            };
        }
        // Get all storageRooms
        public async Task<Response<IEnumerable<StorageRoomDTO>>> GetStorageRoomList()
        {
            var response = new Response<IEnumerable<StorageRoomDTO>>();

            try
            {
                var storageRooms = await _storageRoomRepository.GetAllAsync();

                if (!storageRooms.Any())
                {
                    response.Success = false;
                    response.Message = "Không có dữ liệu";
                }
                else
                {
                    var dtos = _mapper.Map<List<StorageRoomDTO>>(storageRooms);

                    // Chuyển type sang dạng string
                    foreach (var dto in dtos)
                    {
                        var origin = storageRooms.First(x => x.StorageRoomId == dto.StorageRoomId);
                        dto.Type = ConvertTypeToString(origin.Type);
                    }

                    response.Success = true;
                    response.Data = dtos;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // Get storageRoom by Id
        public async Task<Response<StorageRoomDTO>> GetStorageRoomById(int storageRoomId)
        {
            var response = new Response<StorageRoomDTO>();
            try
            {
                var storageRoom = await _storageRoomRepository.GetByIdAsync(storageRoomId);
                if (storageRoom == null)
                {
                    response.Success = false;
                    response.Data = null;
                    response.Message = "Không tìm thấy phòng chứa kho ";
                    return response;
                }
                else
                {
                    var dto = _mapper.Map<StorageRoomDTO>(storageRoom);
                    dto.Type = ConvertTypeToString(storageRoom.Type);

                    response.Success = true;
                    response.Data = dto;
                    response.Message = "StorageRoom found";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

        // Create new StorageRoom
        public async Task<Response<StorageRoomDTO>> CreateNewStorageRoom(StorageRoomInputRequest storageRoomInputRequest)
        {
            var response = new Response<StorageRoomDTO>();
            try
            {
                // Kiểm tra trùng tên
                var existingStorageRoom = await _storageRoomRepository.GetSingleByConditionAsync(x =>
                    x.StorageRoomName.Equals(storageRoomInputRequest.StorageRoomName));

                if (existingStorageRoom != null)
                {
                    response.Success = false;
                    response.Message = "Tên kho đã tồn tại.";
                    return response;
                }

                // Tạo StorageRoomCode tự động (SRC001, SRC002, ...)
                var storageRoomCount = await _storageRoomRepository.CountAsync(x => true);
                var newCode = $"SRC{storageRoomCount + 1:D3}"; // Định dạng số thứ tự thành 3 chữ số (001, 002, ...)

                // Kiểm tra mã code có trùng không (đề phòng trường hợp bất thường)
                var existingCode = await _storageRoomRepository.GetSingleByConditionAsync(x => x.StorageRoomCode == newCode);
                if (existingCode != null)
                {
                    response.Success = false;
                    response.Message = "Mã kho đã tồn tại.";
                    return response;
                }

                var newStorageRoom = _mapper.Map<StorageRoom>(storageRoomInputRequest);
                newStorageRoom.StorageRoomCode = newCode; // Gán mã code tự động
                newStorageRoom.CreatedDate = DateTime.Now;
                newStorageRoom.RemainingRoomVolume = storageRoomInputRequest.Capacity;

                await _storageRoomRepository.InsertAsync(newStorageRoom);
                await _storageRoomRepository.SaveAsync();

                var dto = _mapper.Map<StorageRoomDTO>(newStorageRoom);
                dto.Type = ConvertTypeToString(newStorageRoom.Type);

                response.Message = "Tạo mới thành công";
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

        // Activate/Deactivate
        public async Task<Response<StorageRoomDTO>> ActivateDeactivateStorageRoom(int storageRoomId, bool update)
        {
            var response = new Response<StorageRoomDTO>();
            try
            {
                var storageRoom = await _storageRoomRepository.GetByIdAsync(storageRoomId);
                if (storageRoom == null)
                {
                    response.Success = false;
                    response.Data = null;
                    response.Message = "Không tìm thấy phòng chứa kho";
                    return response;
                }

                storageRoom.Status = update;
                await _storageRoomRepository.UpdateAsync(storageRoom);
                await _storageRoomRepository.SaveAsync();

                var dto = _mapper.Map<StorageRoomDTO>(storageRoom);
                dto.Type = ConvertTypeToString(storageRoom.Type);

                response.Success = true;
                response.Data = dto;
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

        // Update StorageRoom
        public async Task<Response<StorageRoomDTO>> UpdateStorageRoom(StorageRoomInputRequest storageRoomUpdateRequest)
        {
            var response = new Response<StorageRoomDTO>();

            try
            {
                var storageRoomToUpdate = await _storageRoomRepository.GetByIdAsync(storageRoomUpdateRequest.StorageRoomId);
                if (storageRoomToUpdate == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy nhà kho";
                    return response;
                }

                // Lưu giá trị Capacity hiện tại để tính toán sự thay đổi
                var oldCapacity = storageRoomToUpdate.Capacity;

                // Cập nhật các trường từ request
                if (!string.IsNullOrEmpty(storageRoomUpdateRequest.StorageRoomCode))
                    storageRoomToUpdate.StorageRoomCode = storageRoomUpdateRequest.StorageRoomCode;

                if (!string.IsNullOrEmpty(storageRoomUpdateRequest.StorageRoomName))
                    storageRoomToUpdate.StorageRoomName = storageRoomUpdateRequest.StorageRoomName;

                if (storageRoomUpdateRequest.Capacity.HasValue)
                {
                    // Tính sự thay đổi của Capacity
                    var newCapacity = storageRoomUpdateRequest.Capacity.Value;
                    var capacityChange = newCapacity - oldCapacity;

                    // Cập nhật Capacity
                    storageRoomToUpdate.Capacity = newCapacity;

                    // Điều chỉnh RemainingRoomVolume theo sự thay đổi của Capacity
                    storageRoomToUpdate.RemainingRoomVolume = storageRoomToUpdate.RemainingRoomVolume + capacityChange;
                }

                if (storageRoomUpdateRequest.Status.HasValue)
                    storageRoomToUpdate.Status = storageRoomUpdateRequest.Status;

                if (storageRoomUpdateRequest.CreatedBy.HasValue)
                    storageRoomToUpdate.CreatedBy = storageRoomUpdateRequest.CreatedBy;

                if (storageRoomUpdateRequest.CreatedDate.HasValue)
                    storageRoomToUpdate.CreatedDate = storageRoomUpdateRequest.CreatedDate;

                await _storageRoomRepository.UpdateAsync(storageRoomToUpdate);
                await _storageRoomRepository.SaveAsync();

                var dto = _mapper.Map<StorageRoomDTO>(storageRoomToUpdate);
                dto.Type = ConvertTypeToString(storageRoomToUpdate.Type);

                response.Success = true;
                response.Data = dto;
                response.Message = "Cập nhật nhà kho thành công";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi cập nhật nhà kho: {ex.Message}");
                Console.WriteLine($"🔍 Chi tiết lỗi: {ex.StackTrace}");

                response.Success = false;
                response.Message = $"Lỗi: {ex.Message}";
            }

            return response;
        }

        public async Task<Dictionary<int, string>> GetAllRoomTypes()
        {
            var result = Enum.GetValues(typeof(StorageRoomType))
                .Cast<StorageRoomType>()
                .ToDictionary(
                    key => (int)key,
                    value => value switch
                    {
                        StorageRoomType.Normal => "Phòng thường",
                        StorageRoomType.Cool => "Phòng mát",
                        StorageRoomType.Freezer => "Phòng đông lạnh",
                        _ => "Không xác định"
                    });

            return await Task.FromResult(result);
        }


    }
}