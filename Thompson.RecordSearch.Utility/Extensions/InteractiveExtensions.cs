using System;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.Extensions
{
    public static class InteractiveExtensions
    {
        public static void CompleteProgess(
            this IWebInteractive web)
        {
            web?.ReportProgessComplete?.Invoke();
        }

        public static void EchoProgess(
            this IWebInteractive web,
            int min,
            int max,
            int current,
            string message = "",
            bool calcPercentage = true,
            string percentageMessage = "")
        {
            if (string.IsNullOrEmpty(message)) message = $"Item {current} of {max}";
            Console.WriteLine(message);
            web?.ReportProgress?.Invoke(min, max, current);
            if (!calcPercentage) return;
            if (!string.IsNullOrEmpty(percentageMessage)) Console.WriteLine(percentageMessage);
            var interval = GetProgressInterval(max);
            if (current % interval != 0) return;
            var pct = Math.Round(Convert.ToDecimal(current) / Convert.ToDecimal(max), 2) * 100;
            Console.WriteLine($" Percent complete: {pct}");
        }

        private static int GetProgressInterval(int max)
        {
            const int mininterval = 10;
            var pct = Convert.ToInt32(Math.Round(Convert.ToDecimal(max) * 0.1m));
            if (pct < mininterval) return mininterval;
            return Math.Max(pct % 2, mininterval);
        }
    }
}
