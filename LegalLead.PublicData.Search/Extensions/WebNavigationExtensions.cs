using OpenQA.Selenium;
using Polly;
using Polly.Retry;
using System;
using System.Diagnostics;

namespace LegalLead.PublicData.Search.Extensions
{
    internal static class WebNavigationExtensions
    {
        /// <summary>
        /// Executes JavaScript with a custom timeout and retry policy.
        /// </summary>
        /// <param name="driver">The WebDriver instance.</param>
        /// <param name="executor">The JavaScript executor instance.</param>
        /// <param name="timeout">The custom timeout for the HTTP client.</param>
        /// <param name="script">The JavaScript to execute.</param>
        /// <returns>The result of the JavaScript execution.</returns>
        /// <exception cref="WebDriverException">Thrown when the execution fails after retries.</exception>
        /// <remarks>
        /// This method sets a custom HTTP client timeout and retries the JavaScript execution up to 5 times
        /// with exponential backoff intervals (1, 2, 4, 8, and 16 seconds) in case of exceptions.
        /// The original page-load timeout is restored after the method completes.
        /// </remarks>
        public static object ExecuteScriptWithRetry(this IWebDriver driver, IJavaScriptExecutor executor, TimeSpan timeout, string script)
        {
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetry(5, retryAttempt =>
                {
                    var ms = 500 * Math.Pow(2, retryAttempt);
                    return TimeSpan.FromMilliseconds(ms);
                });

            object result = null;

            var originalTimeout = driver.Manage().Timeouts().PageLoad;

            try
            {
                var currentUrl = driver.Url;
                Debug.WriteLine(currentUrl);
                driver.Manage().Timeouts().PageLoad = timeout;
                policy.Execute(() =>
                {
                    result = executor.ExecuteScript(script);
                });
            }
            finally
            {
                // Restore the original page-load timeout
                driver.Manage().Timeouts().PageLoad = originalTimeout;
            }

            return result;
        }

        /// <summary>
        /// Navigates to destination URI with a custom timeout and retry policy.
        /// </summary>
        /// <param name="driver">The WebDriver instance.</param>
        /// <param name="timeout">The custom timeout for the HTTP client.</param>
        /// <param name="script">The JavaScript to execute.</param>
        /// <remarks>
        /// This method sets a custom HTTP client timeout and retries the JavaScript execution up to 5 times
        /// with exponential backoff intervals (1, 2, 4 seconds) in case of exceptions.
        /// The original page-load timeout is restored after the method completes.
        /// </remarks>
        public static void NavigateWithRetry(this IWebDriver driver, TimeSpan timeout, Uri uri, Uri fallbackUri = null)
        {
            var policy = Policy
                .Handle<WebDriverTimeoutException>()
                .WaitAndRetry(3, retryAttempt =>
                {
                    var ms = 750 * Math.Pow(2, retryAttempt);
                    return TimeSpan.FromMilliseconds(ms);
                }, (exception, timeSpan, retryCount, context) =>
                {
                    if (fallbackUri != null)
                    {
                        driver.Navigate().GoToUrl(fallbackUri);
                        // Wait for a short period to ensure the fallback page loads
                        System.Threading.Thread.Sleep(150);
                    }
                });

            var originalTimeout = driver.Manage().Timeouts().PageLoad;

            try
            {
                driver.Manage().Timeouts().PageLoad = timeout;
                policy.Execute(() =>
                {
                    driver.Navigate().GoToUrl(uri);
                });
            }
            finally
            {
                ResetTimeout(driver, originalTimeout);
            }
        }

        private static void ResetTimeout(IWebDriver driver, TimeSpan originalTimeout)
        {
            try
            {
                // Restore the original page-load timeout
                driver.Manage().Timeouts().PageLoad = originalTimeout;
            }
            catch (Exception)
            {
                // no action taken
            }
        }

        public static void ReloadCurrentPageWithRetry(this IWebDriver driver)
        {
            // Define a retry policy with Polly
            RetryPolicy retryPolicy = Policy
                .Handle<WebDriverException>()
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        // Console.WriteLine($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                    });

            retryPolicy.Execute(() =>
            {
                try
                {
                    // Get the current URL
                    string currentUrl = driver.Url;

                    // Navigate to a blank page to clear any ongoing requests
                    driver.Navigate().GoToUrl("about:blank");

                    // Reload the original page
                    driver.Navigate().GoToUrl(currentUrl);

                    // Verify the page is loaded (you can add more checks as needed)
                    if (driver.Url != currentUrl)
                    {
                        throw new WebDriverException("Failed to load the requested page.");
                    }
                }
                catch (Exception)
                {
                    // suppress any unexpected errors here.
                }
            });
        }

    }
}
