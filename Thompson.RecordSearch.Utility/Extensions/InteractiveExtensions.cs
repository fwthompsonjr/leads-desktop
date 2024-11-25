using System;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.Extensions
{
    public static class InteractiveExtensions
    {
        public static void CompleteProgess(
            this IWebInteractive web)
        {
            web.ReportProgessComplete?.Invoke();
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
            web.ReportProgress?.Invoke(min, max, current);
            if (!calcPercentage) return;
            if (!string.IsNullOrEmpty(percentageMessage)) Console.WriteLine(percentageMessage);
            var pct = Math.Round(Convert.ToDecimal(current) / Convert.ToDecimal(max), 2) * 100;
            Console.WriteLine($" Percent complete: {pct}");
        }
    }
}
