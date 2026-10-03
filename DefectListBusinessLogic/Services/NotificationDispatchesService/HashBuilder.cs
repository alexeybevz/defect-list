using System;
using System.Security.Cryptography;
using System.Text;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Services.NotificationDispatchesService
{
    public static class HashBuilder
    {
        public static string BuildHash(int userId, NotificationEventType notificationEventType, DateTime digestDate)
        {
            var raw = $"{notificationEventType}:{userId}:{digestDate:yyyyMMdd}";
            var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));

            return ToHexString(hash);
        }

        private static string ToHexString(byte[] bytes)
        {
            if (bytes == null)
                return string.Empty;

            var sb = new StringBuilder(bytes.Length * 2);

            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}