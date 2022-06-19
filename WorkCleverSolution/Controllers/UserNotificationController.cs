using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Attributes;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/UserNotification/[action]")]
public class UserNotificationController : BaseApiController
{
    public UserNotificationController(IServices services) : base(services)
    {
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> ListUserNotifications(int limit)
    {
        return Wrap(await Services.UserNotificationService().ListUserNotifications(User.GetUserId(), limit));
    }

    [HttpGet]
    [JwtAuthorize]
    public async Task<ServiceResult> GetUnreadNotificationsCount()
    {
        return Wrap(await Services.UserNotificationService().GetUnreadNotificationsCount(User.GetUserId()));
    }

    [HttpPost]
    [JwtAuthorize]
    public async Task<ServiceResult> SetNotificationsRead()
    {
        await Services.UserNotificationService().SetNotificationsRead(User.GetUserId());
        return Wrap();
    }
}