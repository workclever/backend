using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Data;
using WorkCleverSolution.Dto.User;

namespace WorkCleverSolution.Services;

public interface IUserNotificationService
{
    Task CreateNotification(int byUserId, int toUserId, string type, string content, int taskId);
    Task<List<UserNotificationOutput>> ListUserNotifications(int userId, int limit);
    Task<int> GetUnreadNotificationsCount(int userId);
    Task SetNotificationsRead(int userId);
}

public class UserNotificationService : IUserNotificationService
{
    private readonly IRepository<UserNotification> _notificationRepository;

    public UserNotificationService(ApplicationDbContext dbContext)
    {
        _notificationRepository = new Repository<UserNotification>(dbContext);
    }

    public async Task CreateNotification(int byUserId, int toUserId, string type, string content, int taskId)
    {
        var userNotification = new UserNotification
        {
            ByUserId = byUserId,
            ToUserId = toUserId,
            TaskId = taskId,
            Type = type,
            Content = content,
            Unread = true
        };
        await _notificationRepository.Create(userNotification);
    }

    public async Task<List<UserNotificationOutput>> ListUserNotifications(int userId, int limit)
    {
        return await _notificationRepository.Where(r => r.ToUserId == userId)
            .Select(r => new UserNotificationOutput
            {
                Id = r.Id,
                ByUserId = r.ByUserId,
                DateCreated = r.DateCreated,
                Content = r.Content,
                Type = r.Type,
                TaskId = r.TaskId
            })
            .OrderByDescending(r => r.Id)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<int> GetUnreadNotificationsCount(int userId)
    {
        return await _notificationRepository
            .Where(r => r.ToUserId == userId && r.Unread)
            .CountAsync();
    }

    public async Task SetNotificationsRead(int userId)
    {
        var unreadNotifications = await _notificationRepository
            .Where(r => r.ToUserId == userId && r.Unread)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.Unread = false;
        }

        await _notificationRepository.UpdateRange(unreadNotifications);
    }
}