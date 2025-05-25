using AutoMapper;
using CloudinaryDotNet;
using PharmaDistiPro.DTO.StorageRooms;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Interface;
using PharmaDistiPro.Services.Interface;
using Microsoft.Extensions.Logging;
using System;
using PharmaDistiPro.Helper.Enums;
using PharmaDistiPro.Repositories.Impl;
using System.Net.Mail;
using Microsoft.Extensions.Caching.Memory;

namespace PharmaDistiPro.Services.Impl
{
    public class StorageHistoryService : IStorageHistoryService
    {
        private readonly IStorageHistoryRepository _storageHistoryRepository;
        private readonly IStorageRoomRepository _storageRoomRepository;
        private readonly IUserRepository _userRepository; // Đã khai báo nhưng chưa khởi tạo
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<StorageHistoryService> _logger;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _memoryCache; // Tạo một instance của IMemoryCache
        public StorageHistoryService(
            IStorageRoomRepository storageRoomRepository,
            IStorageHistoryRepository storageHistoryRepository,
            IUserRepository userRepository, // Thêm vào constructor
            Cloudinary cloudinary,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<StorageHistoryService> logger,
            IEmailService emailService, IMemoryCache memoryCache)
        {
            _storageRoomRepository = storageRoomRepository ?? throw new ArgumentNullException(nameof(storageRoomRepository));
            _storageHistoryRepository = storageHistoryRepository ?? throw new ArgumentNullException(nameof(storageHistoryRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository)); // Khởi tạo
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService)); // Đảm bảo không null
            _memoryCache = memoryCache;
        }
        public async Task<StorageHistoryDTO> CreateStorageHistoryAsync(StorageHistoryInputRequest request)
        {
            _logger.LogInformation("Starting CreateStorageHistoryAsync with request: {@Request}", request);

            if (request == null || !request.Temperature.HasValue || !request.Humidity.HasValue)
            {
                throw new ArgumentException("Missing required fields: Temperature or Humidity.");
            }

            if (request.Temperature < -50 || request.Temperature > 100)
            {
                throw new ArgumentException("Temperature must be between -50 and 100 degrees.");
            }

            if (request.Humidity < 0 || request.Humidity > 100)
            {
                throw new ArgumentException("Humidity must be between 0 and 100 percent.");
            }

            var storageRoom = await _storageRoomRepository.GetByIdAsync(request.StorageRoomId);
            if (storageRoom == null)
            {
                throw new ArgumentException($"StorageRoom with ID {request.StorageRoomId} does not exist.");
            }

            // Xác định múi giờ Việt Nam
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            try
            {
                var history = new StorageHistory
                {
                    StorageRoomId = request.StorageRoomId,
                    Temperature = request.Temperature.Value,
                    Humidity = request.Humidity.Value,
                    CreatedDate = request.CreatedDate ?? vietnamTime,
                    Service = request.Service?.Trim() ?? "Unknown"
                };

                await _storageHistoryRepository.InsertAsync(history);
                await _storageHistoryRepository.SaveAsync();

                var result = _mapper.Map<StorageHistoryDTO>(history);

                
                if (storageRoom.Type.HasValue)
                {
                    var (exceedsLimit, alertMessage, detail) = CheckStorageThreshold(
                        (StorageRoomType)storageRoom.Type.Value,
                        request.Temperature.Value,
                        request.Humidity.Value
                    );

                    if (exceedsLimit)
                    {
                        await SendAlertEmailsAsync(storageRoom.StorageRoomName, alertMessage, detail);
                        result.AlertMessage = alertMessage;
                        result.AlertDetail = detail;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating StorageHistory for StorageRoomId {StorageRoomId}", request.StorageRoomId);
                throw new Exception($"Error creating StorageHistory: {ex.Message}", ex);
            }
        }

        private (bool exceedsLimit, string alertMessage, string detail) CheckStorageThreshold(StorageRoomType type, double temp, double humidity)
        {
            bool exceedsLimit = false;
            string alertMessage = "";
            string detail = "";

            switch (type)
            {
                case StorageRoomType.Normal:
                    
                    if ((temp < 15 || temp > 30) && humidity >= 75)
                    {
                        exceedsLimit = true;
                        alertMessage = "Cả nhiệt độ và độ ẩm của Phòng thường đều vượt ngưỡng (Nhiệt độ: 15-30°C; Độ ẩm < 75%)";
                        detail += $"- Nhiệt độ ({temp}°C) vượt ngưỡng cho phép (15-30°C)\n";
                        detail += $"- Độ ẩm ({humidity}%) vượt ngưỡng cho phép (< 75%)\n";
                    }
                   
                    else
                    {
                        if (temp < 15 || temp > 30)
                        {
                            exceedsLimit = true;
                            alertMessage = "Nhiệt độ của Phòng thường vượt ngưỡng (Nhiệt độ: 15-30°C)";
                            detail += $"- Nhiệt độ ({temp}°C) vượt ngưỡng cho phép (15-30°C)\n";
                        }

                        if (humidity >= 75)
                        {
                            exceedsLimit = true;
                            alertMessage = "Độ ẩm của Phòng thường vượt ngưỡng (Độ ẩm < 75%)";
                            detail += $"- Độ ẩm ({humidity}%) vượt ngưỡng cho phép (< 75%)\n";
                        }
                    }
                    break;

                case StorageRoomType.Cool:
                    
                    if ((temp < 8 || temp > 15) && humidity >= 70)
                    {
                        exceedsLimit = true;
                        alertMessage = "Cả nhiệt độ và độ ẩm của Phòng mát đều vượt ngưỡng (Nhiệt độ: 8-15°C; Độ ẩm < 70%)";
                        detail += $"- Nhiệt độ ({temp}°C) vượt ngưỡng cho phép (8-15°C)\n";
                        detail += $"- Độ ẩm ({humidity}%) vượt ngưỡng cho phép (< 70%)\n";
                    }
                    // Kiểm tra riêng biệt từng điều kiện nếu không cùng vượt ngưỡng
                    else
                    {
                        if (temp < 8 || temp > 15)
                        {
                            exceedsLimit = true;
                            alertMessage = "Nhiệt độ của Phòng mát vượt ngưỡng (Nhiệt độ: 8-15°C)";
                            detail += $"- Nhiệt độ ({temp}°C) vượt ngưỡng cho phép (8-15°C)\n";
                        }

                        if (humidity >= 70)
                        {
                            exceedsLimit = true;
                            alertMessage = "Độ ẩm của Phòng mát vượt ngưỡng (Độ ẩm < 70%)";
                            detail += $"- Độ ẩm ({humidity}%) vượt ngưỡng cho phép (< 70%)\n";
                        }
                    }
                    break;

                case StorageRoomType.Freezer:
                    // Kiểm tra cả nhiệt độ và độ ẩm vượt ngưỡng
                    if ((temp < 2 || temp > 8) && humidity >= 45)
                    {
                        exceedsLimit = true;
                        alertMessage = "Cả nhiệt độ và độ ẩm của Phòng đông lạnh đều vượt ngưỡng (Nhiệt độ: 2-8°C; Độ ẩm < 45%)";
                        detail += $"- Nhiệt độ ({temp}°C) vượt ngưỡng cho phép (2-8°C)\n";
                        detail += $"- Độ ẩm ({humidity}%) vượt ngưỡng cho phép (< 45%)\n";
                    }
                    // Kiểm tra riêng biệt từng điều kiện nếu không cùng vượt ngưỡng
                    else
                    {
                        if (temp < 2 || temp > 8)
                        {
                            exceedsLimit = true;
                            alertMessage = "Nhiệt độ của Phòng đông lạnh vượt ngưỡng (Nhiệt độ: 2-8°C)";
                            detail += $"- Nhiệt độ ({temp}°C) vượt ngưỡng cho phép (2-8°C)\n";
                        }

                        if (humidity >= 45)
                        {
                            exceedsLimit = true;
                            alertMessage = "Độ ẩm của Phòng đông lạnh vượt ngưỡng (Độ ẩm < 45%)";
                            detail += $"- Độ ẩm ({humidity}%) vượt ngưỡng cho phép (< 45%)\n";
                        }
                    }
                    break;
            }

            return (exceedsLimit, alertMessage, detail);
        }



        private async Task SendAlertEmailsAsync(string roomName, string alertMessage, string detail)
        {
            var users = await _userRepository.GetUsersByRoleIdAsync(2);
            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            var cacheKey = $"LastAlertSent_{roomName}";

            if (_memoryCache.TryGetValue(cacheKey, out DateTime lastAlertSent))
            {
                // Nếu đã gửi email trong vòng 2 phút theo giờ Việt Nam thì bỏ qua
                if ((vietnamTime- lastAlertSent).TotalMinutes < 2)
                {
                    _logger.LogInformation("An alert was sent recently for room {RoomName}, skipping this alert.", roomName);
                    return; // Không gửi email nếu đã gửi trong vòng 5 phút
                }
            }

            foreach (var user in users)
            {
                string emailBody = $@"
Xin chào {user.FirstName},

{alertMessage}

Thông tin chi tiết:
Phòng: {roomName}
Ngày ghi nhận: {vietnamTime:dd/MM/yyyy HH:mm:ss} (GMT+7)

{detail}

Vui lòng kiểm tra và xử lý kịp thời để đảm bảo an toàn hàng hóa.
Trân trọng,
PharmaDistiPro
";

                await _emailService.SendEmailAsync(user.Email, "Cảnh báo môi trường kho", emailBody);
            }
            _memoryCache.Set(cacheKey, vietnamTime, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) // Đặt thời gian hết hạn cache là 2 phút
            });
        }



        public async Task<List<StorageHistoryChartDTO>> GetTop50EarliestForChartAsync(int storageRoomId)
        {
            try
            {
                // Kiểm tra StorageRoom
                var storageRoom = await _storageRoomRepository.GetByIdAsync(storageRoomId);
                if (storageRoom == null)
                {
                    throw new ArgumentException($"StorageRoom with ID {storageRoomId} does not exist.");
                }

                // Lấy top 50 bản ghi sớm nhất
                var histories = await _storageHistoryRepository.GetAllAsync();
                var result = histories
                    .Where(h => h.StorageRoomId == storageRoomId)
                   .OrderByDescending(h => h.Id)
                    .Take(50)
                    .Select(h => new StorageHistoryChartDTO
                    {
                        Temperature = h.Temperature,
                        Humidity = h.Humidity,
                        CreatedDate = h.CreatedDate
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching chart data for StorageRoomId {0}", storageRoomId);
                throw new Exception($"Error fetching chart data: {ex.Message}", ex);
            }
        }


        // get sensor data newest 
        public async Task<StorageHistoryChartDTO> GetLatestByStorageRoomIdAsync(int storageRoomId)
        {
            try
            {
                // Kiểm tra StorageRoom
                var storageRoom = await _storageRoomRepository.GetByIdAsync(storageRoomId);
                if (storageRoom == null)
                {
                    throw new ArgumentException($"StorageRoom with ID {storageRoomId} does not exist.");
                }

                // Lấy bản ghi mới nhất
                var histories = await _storageHistoryRepository.GetAllAsync();
                var result = histories
                    .Where(h => h.StorageRoomId == storageRoomId)
                    .OrderByDescending(h => h.CreatedDate)
                    .Select(h => new StorageHistoryChartDTO
                    {
                        Temperature = h.Temperature,
                        Humidity = h.Humidity,
                        CreatedDate = h.CreatedDate
                    })
                    .FirstOrDefault();

                if (result == null)
                {
                    throw new InvalidOperationException($"No history found for StorageRoom with ID {storageRoomId}.");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching latest data for StorageRoomId {storageRoomId}: {ex.Message}", ex);
            }


        }
        public async Task<bool> HasSensorAsync(int storageRoomId)
        {
            try
            {
                var histories = await _storageHistoryRepository.GetAllAsync();
                return histories.Any(h => h.StorageRoomId == storageRoomId);
            }
            catch
            {
                return false; // Nếu có lỗi, trả về false
            }
        }
    }
}