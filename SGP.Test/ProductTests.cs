using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SGP.Test.Helpers;
using Xunit;

namespace SGP.Test
{
    public class ProductTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly string _urlBase = "https://localhost:7184";

        private static ExtentReports _extent;
        private static ExtentTest _test;

        private static string _reportPath;

        public ProductTests()
        {
            if (_extent == null)
            {
                string pathProyecto = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

                _reportPath = Path.Combine(pathProyecto, "Reportes");

                if (!Directory.Exists(_reportPath)) Directory.CreateDirectory(_reportPath);

                var htmlReporter = new ExtentSparkReporter(Path.Combine(_reportPath, "Reporte_Final.html"));
                htmlReporter.Config.DocumentTitle = "Reporte SGP";
                htmlReporter.Config.ReportName = "Resultados de Pruebas Automatizadas";

                _extent = new ExtentReports();
                _extent.AttachReporter(htmlReporter);
            }

            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        private void RealizarLogin(string usuario, string password)
        {
            _driver.Navigate().GoToUrl($"{_urlBase}/Account/Login");
            _driver.FindElement(By.Id("inputUser")).SendKeys(usuario);
            _driver.FindElement(By.Id("inputPassword")).SendKeys(password);
            _driver.FindElement(By.Id("btnLogin")).Click();

            if (usuario == "admin")
            {
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.FindElements(By.TagName("table")).Count > 0 || d.Url.Contains("/Products"));
                Thread.Sleep(1000);
            }
        }

        private void LlenarCampo(string nombreCampo, string valor)
        {
            var campo = _driver.FindElement(By.Name(nombreCampo));
            campo.Clear();
            Thread.Sleep(200);
            campo.SendKeys(valor);
            Thread.Sleep(500);
        }

        [Fact]
        public void TC01_A_LoginFallido()
        {
            _test = _extent.CreateTest("TC-01 A: Login Fallido (Prueba Negativa)");
            try
            {
                _driver.Navigate().GoToUrl($"{_urlBase}/Account/Login");
                _driver.FindElement(By.Id("inputUser")).SendKeys("usuario_fake");
                _driver.FindElement(By.Id("inputPassword")).SendKeys("clave_fake");
                _driver.FindElement(By.Id("btnLogin")).Click();

                Thread.Sleep(1000);
                Assert.Contains("incorrectos", _driver.PageSource);

                _test.Pass("Bloqueo correcto.");
                _test.AddScreenCaptureFromPath(SeleniumHelper.TomarCapturaPantalla(_driver, "TC01_A_FalloEsperado", _reportPath));
            }
            catch (Exception ex)
            {
                _test.Fail(ex.Message);
                _test.AddScreenCaptureFromPath(SeleniumHelper.TomarCapturaPantalla(_driver, "TC01_A_Error", _reportPath));
                throw;
            }
        }

        [Fact]
        public void TC01_B_LoginExito()
        {
            _test = _extent.CreateTest("TC-01 B: Login Exitoso (Camino Feliz)");
            try
            {
                RealizarLogin("admin", "1234");
                Assert.True(_driver.FindElements(By.TagName("table")).Count > 0);

                _test.Pass("Login exitoso.");
                _test.AddScreenCaptureFromPath(SeleniumHelper.TomarCapturaPantalla(_driver, "TC01_B_Exito", _reportPath));
            }
            catch (Exception ex)
            {
                _test.Fail(ex.Message);
                _test.AddScreenCaptureFromPath(SeleniumHelper.TomarCapturaPantalla(_driver, "TC01_B_Error", _reportPath));
                throw;
            }
        }

