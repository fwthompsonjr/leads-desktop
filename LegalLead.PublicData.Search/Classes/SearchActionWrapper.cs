
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using Polly;
using Polly.Timeout;
using System;
using System.Diagnostics;
namespace LegalLead.PublicData.Search
{

    internal static class SearchActionWrapper
    {
        public static object Execute(ICountySearchAction action)
        {
            // Define the timeout policy
            var timeoutPolicy = Policy.Timeout(TimeSpan.FromSeconds(30), TimeoutStrategy.Pessimistic);

            // Define the retry policy to handle multiple exceptions
            var retryPolicy = Policy
                .Handle<Exception>() // This will handle any exception
                .Or<TimeoutRejectedException>() // This will handle timeout exceptions
                .WaitAndRetry(2, retryAttempt => TimeSpan.FromSeconds(3), (exception, timeSpan, retryCount, context) =>
                {
                    // Perform corrective action here using action.OrderId
                    PerformCorrectiveAction(action);
                });

            // Combine the policies
            var combinedPolicy = Policy.Wrap(retryPolicy, timeoutPolicy);

            try
            {
                return combinedPolicy.Execute(() =>
                {
                    // Execute the action
                    var result = action.Execute();
                    return result;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Process failed: {ex.Message}");
                throw;
            }
        }

        private static void PerformCorrectiveAction(ICountySearchAction search)
        {
            // Implement your corrective action logic here
            if (search is DallasNavigateSearch navigateSearch)
            {
                navigateSearch.RemediateSearch();
            }
        }
    }
}