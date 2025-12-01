using OpenQA.Selenium;

namespace SGP.Test.Helpers
{
    public static class SeleniumHelper
    {
        public static string TomarCapturaPantalla(IWebDriver driver, string nombreCaso, string carpetaBase)
        {
            string carpetaScreenshots = Path.Combine(carpetaBase, "Screenshots");
            if (!Directory.Exists(carpetaScreenshots))
                Directory.CreateDirectory(carpetaScreenshots);

            ITakesScreenshot ts = (ITakesScreenshot)driver;
            Screenshot screenshot = ts.GetScreenshot();

            string nombreArchivo = $"{nombreCaso}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string pathAbsoluto = Path.Combine(carpetaScreenshots, nombreArchivo);

            screenshot.SaveAsFile(pathAbsoluto);
            return Path.Combine("Screenshots", nombreArchivo);
        }
    }
}