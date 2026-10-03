using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;
using DefectListDomain.Services.NotificationDispatchesService;
using ReporterBusinessLogic.Services.UserService;

namespace DefectListBusinessLogic.Services.NotificationDispatchesService
{
    public class BomItemsBzrChangesDailyDigestHandler : INotificationHandler
    {
        private readonly IBomItemsBzrChangesDailyDigestBuilder _bomItemsChangesDailyDigestBuilder;
        private readonly IDefectListEmailService _emailService;
        private readonly IUserService _userService;

        public BomItemsBzrChangesDailyDigestHandler(
            IBomItemsBzrChangesDailyDigestBuilder bomItemsChangesDailyDigestBuilder,
            IDefectListEmailService emailService,
            IUserService userService)
        {
            _bomItemsChangesDailyDigestBuilder = bomItemsChangesDailyDigestBuilder;
            _emailService = emailService;
            _userService = userService;
        }

        public NotificationEventType EventType => NotificationEventType.BomItemsBzrChangesDailyDigest;

        public async Task SendAsync(int userId, DateTime digestDate)
        {
            var dbUser = _userService.GetUser(userId);
            if (dbUser == null || !dbUser.IsActive)
                return;

            var content = await _bomItemsChangesDailyDigestBuilder.ExecuteAsync(digestDate);

            if (string.IsNullOrEmpty(content))
                return;

            await _emailService.SendNotificationAsync(
                dbUser.Email,
                "Изменения по БЗР в ДВ",
                content,
                true,
                new List<string>());
        }
    }
}