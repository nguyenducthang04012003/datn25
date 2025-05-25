using Microsoft.EntityFrameworkCore;
using PharmaDistiPro.Common.Enums;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(SEP490_G74Context context) : base(context)
        {
        }

        public async Task<User?> GetWarehouseManagerToConfirm()
        {
            // Lấy warehouse chưa có order nào, ưu tiên warehouse có UserId nhỏ nhất
            var warehouseWithoutOrders = await _context.Users
                .Where(x => x.RoleId == 2 && !x.OrderAssignToNavigations.Any() && x.Status==true) // Warehouse chưa có order nào
                .OrderBy(x => x.UserId) 
                .FirstOrDefaultAsync();

            if (warehouseWithoutOrders != null)
            {
                return warehouseWithoutOrders; 
            }

            
            var warehouseWithMinOrders = await _context.Users
                .Where(x => x.RoleId == 2 && x.Status == true)
              
                .OrderBy(x => x.OrderAssignToNavigations.Count) 
                .FirstOrDefaultAsync();

            return warehouseWithMinOrders;
        }
        public async Task<User> GetUser(string username)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserName==username );
        }


        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);

        }
        public async Task<User> UpdateUser(User user)
        {

            _context.Users.Update(user);
            int rowAffected = await _context.SaveChangesAsync();
            return user;
        }
        public async Task<List<User>> GetUsersByRoleIdAsync(int roleId)
        {
            return await _context.Users
                                 .Where(u => u.RoleId == roleId)
                                 .ToListAsync();
        }
    }
}
