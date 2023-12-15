using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace Yabi.Models
{
    public class Mayer
    {
        // Simulations determined that in the past, the best long-term result were achieved by accumulating Bitcoin whenever the Mayer Multiple was below 2.4.
        const double TOP = 2.4;
        public static async Task<string> GetData()
        {
            var url = "https://buybitcoinworldwide.com/mayer-multiple/";

            // Set up the browser
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            var driver = new ChromeDriver(options);

            var mayer = "";

            try
            {

                driver.Navigate().GoToUrl(url);

                // Wait until the information is loaded, damn SPA's
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(3));
                var element = wait.Until(d => d.FindElement(By.Id("mayer-multiple")));
                wait.Until(d => element.Text != "Loading...");

                mayer = element.Text;
            }
            finally
            {
                driver.Quit();
            }

            return mayer;
        }

        // Largest score is better to buy
        public static async Task<int> GetScore()
        {
            double Score = 0;
            var MayerIndex = await Mayer.GetData();
            var IsSuccess = double.TryParse(MayerIndex, out double result);

            if (IsSuccess) {

                Score = result * 100 / TOP - 100;

                return Math.Abs((int)Score);
            }

            return 0;

        }
    }
}