        [Fact]
        public void TC02_CreacionDeProducto()
        {
            _test = _extent.CreateTest("TC-02: Creación de Producto");
            try
            {
                RealizarLogin("admin", "1234");
                _driver.Navigate().GoToUrl($"{_urlBase}/Products/Create");

                LlenarCampo("Nombre", "Laptop Selenium");
                LlenarCampo("Descripcion", "Creado con pausa");
                LlenarCampo("Precio", "1500");
                LlenarCampo("Stock", "50");

                _driver.FindElement(By.CssSelector("input[type='submit']")).Click();

                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.FindElements(By.TagName("table")).Count > 0);

                Assert.Contains("Laptop Selenium", _driver.PageSource);
                _test.Pass("Producto creado.");
                _test.AddScreenCaptureFromPath(SeleniumHelper.TomarCapturaPantalla(_driver, "TC02_Creado", _reportPath));
            }
            catch (Exception ex)
            {
                _test.Fail(ex.Message);
                _test.AddScreenCaptureFromPath(SeleniumHelper.TomarCapturaPantalla(_driver, "TC02_Error", _reportPath));
                throw;
            }
        }

        [Fact]
        public void TC03_ListadoDeProductos()
        {
            _test = _extent.CreateTest("TC-03: Listado");
            try
            {
                RealizarLogin("admin", "1234");
                Assert.True(_driver.FindElement(By.TagName("table")).Displayed);
                Assert.True(_driver.PageSource.Contains("Cerrar"));

                _test.Pass("Elementos visibles.");
                _test.AddScreenCaptureFromPath(SeleniumHelper.TomarCapturaPantalla(_driver, "TC03_Listado", _reportPath));
            }
            catch (Exception ex) { _test.Fail(ex.Message); throw; }
        }

        [Fact]
        public void TC04_EdicionDeProducto()
        {
            _test = _extent.CreateTest("TC-04: Edición");
            try
            {
                RealizarLogin("admin", "1234");

                string nombreUnico = "Edit_" + Guid.NewGuid().ToString().Substring(0, 4);
                _driver.Navigate().GoToUrl($"{_urlBase}/Products/Create");

                LlenarCampo("Nombre", nombreUnico);
                LlenarCampo("Descripcion", "Para Editar");
                LlenarCampo("Precio", "100");
                LlenarCampo("Stock", "10");
                _driver.FindElement(By.CssSelector("input[type='submit']")).Click();
                Thread.Sleep(2000);

                var btnEdit = _driver.FindElement(By.XPath($"//tr[contains(., '{nombreUnico}')]//a[contains(text(), 'Edit')]"));
                new Actions(_driver).MoveToElement(btnEdit).Perform();
                btnEdit.Click();

                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.FindElement(By.Name("Nombre")).Displayed);

                LlenarCampo("Precio", "5555");
                _driver.FindElement(By.CssSelector("input[type='submit']")).Click();
                Thread.Sleep(2000);

                string pageSource = _driver.PageSource;
                bool precioEncontrado = pageSource.Contains("5,555") || pageSource.Contains("5.555") || pageSource.Contains("5555");
                Assert.True(precioEncontrado);

                _test.Pass("Precio actualizado.");
                _test.AddScreenCaptureFromPath(SeleniumHelper.TomarCapturaPantalla(_driver, "TC04_Editado", _reportPath));
            }
            catch (Exception ex)
            {
                _test.Fail(ex.Message);
                _test.AddScreenCaptureFromPath(SeleniumHelper.TomarCapturaPantalla(_driver, "TC04_Error", _reportPath));
                throw;
            }
        }

        [Fact]
        public void TC05_EliminacionDeProducto()
        {
            _test = _extent.CreateTest("TC-05: Eliminación");
            try
            {
                RealizarLogin("admin", "1234");

                string nombreBorrar = "Del_" + Guid.NewGuid().ToString().Substring(0, 4);
                _driver.Navigate().GoToUrl($"{_urlBase}/Products/Create");

                LlenarCampo("Nombre", nombreBorrar);
                LlenarCampo("Descripcion", "Para Borrar");
                LlenarCampo("Precio", "10");
                LlenarCampo("Stock", "5");
                _driver.FindElement(By.CssSelector("input[type='submit']")).Click();
                Thread.Sleep(2000);

                var btnDelete = _driver.FindElement(By.XPath($"//tr[contains(., '{nombreBorrar}')]//a[contains(text(), 'Delete')]"));
                new Actions(_driver).MoveToElement(btnDelete).Perform();
                btnDelete.Click();

                Thread.Sleep(1000);
                _driver.FindElement(By.CssSelector("input[type='submit']")).Click();
                Thread.Sleep(2000);

                Assert.DoesNotContain(nombreBorrar, _driver.PageSource);

                _test.Pass("Eliminado OK.");
                _test.AddScreenCaptureFromPath(SeleniumHelper.TomarCapturaPantalla(_driver, "TC05_Eliminado", _reportPath));
            }
            catch (Exception ex)
            {
                _test.Fail(ex.Message);
                _test.AddScreenCaptureFromPath(SeleniumHelper.TomarCapturaPantalla(_driver, "TC05_Error", _reportPath));
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                _driver.Quit();
                _driver.Dispose();
                _extent.Flush();
            }
            catch { }
        }
    }
}