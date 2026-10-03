using System;
using System.Threading.Tasks;
using ReporterBusinessLogic.Services.UserService;
using System.Collections.Generic;
using DefectListDomain.Models;
using DefectListDomain.Services.NotificationDispatchesService;

namespace DefectListBusinessLogic.Services.NotificationDispatchesService
{
    public class UserBomHeaderSubscriptionDigestHandler : INotificationHandler
    {
        private readonly IUserBomHeaderSubscriptionDigestBuilder _userBomHeaderSubscriptionDigestBuilder;
        private readonly IDefectListEmailService _emailService;
        private readonly IUserService _userService;

        public UserBomHeaderSubscriptionDigestHandler(
            IUserBomHeaderSubscriptionDigestBuilder userBomHeaderSubscriptionDigestBuilder,
            IDefectListEmailService emailService,
            IUserService userService)
        {
            _userBomHeaderSubscriptionDigestBuilder = userBomHeaderSubscriptionDigestBuilder;
            _emailService = emailService;
            _userService = userService;
        }

        public NotificationEventType EventType => NotificationEventType.UserBomHeaderSubscriptionDailyDigest;

        public async Task SendAsync(int userId, DateTime digestDate)
        {
            var dbUser = _userService.GetUser(userId);
            if (dbUser == null || !dbUser.IsActive)
                return;

            var content = await _userBomHeaderSubscriptionDigestBuilder.ExecuteAsync(userId, digestDate);

            if (string.IsNullOrEmpty(content))
                return;

            await _emailService.SendNotificationAsync(
                dbUser.Email,
                "Уведомление об изменениях в ДВ",
                content,
                true,
                new List<string>());
        }
    }
}