using ACTDataService.Helpers;
using ACTDataService.Models;
using System.Collections.ObjectModel;

namespace ACTDataService.Services
{
    public interface IUserService
    {
        //Task<IEnumerable<Models.UserModel>> GetAllUsersAsync();
        Task<ObservableCollection<UserFileModel>> GetAllUsersAsync();
        Task<ObservableCollection<EventLogModel>> GetEventLog(string startDate, string finishDate);
        Task<ObservableCollection<UserModel>> GetUsersWithGroup();
    }
}