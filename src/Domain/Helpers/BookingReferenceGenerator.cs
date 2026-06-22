// ToursApp.Domain/Helpers/BookingReferenceGenerator.cs
namespace ToursApp.Domain.Helpers
{
    public static class BookingReferenceGenerator
    {
        private static readonly Random _random = new();
        private static readonly object _lock = new();

        public static string Generate()
        {
            var now = DateTime.UtcNow;
            var year = now.Year;
            var month = now.Month.ToString("D2");
            var day = now.Day.ToString("D2");
            
            lock (_lock)
            {
                var randomPart = _random.Next(1000, 9999).ToString();
                return $"BKG-{year}{month}{day}-{randomPart}";
            }
        }

        // Alternative: Sequential with caching
        public static string GenerateSequential(int sequenceNumber)
        {
            var now = DateTime.UtcNow;
            var year = now.Year;
            var month = now.Month.ToString("D2");
            var day = now.Day.ToString("D2");
            
            return $"BKG-{year}{month}{day}-{sequenceNumber.ToString("D4")}";
        }
    }
}