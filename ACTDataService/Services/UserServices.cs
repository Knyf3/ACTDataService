using ACTDataService.Helpers;
using ACTDataService.Models;
using System.Collections.ObjectModel;

namespace ACTDataService.Services
{
    public class UserService : IUserService
    {
        private readonly string _fileSettings;
        public SQLDataAccessHelper _sqlda;

        public UserService(string fileSettings)
        {
            _fileSettings = fileSettings;
            _sqlda = new SQLDataAccessHelper(_fileSettings);
        }

        //public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        //{
        //    return await _context.Users.ToListAsync();
        //}

        public async Task<ObservableCollection<UserFileModel>> GetAllUsersAsync()
        {
            
            var userList = await _sqlda.GetUserstoList();
            return userList;
        }
    }
}
