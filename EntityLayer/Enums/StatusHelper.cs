using System;

namespace EntityLayer.Enums
{
    public static class StatusHelper
    {
        // Customer ve User Status
        public const int Passive = 0;
        public const int Active = 1;

        // Order Status
        public const int Draft = 1;
        public const int Approved = 2;
        public const int Cancelled = 3;

        public static string GetStatusText(int status)
        {
            if (status == Passive) return "Pasif";
            if (status == Active) return "Aktif";
            if (status == Draft) return "Taslak";
            if (status == Approved) return "Onaylandı";
            if (status == Cancelled) return "İptal Edildi";
            return "Bilinmeyen";
        }

        public static string GetCustomerStatusText(int status)
        {
            if (status == Passive) return "Pasif";
            if (status == Active) return "Aktif";
            return "Bilinmeyen";
        }

        public static string GetOrderStatusText(int status)
        {
            if (status == Draft) return "Taslak";
            if (status == Approved) return "Onaylandı";
            if (status == Cancelled) return "İptal Edildi";
            return "Bilinmeyen";
        }

        public static string GetUserStatusText(int status)
        {
            if (status == Passive) return "Pasif";
            if (status == Active) return "Aktif";
            return "Bilinmeyen";
        }
    }
}