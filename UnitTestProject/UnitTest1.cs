using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        ChromeOptions options = new ChromeOptions();
        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        IWebDriver Webdriver = null;
        IWebElement element = null;
        
        [TestMethod]
        public void Start()
        {
            service.HideCommandPromptWindow = true;
            Webdriver = new ChromeDriver(service, options);
            Webdriver.Manage().Window.Maximize();
            Webdriver.Navigate().GoToUrl("https://akamai-s1-astra-stage.arcelik.com.tr/");
            cookie();
        }

        [TestMethod]
        public void Login()
        {
            Start();
            WebDriverWait wait = new WebDriverWait(Webdriver, TimeSpan.FromSeconds(50));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("a[title='Üyelik']")));
            Webdriver.FindElement(By.CssSelector("a[title='Üyelik']")).Click();
            cookie();
            Webdriver.FindElement(By.Id("j_username")).SendKeys("test.arcelik.furkan@outlook.com");
            Webdriver.FindElement(By.Id("j_password")).SendKeys("12345furkan");
            Webdriver.FindElement(By.Id("form-login-btn")).Click();
            cookie();
        }

        [TestMethod]
        public void Single_Card()
        { 
            Login();
            WebDriverWait wait = new WebDriverWait(Webdriver, TimeSpan.FromSeconds(50));
            Webdriver.FindElement(By.CssSelector("a[class='btn-cart']")).Click();
            cookie();
            try
            {
                //Sepet Doluysa
                Webdriver.FindElement(By.CssSelector("button[class='link-inline js-remove-cart-entries']")).Click();
                cookie();
                Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
                cookie();
            }
            catch (Exception)
            {
                //Sepet Boşsa
                Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
                cookie();
            }

            //Search and PLP
            Webdriver.FindElement(By.CssSelector("svg[class='icon icon-search']")).Click();
            Webdriver.FindElement(By.Id("searchText")).SendKeys("kahve");
            System.Threading.Thread.Sleep(2000);
            element = Webdriver.FindElement(By.Id("searchText"));
            element.SendKeys(OpenQA.Selenium.Keys.Return);
            cookie();

            //Go PDP
            System.Threading.Thread.Sleep(2000);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("a[href='/turk-kahve-makinesi/k-3300-mini-telve-beyaz-icecek-hazirlama']")));
            Webdriver.FindElement(By.CssSelector("a[href='/turk-kahve-makinesi/k-3300-mini-telve-beyaz-icecek-hazirlama']")).Click();
            cookie();

            //Hangi Mağazada Var
            IJavaScriptExecutor jse = (IJavaScriptExecutor)Webdriver;
            jse.ExecuteScript("scroll(0,1000);");
            System.Threading.Thread.Sleep(1000);
            //Ürün özellikleri kapat
            Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[0].Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[3].Click();
            Webdriver.FindElement(By.Id("cityCode")).Click();
            Webdriver.FindElement(By.CssSelector("option[value='34']")).Click();
            Webdriver.FindElement(By.Id("townCode")).Click();
            Webdriver.FindElement(By.CssSelector("option[value='34.34']")).Click();
            Webdriver.FindElement(By.CssSelector("button[title='Mağaza bul']")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[3].Click();
            //Taksit Seçenekleri
            //jse.ExecuteScript("scroll(0,4500)");
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[4].Click();
            System.Threading.Thread.Sleep(2000);
            int installment = Webdriver.FindElement(By.CssSelector("div[class='pdp-installments-list']")).FindElements(By.CssSelector("div[class='item']")).Count();
            if (installment > 0)
            {
                System.Threading.Thread.Sleep(2000);
                Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[4].Click();

                //Sepete At
                jse.ExecuteScript("scroll(6000,0)");
                System.Threading.Thread.Sleep(2000);
                Webdriver.FindElement(By.CssSelector("button[title='Sepete At']")).Click();
                System.Threading.Thread.Sleep(5000);
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div[class='ntf-msg']")));
                string hata = Webdriver.FindElement(By.CssSelector("div[class='p']")).Text.Trim().ToString();
                string basarili_islem = Webdriver.FindElement(By.CssSelector("div[class='ntf-msg']")).Text.Trim().ToString();
                if (hata == "Sepette bu üründen alabileceğiniz azami adete ulaştınız.")
                {
                    Webdriver.FindElement(By.CssSelector("button[class='btn btn-outline-dark js-popup-trigger-func']")).Click();
                }
                else if (basarili_islem == "Ürün sepetinize eklendi")
                {
                    Webdriver.FindElement(By.CssSelector("a[title='Sepete Git']")).Click();
                    cookie();
                    //Adet arttır
                    Webdriver.FindElement(By.CssSelector("button[class='btn-plus ']")).Click();
                    cookie();
                    //Adet Azalt
                    Webdriver.FindElement(By.CssSelector("button[class='btn-minus']")).Click();
                    cookie();
                    //Favori Ekle
                    Webdriver.FindElement(By.Id("g-recaptcha-btn-favourite")).Click();
                    System.Threading.Thread.Sleep(2000);
                    Webdriver.Navigate().Refresh();
                    cookie();
                    System.Threading.Thread.Sleep(2000);
                    //Sepeti Onayla
                    Webdriver.FindElement(By.CssSelector("a[href='/checkout/add-address']")).Click();
                    cookie();
                    string BasketLastPrice = Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div[2]/div/div/div/div[2]/div[1]/div/div[3]/div[1]/div/span")).Text.TrimStart().TrimEnd('L').TrimEnd('T').TrimEnd().ToString();
                    int BasketLastPriceINT = Convert.ToInt32(BasketLastPrice);
                    //CART
                    Webdriver.FindElement(By.Id("pay-payment-card")).Click();
                    Webdriver.FindElement(By.Id("mcard_step_1_cardno")).SendKeys("4022774022774026");
                    Webdriver.FindElement(By.Id("mcard_step_1_name")).SendKeys("Arcelik");
                    Webdriver.FindElement(By.Id("mcard_step_1_expiry")).SendKeys("1230");
                    Webdriver.FindElement(By.Id("mcard_step_1_cvv")).SendKeys("000");
                    //Onay
                    Webdriver.FindElement(By.Id("chk_cart_sum_confirm_1")).Click();
                    Webdriver.FindElement(By.Id("chk_cart_sum_confirm_2")).Click();
                    //Tamamla
                    Webdriver.FindElement(By.Id("postPayment")).Click();
                    //Güvenlik Kodu
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("input[class='value']")));
                    Webdriver.FindElement(By.CssSelector("input[class='value']")).SendKeys("a");
                    System.Threading.Thread.Sleep(1000);
                    Webdriver.FindElement(By.Id("submitbutton")).Click();
                    //Bitiş
                    cookie();
                    System.Threading.Thread.Sleep(1000);
                    string order = Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div/div/div[2]/div[1]/div[2]")).Text.TrimStart().TrimStart('S').TrimStart('i').TrimStart('p').TrimStart('a').TrimStart('r').TrimStart('i').TrimStart('ş').TrimStart().TrimStart('n').TrimStart('u').TrimStart('m').TrimStart('a').TrimStart('r').TrimStart('a').TrimStart('s').TrimStart('ı').TrimStart(':').TrimStart().ToString();
                    System.Threading.Thread.Sleep(1000);
                    //Hybris_Control
                    ((IJavaScriptExecutor)Webdriver).ExecuteScript("window.open();");
                    Webdriver.SwitchTo().Window(Webdriver.WindowHandles.Last());
                    Webdriver.Navigate().GoToUrl("https://backoffice.c1m0wu3z2z-arcelikas1-s1-public.model-t.cc.commerce.ondemand.com/backoffice/login.zul");
                    Webdriver.FindElement(By.CssSelector("input[name='j_username']")).SendKeys("muhammedfurkan_gunduz@arcelik.com");
                    Webdriver.FindElement(By.CssSelector("input[name='j_password']")).SendKeys("123");
                    Webdriver.FindElements(By.CssSelector("option[class='z-option']"))[2].Click();
                    Webdriver.FindElement(By.CssSelector("button[class='login_btn y-btn-primary z-button']")).Click();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[2]/div/div[2]/div/div/button")));
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[2]/div/div[2]/div/div/button")).Click();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("input[class='yw-explorerTree-filterTextbox yw-filter-textbox y-general-textinput z-textbox']")));
                    Webdriver.FindElement(By.CssSelector("input[class='yw-explorerTree-filterTextbox yw-filter-textbox y-general-textinput z-textbox']")).SendKeys("Consignment Entry");
                    System.Threading.Thread.Sleep(5000);
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div/div/div[2]/div/div[3]/div/div/div/table/tbody/tr[2]/td/div/div[1]/span")).Click();
                    System.Threading.Thread.Sleep(2000);
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/div[1]/span/input")).SendKeys(order);
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/div[2]/button")));
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/div[2]/button")).Click();
                    System.Threading.Thread.Sleep(2000);
                    string status = Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[3]/div[2]/div[1]/div[2]/div/div[3]/table/tbody[1]/tr/td[2]/div/span")).Text.Trim().ToString();
                    jse.ExecuteScript("alert(" + "'" + status + "'" + ");");
                    System.Threading.Thread.Sleep(5000);
                    Webdriver.SwitchTo().Alert().Accept();
                    System.Threading.Thread.Sleep(1000);
                    string UnitPrice = Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[3]/div[2]/div[1]/div[2]/div/div[3]/table/tbody[1]/tr/td[6]/div/span")).Text.Trim().ToString();
                    int UnitPriceINT = Convert.ToInt32(UnitPrice);
                    System.Threading.Thread.Sleep(1000);
                    string Discount = Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[3]/div[2]/div[1]/div[2]/div/div[3]/table/tbody[1]/tr/td[7]/div/span")).Text.Trim().TrimStart().TrimEnd().ToString();
                    if (Discount != "Null")
                    {
                        int DiscountINT = Convert.ToInt32(Discount);
                        int LastPrice = UnitPriceINT - DiscountINT;
                        jse.ExecuteScript("alert(" + "'" + LastPrice + "'" + ");");
                        System.Threading.Thread.Sleep(5000);
                        Webdriver.SwitchTo().Alert().Accept();
                        System.Threading.Thread.Sleep(1000);
                        if (BasketLastPriceINT == LastPrice && status == "Order has been created")
                        {
                            jse.ExecuteScript("alert('OK');");
                            System.Threading.Thread.Sleep(1000);
                            Webdriver.SwitchTo().Alert().Accept();
                            System.Threading.Thread.Sleep(1000);
                            Webdriver.Quit();
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Multiple_Card()
        {
            Login();
            WebDriverWait wait = new WebDriverWait(Webdriver, TimeSpan.FromSeconds(50));
            Webdriver.FindElement(By.CssSelector("a[class='btn-cart']")).Click();
            cookie();
            try
            {
                //Sepet Doluysa
                Webdriver.FindElement(By.CssSelector("button[class='link-inline js-remove-cart-entries']")).Click();
                cookie();
                Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
                cookie();
            }
            catch (Exception)
            {
                //Sepet Boşsa
                Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
                cookie();
            }

            //Search and PLP
            Webdriver.FindElement(By.CssSelector("svg[class='icon icon-search']")).Click();
            Webdriver.FindElement(By.Id("searchText")).SendKeys("Kahve");
            System.Threading.Thread.Sleep(2000);
            element = Webdriver.FindElement(By.Id("searchText"));
            element.SendKeys(OpenQA.Selenium.Keys.Return);
            cookie();

            //Go PDP
            System.Threading.Thread.Sleep(2000);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("a[href='/turk-kahve-makinesi/k-3300-mini-telve-beyaz-icecek-hazirlama']")));
            Webdriver.FindElement(By.CssSelector("a[href='/turk-kahve-makinesi/k-3300-mini-telve-beyaz-icecek-hazirlama']")).Click();
            cookie();

            //Hangi Mağazada Var
            IJavaScriptExecutor jse = (IJavaScriptExecutor)Webdriver;
            jse.ExecuteScript("scroll(0,1000);");
            System.Threading.Thread.Sleep(1000);
            //Ürün özellikleri kapat
            Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[0].Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[3].Click();
            Webdriver.FindElement(By.Id("cityCode")).Click();
            Webdriver.FindElement(By.CssSelector("option[value='34']")).Click();
            Webdriver.FindElement(By.Id("townCode")).Click();
            Webdriver.FindElement(By.CssSelector("option[value='34.34']")).Click();
            Webdriver.FindElement(By.CssSelector("button[title='Mağaza bul']")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[3].Click();
            //Taksit Seçenekleri
            //jse.ExecuteScript("scroll(0,4500)");
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[4].Click();
            System.Threading.Thread.Sleep(2000);
            int installment = Webdriver.FindElement(By.CssSelector("div[class='pdp-installments-list']")).FindElements(By.CssSelector("div[class='item']")).Count();
            if (installment > 0)
            {
                System.Threading.Thread.Sleep(2000);
                Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[4].Click();

                //Sepete At
                jse.ExecuteScript("scroll(6000,0)");
                System.Threading.Thread.Sleep(2000);
                Webdriver.FindElement(By.CssSelector("button[title='Sepete At']")).Click();
                System.Threading.Thread.Sleep(5000);
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div[class='ntf-msg']")));
                string hata = Webdriver.FindElement(By.CssSelector("div[class='p']")).Text.Trim().ToString();
                string basarili_islem = Webdriver.FindElement(By.CssSelector("div[class='ntf-msg']")).Text.Trim().ToString();
                if (hata == "Sepette bu üründen alabileceğiniz azami adete ulaştınız.")
                {
                    Webdriver.FindElement(By.CssSelector("button[class='btn btn-outline-dark js-popup-trigger-func']")).Click();
                }
                else if (basarili_islem == "Ürün sepetinize eklendi")
                {
                    Webdriver.FindElement(By.CssSelector("a[title='Sepete Git']")).Click();
                    cookie();
                    //Adet arttır
                    Webdriver.FindElement(By.CssSelector("button[class='btn-plus ']")).Click();
                    cookie();
                    //Adet Azalt
                    Webdriver.FindElement(By.CssSelector("button[class='btn-minus']")).Click();
                    cookie();
                    //Favori Ekle
                    Webdriver.FindElement(By.Id("g-recaptcha-btn-favourite")).Click();
                    System.Threading.Thread.Sleep(2000);
                    Webdriver.Navigate().Refresh();
                    cookie();
                    System.Threading.Thread.Sleep(2000);
                    //Sepeti Onayla
                    Webdriver.FindElement(By.CssSelector("a[href='/checkout/add-address']")).Click();
                    cookie();
                    string BasketLastPrice = Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div[2]/div/div/div/div[2]/div[1]/div/div[3]/div[1]/div/span")).Text.TrimStart().TrimEnd('L').TrimEnd('T').TrimEnd().ToString();
                    float BasketLastPriceINT = Convert.ToInt32(BasketLastPrice);
                    float BasketLastPriceCardINT = BasketLastPriceINT / 2;
                    //CART
                    //1.kart
                    Webdriver.FindElement(By.Id("pay-payment-card-multi")).Click();
                    Webdriver.FindElement(By.Id("mcard_step_2_amount")).SendKeys(Convert.ToString(BasketLastPriceCardINT));
                    Webdriver.FindElement(By.Id("mcard_step_2_cardno")).SendKeys("4022774022774026");
                    Webdriver.FindElement(By.Id("mcard_step_2_name")).SendKeys("Arcelik");
                    Webdriver.FindElement(By.Id("mcard_step_2_expiry")).SendKeys("1230");
                    Webdriver.FindElement(By.Id("mcard_step_2_cvc")).SendKeys("000");
                    jse.ExecuteScript("scroll(0,1000);");
                    System.Threading.Thread.Sleep(2000);
                    Webdriver.FindElement(By.CssSelector("button[title='Kartı Onayla']")).Click();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("input[class='value']")));
                    Webdriver.FindElement(By.CssSelector("input[class='value']")).SendKeys("a");
                    System.Threading.Thread.Sleep(1000);
                    Webdriver.FindElement(By.Id("submitbutton")).Click();
                    //2.kart
                    cookie();
                    Webdriver.FindElement(By.Id("mcard_step_3_cardno")).SendKeys("5571135571135575");
                    Webdriver.FindElement(By.Id("mcard_step_2_name")).SendKeys("Arcelik");
                    Webdriver.FindElement(By.Id("mcard_step_3_expiry")).SendKeys("1230");
                    Webdriver.FindElement(By.Id("mcard_step_2_cvv")).SendKeys("000");
                    //Onay
                    Webdriver.FindElement(By.Id("chk_cart_sum_confirm_1")).Click();
                    Webdriver.FindElement(By.Id("chk_cart_sum_confirm_2")).Click();
                    //Tamamla
                    Webdriver.FindElement(By.Id("postPayment")).Click();
                    //Güvenlik Kodu
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("input[class='value']")));
                    Webdriver.FindElement(By.CssSelector("input[class='value']")).SendKeys("a");
                    System.Threading.Thread.Sleep(1000);
                    Webdriver.FindElement(By.Id("submitbutton")).Click();
                    //Bitiş
                    cookie();
                    System.Threading.Thread.Sleep(1000);
                    string order = Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div/div/div[2]/div[1]/div[2]")).Text.TrimStart().TrimStart('S').TrimStart('i').TrimStart('p').TrimStart('a').TrimStart('r').TrimStart('i').TrimStart('ş').TrimStart().TrimStart('n').TrimStart('u').TrimStart('m').TrimStart('a').TrimStart('r').TrimStart('a').TrimStart('s').TrimStart('ı').TrimStart(':').TrimStart().ToString();
                    System.Threading.Thread.Sleep(1000);
                    //Hybris_Control
                    ((IJavaScriptExecutor)Webdriver).ExecuteScript("window.open();");
                    Webdriver.SwitchTo().Window(Webdriver.WindowHandles.Last());
                    Webdriver.Navigate().GoToUrl("https://backoffice.c1m0wu3z2z-arcelikas1-s1-public.model-t.cc.commerce.ondemand.com/backoffice/login.zul");
                    Webdriver.FindElement(By.CssSelector("input[name='j_username']")).SendKeys("muhammedfurkan_gunduz@arcelik.com");
                    Webdriver.FindElement(By.CssSelector("input[name='j_password']")).SendKeys("123");
                    Webdriver.FindElements(By.CssSelector("option[class='z-option']"))[2].Click();
                    Webdriver.FindElement(By.CssSelector("button[class='login_btn y-btn-primary z-button']")).Click();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[2]/div/div[2]/div/div/button")));
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[2]/div/div[2]/div/div/button")).Click();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("input[class='yw-explorerTree-filterTextbox yw-filter-textbox y-general-textinput z-textbox']")));
                    Webdriver.FindElement(By.CssSelector("input[class='yw-explorerTree-filterTextbox yw-filter-textbox y-general-textinput z-textbox']")).SendKeys("Consignment Entry");
                    System.Threading.Thread.Sleep(5000);
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div/div/div[2]/div/div[3]/div/div/div/table/tbody/tr[2]/td/div/div[1]/span")).Click();
                    System.Threading.Thread.Sleep(2000);
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/div[1]/span/input")).SendKeys(order);
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/div[2]/button")));
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/div[2]/button")).Click();
                    System.Threading.Thread.Sleep(2000);
                    string status = Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[3]/div[2]/div[1]/div[2]/div/div[3]/table/tbody[1]/tr/td[2]/div/span")).Text.Trim().ToString();
                    jse.ExecuteScript("alert(" + "'" + status + "'" + ");");
                    System.Threading.Thread.Sleep(5000);
                    Webdriver.SwitchTo().Alert().Accept();
                    System.Threading.Thread.Sleep(1000);
                    string UnitPrice = Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[3]/div[2]/div[1]/div[2]/div/div[3]/table/tbody[1]/tr/td[6]/div/span")).Text.Trim().ToString();
                    int UnitPriceINT = Convert.ToInt32(UnitPrice);
                    System.Threading.Thread.Sleep(1000);
                    string Discount = Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[3]/div[2]/div[1]/div[2]/div/div[3]/table/tbody[1]/tr/td[7]/div/span")).Text.Trim().TrimStart().TrimEnd().ToString();
                    if (Discount != "Null")
                    {
                        int DiscountINT = Convert.ToInt32(Discount);
                        int LastPrice = UnitPriceINT - DiscountINT;
                        jse.ExecuteScript("alert(" + "'" + LastPrice + "'" + ");");
                        System.Threading.Thread.Sleep(5000);
                        Webdriver.SwitchTo().Alert().Accept();
                        System.Threading.Thread.Sleep(1000);
                        if (BasketLastPriceINT == LastPrice && status == "Order has been created")
                        {
                            jse.ExecuteScript("alert('OK');");
                            System.Threading.Thread.Sleep(1000);
                            Webdriver.SwitchTo().Alert().Accept();
                            System.Threading.Thread.Sleep(1000);
                            Webdriver.Quit();
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void User_Operations()
        {
            Login();
            Webdriver.FindElement(By.CssSelector("a[href='/hesabim/bilgilerim']")).Click();
            cookie();
            //Cancel_Order
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.CssSelector("a[href='/hesabim/siparislerim ']")).Click();
            cookie();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElements(By.CssSelector("a[class='ord-block1']"))[2].Click();
            cookie();
            Webdriver.FindElement(By.CssSelector("a[class='link-inline btn-ord-back']")).Click();
            cookie();
            Webdriver.FindElement(By.Id("ocp_reason")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.CssSelector("option[value='orderedByMistake']")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.CssSelector("button[class='btn btn-outline-primary btn-narrow js-send-reason']")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
            cookie();
            //Add Address
            Webdriver.FindElement(By.CssSelector("a[href='/hesabim/bilgilerim']")).Click();
            cookie();
            Webdriver.FindElement(By.CssSelector("button[title='Yeni Adres Ekle']")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.Id("fullName")).SendKeys("Deneme Deneme");
            Webdriver.FindElement(By.Id("phone")).SendKeys("012345678");
            Webdriver.FindElement(By.Id("cityCode")).Click();
            Webdriver.FindElement(By.CssSelector("option[value='11']")).Click();
            Webdriver.FindElement(By.Id("townCode")).Click();
            Webdriver.FindElement(By.CssSelector("option[value='11.2']")).Click();
            Webdriver.FindElement(By.Id("neighborhood")).Click();
            Webdriver.FindElement(By.CssSelector("option[value='1647206567']")).Click();
            Webdriver.FindElement(By.Id("line1")).SendKeys("BBB");
            Webdriver.FindElement(By.Id("addressName")).Clear();
            Webdriver.FindElement(By.Id("addressName")).SendKeys("Yeni Adres");
            Webdriver.FindElement(By.CssSelector("button[class='btn btn-primary js-save-address']")).Click();
            cookie();
            //Update_Address
            Webdriver.FindElement(By.CssSelector("a[href='/hesabim/bilgilerim']")).Click();
            cookie();
            Webdriver.FindElement(By.CssSelector("a[class='link-inline js-edit-address']")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.Id("cityCode")).Click();
            Webdriver.FindElement(By.CssSelector("option[value='7']")).Click();
            Webdriver.FindElement(By.Id("townCode")).Click();
            Webdriver.FindElement(By.CssSelector("option[value='7.2']")).Click();
            Webdriver.FindElement(By.Id("neighborhood")).Click();
            Webdriver.FindElement(By.CssSelector("option[value='1350127475']")).Click();
            Webdriver.FindElement(By.Id("line1")).SendKeys("AAAA");
            Webdriver.FindElement(By.Id("addressName")).Clear();
            Webdriver.FindElement(By.Id("addressName")).SendKeys("Ev Adresi");
            Webdriver.FindElement(By.CssSelector("button[class='btn btn-primary js-save-address']")).Click();
            cookie();
            //Delete_Address
            Webdriver.FindElement(By.CssSelector("a[href='/hesabim/bilgilerim']")).Click();
            cookie();
            Webdriver.FindElements(By.CssSelector("a[class='link-inline js-remove-address js-ov-trg mr-10']"))[1].Click();
            Webdriver.FindElement(By.CssSelector("button[title='Sil']")).Click();
            //Update_Password
            Webdriver.FindElement(By.CssSelector("a[href='/hesabim/bilgilerim']")).Click();
            cookie();
            IJavaScriptExecutor js = Webdriver as IJavaScriptExecutor;
            js.ExecuteScript("window.scrollBy(0,700);");
            System.Threading.Thread.Sleep(2000);
            WebDriverWait wait = new WebDriverWait(Webdriver, TimeSpan.FromSeconds(50));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("a[href='/my-account/update-password']")));
            Webdriver.FindElement(By.CssSelector("a[href='/my-account/update-password']")).Click();
            cookie();
            Webdriver.FindElement(By.Id("newPassword")).SendKeys("12345furkan");
            Webdriver.FindElement(By.Id("newPasswordVerification")).SendKeys("12345furkan");
            Webdriver.FindElement(By.CssSelector("button[class='btn btn-outline-primary btn-profile-save']")).Click();
            cookie();
            System.Threading.Thread.Sleep(2000);
            cookie();
            Webdriver.Quit();
        }

        [TestMethod]
        public void Guest_User()
        {
            Start();
            WebDriverWait wait = new WebDriverWait(Webdriver, TimeSpan.FromSeconds(50));
            Webdriver.FindElement(By.CssSelector("a[class='btn-cart']")).Click();
            cookie();
            try
            {
                //Sepet Doluysa
                Webdriver.FindElement(By.CssSelector("button[class='link-inline js-remove-cart-entries']")).Click();
                cookie();
                Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
                cookie();
            }
            catch (Exception)
            {
                //Sepet Boşsa
                Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
                cookie();
            }

            //Search and PLP
            Webdriver.FindElement(By.CssSelector("svg[class='icon icon-search']")).Click();
            Webdriver.FindElement(By.Id("searchText")).SendKeys("Kahve");
            System.Threading.Thread.Sleep(2000);
            element = Webdriver.FindElement(By.Id("searchText"));
            element.SendKeys(OpenQA.Selenium.Keys.Return);
            cookie();

            //Go PDP
            System.Threading.Thread.Sleep(2000);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("a[href='/turk-kahve-makinesi/k-3300-mini-telve-beyaz-icecek-hazirlama']")));
            Webdriver.FindElement(By.CssSelector("a[href='/turk-kahve-makinesi/k-3300-mini-telve-beyaz-icecek-hazirlama']")).Click();
            cookie();

            //Hangi Mağazada Var
            IJavaScriptExecutor jse = (IJavaScriptExecutor)Webdriver;
            jse.ExecuteScript("scroll(0,1200);");
            System.Threading.Thread.Sleep(1000);
            //Ürün özellikleri kapat
            Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[0].Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[3].Click();
            Webdriver.FindElement(By.Id("cityCode")).Click();
            Webdriver.FindElement(By.CssSelector("option[value='34']")).Click();
            Webdriver.FindElement(By.Id("townCode")).Click();
            Webdriver.FindElement(By.CssSelector("option[value='34.34']")).Click();
            Webdriver.FindElement(By.CssSelector("button[title='Mağaza bul']")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[3].Click();
            //Taksit Seçenekleri
            //jse.ExecuteScript("scroll(0,4500)");
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[4].Click();
            System.Threading.Thread.Sleep(2000);
            int installment = Webdriver.FindElement(By.CssSelector("div[class='pdp-installments-list']")).FindElements(By.CssSelector("div[class='item']")).Count();
            if (installment > 0)
            {
                System.Threading.Thread.Sleep(2000);
                Webdriver.FindElements(By.CssSelector("div[class='acc-item']"))[4].Click();

                //Sepete At
                jse.ExecuteScript("scroll(6000,0)");
                System.Threading.Thread.Sleep(2000);
                Webdriver.FindElement(By.CssSelector("button[title='Sepete At']")).Click();
                System.Threading.Thread.Sleep(5000);
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div[class='ntf-msg']")));
                string hata = Webdriver.FindElement(By.CssSelector("div[class='p']")).Text.Trim().ToString();
                string basarili_islem = Webdriver.FindElement(By.CssSelector("div[class='ntf-msg']")).Text.Trim().ToString();
                if (hata == "Sepette bu üründen alabileceğiniz azami adete ulaştınız.")
                {
                    Webdriver.FindElement(By.CssSelector("button[class='btn btn-outline-dark js-popup-trigger-func']")).Click();
                }
                else if (basarili_islem == "Ürün sepetinize eklendi")
                {
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("a[title='Sepete Git']")));
                    Webdriver.FindElement(By.CssSelector("a[title='Sepete Git']")).Click();
                    cookie();
                    //Adet arttır
                    Webdriver.FindElement(By.CssSelector("button[class='btn-plus ']")).Click();
                    cookie();
                    //Adet Azalt
                    Webdriver.FindElement(By.CssSelector("button[class='btn-minus']")).Click();
                    cookie();
                    //Sepeti Onayla
                    Webdriver.FindElement(By.CssSelector("a[href='/checkout/add-address']")).Click();
                    cookie();
                    //Üye Olmadan Devam Et
                    cookie();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("button[title='Üye olmadan devam et']")));
                    Webdriver.FindElement(By.CssSelector("button[title='Üye olmadan devam et']")).Click();
                    Webdriver.FindElement(By.Id("guest_email")).SendKeys("test.furkan@outlook.com");
                    System.Threading.Thread.Sleep(3000);
                    Webdriver.FindElement(By.CssSelector("button[class='btn btn-primary btn-guest']")).Click();
                    //Adres Ekeleme
                    cookie();
                    Webdriver.FindElement(By.CssSelector("button[data-adr-type='SHIPPING']")).Click();
                    System.Threading.Thread.Sleep(2000);
                    Webdriver.FindElement(By.Id("fullName")).SendKeys("Test Test");
                    Webdriver.FindElement(By.Id("phone")).SendKeys("558999028");
                    Webdriver.FindElement(By.Id("cityCode")).Click();
                    Webdriver.FindElement(By.CssSelector("option[value='1']")).Click();
                    Webdriver.FindElement(By.Id("townCode")).Click();
                    Webdriver.FindElement(By.CssSelector("option[value='1.1']")).Click();
                    Webdriver.FindElement(By.Id("neighborhood")).Click();
                    Webdriver.FindElement(By.CssSelector("option[value='1571635607']")).Click();
                    Webdriver.FindElement(By.Id("line1")).SendKeys("Ev Adresi");
                    Webdriver.FindElement(By.Id("addressName")).SendKeys("EV Fatura Adresi");
                    Webdriver.FindElement(By.CssSelector("button[title='Adresi Kaydet']")).Click();
                    System.Threading.Thread.Sleep(5000);
                    cookie();
                    //TC
                    Webdriver.FindElement(By.Id("tcknArea")).SendKeys("12345678900");
                    //CART
                    Webdriver.FindElement(By.Id("pay-payment-card")).Click();
                    Webdriver.FindElement(By.Id("mcard_step_1_cardno")).SendKeys("4022774022774026");
                    Webdriver.FindElement(By.Id("mcard_step_1_name")).SendKeys("Arcelik");
                    Webdriver.FindElement(By.Id("mcard_step_1_expiry")).SendKeys("1230");
                    Webdriver.FindElement(By.Id("mcard_step_1_cvv")).SendKeys("000");
                    System.Threading.Thread.Sleep(1000);
                    string BasketLastPrice = Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div[2]/div/div/div/div[2]/div[1]/div/div[3]/div[1]/div/span")).Text.TrimStart().TrimEnd('L').TrimEnd('T').TrimEnd().ToString();
                    int BasketLastPriceINT = Convert.ToInt32(BasketLastPrice);
                    jse.ExecuteScript("alert(" + "'" + BasketLastPrice + "'" + ");");
                    //Onay
                    Webdriver.FindElement(By.Id("chk_cart_sum_confirm_1")).Click();
                    Webdriver.FindElement(By.Id("chk_cart_sum_confirm_2")).Click();
                    //Tamamla
                    Webdriver.FindElement(By.Id("postPayment")).Click();
                    //Güvenlik Kodu
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("input[class='value']")));
                    Webdriver.FindElement(By.CssSelector("input[class='value']")).SendKeys("a");
                    System.Threading.Thread.Sleep(1000);
                    Webdriver.FindElement(By.Id("submitbutton")).Click();
                    //Bitiş
                    cookie();
                    string order = Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div/div/div[2]/div[1]/div[3]/u[2]")).Text.TrimStart().TrimEnd().ToString();
                    System.Threading.Thread.Sleep(1000);
                    //Hybris_Control
                    ((IJavaScriptExecutor)Webdriver).ExecuteScript("window.open();");
                    Webdriver.SwitchTo().Window(Webdriver.WindowHandles.Last());
                    Webdriver.Navigate().GoToUrl("https://backoffice.c1m0wu3z2z-arcelikas1-s1-public.model-t.cc.commerce.ondemand.com/backoffice/login.zul");
                    Webdriver.FindElement(By.CssSelector("input[name='j_username']")).SendKeys("muhammedfurkan_gunduz@arcelik.com");
                    Webdriver.FindElement(By.CssSelector("input[name='j_password']")).SendKeys("123");
                    Webdriver.FindElements(By.CssSelector("option[class='z-option']"))[2].Click();
                    Webdriver.FindElement(By.CssSelector("button[class='login_btn y-btn-primary z-button']")).Click();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[2]/div/div[2]/div/div/button")));
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[2]/div/div[2]/div/div/button")).Click();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("input[class='yw-explorerTree-filterTextbox yw-filter-textbox y-general-textinput z-textbox']")));
                    Webdriver.FindElement(By.CssSelector("input[class='yw-explorerTree-filterTextbox yw-filter-textbox y-general-textinput z-textbox']")).SendKeys("Consignment Entry");
                    System.Threading.Thread.Sleep(5000);
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div/div/div[2]/div/div[3]/div/div/div/table/tbody/tr[2]/td/div/div[1]/span")).Click();
                    System.Threading.Thread.Sleep(2000);
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/div[1]/span/input")).SendKeys(order);
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/div[2]/button")));
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/div[2]/button")).Click();
                    System.Threading.Thread.Sleep(2000);
                    string status = Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[3]/div[2]/div[1]/div[2]/div/div[3]/table/tbody[1]/tr/td[2]/div/span")).Text.Trim().ToString();
                    jse.ExecuteScript("alert(" + "'" + status + "'" + ");");
                    System.Threading.Thread.Sleep(5000);
                    Webdriver.SwitchTo().Alert().Accept();
                    System.Threading.Thread.Sleep(1000);
                    string UnitPrice = Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[3]/div[2]/div[1]/div[2]/div/div[3]/table/tbody[1]/tr/td[6]/div/span")).Text.Trim().ToString();
                    int UnitPriceINT = Convert.ToInt32(UnitPrice);
                    System.Threading.Thread.Sleep(1000);
                    string Discount = Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[3]/div[2]/div[1]/div[2]/div/div[3]/table/tbody[1]/tr/td[7]/div/span")).Text.Trim().TrimStart().TrimEnd().ToString();
                    if (Discount != "Null")
                    {
                        int DiscountINT = Convert.ToInt32(Discount);
                        int LastPrice = UnitPriceINT - DiscountINT;
                        jse.ExecuteScript("alert(" + "'" + LastPrice + "'" + ");");
                        System.Threading.Thread.Sleep(5000);
                        Webdriver.SwitchTo().Alert().Accept();
                        System.Threading.Thread.Sleep(1000);
                        if (BasketLastPriceINT == LastPrice && status == "Order has been created")
                        {
                            jse.ExecuteScript("alert('OK');");
                            System.Threading.Thread.Sleep(1000);
                            Webdriver.SwitchTo().Alert().Accept();
                            System.Threading.Thread.Sleep(1000);
                            Webdriver.Quit();
                        }
                    }

                }
            }
        }

        [TestMethod]
        public void Forget_Password()
        {
            Start();

            Webdriver.FindElement(By.CssSelector("a[title='Üyelik']")).Click();
            cookie();

            Webdriver.FindElement(By.CssSelector("a[href='/password/forgot-password']")).Click();
            cookie();

            Webdriver.FindElement(By.Id("email")).SendKeys("test.arcelik.furkan@outlook.com");
            element = Webdriver.FindElement(By.Id("email"));
            element.SendKeys(OpenQA.Selenium.Keys.Return);
            System.Threading.Thread.Sleep(2000);
            //Webdriver.Navigate().Refresh();
            cookie();

            //Webdriver.FindElement(By.Id("email")).SendKeys("test.arcelik.furkan@outlook.com");
            //Webdriver.FindElement(By.CssSelector("input[type='submit']")).Click();


            ((IJavaScriptExecutor)Webdriver).ExecuteScript("window.open();");
            Webdriver.SwitchTo().Window(Webdriver.WindowHandles.Last());
            Webdriver.Navigate().GoToUrl("https://login.live.com/login.srf?wa=wsignin1.0&rpsnv=13&ct=1638800574&rver=7.0.6737.0&wp=MBI_SSL&wreply=https%3a%2f%2foutlook.live.com%2fowa%2f%3fnlp%3d1%26RpsCsrfState%3dc2034f5a-482c-5609-f5dc-01cabd65e76e&id=292841&aadredir=1&CBCXT=out&lw=1&fl=dob%2cflname%2cwld&cobrandid=90015");
            //System.Threading.Thread.Sleep(3000);
            //Webdriver.FindElement(By.CssSelector("a[class='internal sign-in-link']")).Click();

            Webdriver.FindElement(By.CssSelector("input[type='email']")).SendKeys("test.arcelik.furkan@outlook.com");
            Webdriver.FindElement(By.CssSelector("input[value='İleri']")).Click();
            System.Threading.Thread.Sleep(5000);
            Webdriver.FindElement(By.CssSelector("input[type='password']")).SendKeys("1234furkan");
            Webdriver.FindElement(By.Id("idSIButton9")).Click();
            System.Threading.Thread.Sleep(5000);
            Webdriver.FindElement(By.Id("idBtn_Back")).Click();
            System.Threading.Thread.Sleep(60000);
            Webdriver.FindElement(By.Id("Pivot25-Tab1")).Click();
            System.Threading.Thread.Sleep(5000);
            Webdriver.FindElements(By.CssSelector("div[class='NsB53xFTU532cgP0ztFSC']"))[0].Click();
            System.Threading.Thread.Sleep(5000);
            //FindElements(By.Id("AQAAAAAAAQABAAAAI+tjMwAAAAA=")).
            Webdriver.FindElement(By.CssSelector("a[rel='noopener noreferrer']")).Click();
            Webdriver.SwitchTo().Window(Webdriver.WindowHandles.Last());
            cookie();
            Webdriver.FindElement(By.Id("password")).SendKeys("12345furkan");
            Webdriver.FindElement(By.Id("checkPassword")).SendKeys("12345furkan");
            Webdriver.FindElement(By.CssSelector("button[title='ŞİFREMİ GÜNCELLE']")).Click();
            cookie();
            System.Threading.Thread.Sleep(5000);
            cookie();
            Webdriver.Quit();
        }

        [TestMethod]
        public void Discount_50TL()
        {
            Start();
            WebDriverWait wait = new WebDriverWait(Webdriver, TimeSpan.FromSeconds(50));
            Webdriver.FindElement(By.CssSelector("a[class='btn-cart']")).Click();
            cookie();
            try
            {
                //Sepet Doluysa
                Webdriver.FindElement(By.CssSelector("button[class='link-inline js-remove-cart-entries']")).Click();
                cookie();
                Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
                cookie();
            }
            catch (Exception)
            {
                //Sepet Boşsa
                Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
                cookie();
            }
            //Search and Go PLP
            Webdriver.FindElement(By.CssSelector("svg[class='icon icon-search']")).Click();
            Webdriver.FindElement(By.Id("searchText")).SendKeys("8836681600");
            System.Threading.Thread.Sleep(2000);
            element = Webdriver.FindElement(By.Id("searchText"));
            element.SendKeys(OpenQA.Selenium.Keys.Return);
            cookie();
            //Go PDP
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div[2]/div[3]/div[2]/div/div/div[5]/div[2]/a")).Click();
            cookie();
            //Add to Basket
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[1]/div[2]/div[2]/div[3]/div[3]/div/div[3]/div[2]/button")).Click();
            //Go to Basket
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/main/div/div/div[1]/div[1]/div[3]/a[2]")));
            Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[1]/div[1]/div[3]/a[2]")).Click();
            //In Basket
            System.Threading.Thread.Sleep(2000);
            cookie();
            string discount_1 = Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div/div/div[3]/div[1]/div[2]/div[1]/div[4]/div[2]/span")).Text.Trim('L').Trim('T').TrimEnd().ToString();
            if (discount_1 == "50")
            {
                Webdriver.Quit();
            }
        }

        [TestMethod]
        public void Discount_10Percent()
        {
            Start();
            WebDriverWait wait = new WebDriverWait(Webdriver, TimeSpan.FromSeconds(50));
            Webdriver.FindElement(By.CssSelector("a[class='btn-cart']")).Click();
            cookie();
            try
            {
                //Sepet Doluysa
                Webdriver.FindElement(By.CssSelector("button[class='link-inline js-remove-cart-entries']")).Click();
                cookie();
                Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
                cookie();
            }
            catch (Exception)
            {
                //Sepet Boşsa
                Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
                cookie();
            }
            //Search and Go PLP
            Webdriver.FindElement(By.CssSelector("svg[class='icon icon-search']")).Click();
            Webdriver.FindElement(By.Id("searchText")).SendKeys("8836651600");
            System.Threading.Thread.Sleep(2000);
            element = Webdriver.FindElement(By.Id("searchText"));
            element.SendKeys(OpenQA.Selenium.Keys.Return);
            cookie();

            //Go PDP
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div[2]/div[3]/div[2]/div/div/div[5]/div[2]/a")).Click();
            cookie();
            //Add to Basket
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[1]/div[2]/div[2]/div[3]/div[3]/div/div[3]/div[2]/button")).Click();
            //Go to Basket
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/main/div/div/div[1]/div[1]/div[3]/a[2]")));
            Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[1]/div[1]/div[3]/a[2]")).Click();
            //In Basket
            System.Threading.Thread.Sleep(2000);
            cookie();
            string first_price = Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div/div/div[3]/div[1]/div[2]/div[1]/div[2]/div[2]/span")).Text.Trim('L').Trim('T').TrimEnd().ToString();
            int first = Convert.ToInt32(first_price);
            string discount_2 = Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div/div/div[3]/div[1]/div[2]/div[1]/div[4]/div[2]/span")).Text.Trim('L').Trim('T').TrimEnd().ToString();
            double discount = Convert.ToDouble(discount_2);
            if (first*10/100==discount)
            {
                Webdriver.Quit();
            }
        }

        [TestMethod]
        public void Discount_Product100TL()
        {
            Start();
            WebDriverWait wait = new WebDriverWait(Webdriver, TimeSpan.FromSeconds(50));
            Webdriver.FindElement(By.CssSelector("a[class='btn-cart']")).Click();
            cookie();
            try
            {
                //Sepet Doluysa
                Webdriver.FindElement(By.CssSelector("button[class='link-inline js-remove-cart-entries']")).Click();
                cookie();
                Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
                cookie();
            }
            catch (Exception)
            {
                //Sepet Boşsa
                Webdriver.FindElement(By.CssSelector("a[class='btn btn-primary']")).Click();
                cookie();
            }
            //Search and Go PLP
            Webdriver.FindElement(By.CssSelector("svg[class='icon icon-search']")).Click();
            Webdriver.FindElement(By.Id("searchText")).SendKeys("8836761600");
            System.Threading.Thread.Sleep(2000);
            element = Webdriver.FindElement(By.Id("searchText"));
            element.SendKeys(OpenQA.Selenium.Keys.Return);
            cookie();

            //Go PDP
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div[2]/div[3]/div[2]/div/div/div[5]/div[2]/a")).Click();
            cookie();
            //Add to Basket
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[1]/div[2]/div[2]/div[3]/div[3]/div/div[3]/div[2]/button")).Click();
            //Go to Basket
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/main/div/div/div[1]/div[1]/div[3]/a[2]")));
            Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[1]/div[1]/div[3]/a[2]")).Click();
            //In Basket
            System.Threading.Thread.Sleep(2000);
            cookie();
            string discount_3 = Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div/div/div[3]/div[1]/div[2]/div[2]/div[1]/div/span")).Text.Trim().ToString();
            if (discount_3 == "100 TL")
            {
                Webdriver.Quit();
            }
        }

        [TestMethod]
        public void Create_Account()
        {
            Start();
            DateTime dt = DateTime.Now.AddMinutes(-1);
            WebDriverWait wait = new WebDriverWait(Webdriver, TimeSpan.FromSeconds(180));
            //Random Mail
            ((IJavaScriptExecutor)Webdriver).ExecuteScript("window.open();");
            Webdriver.SwitchTo().Window(Webdriver.WindowHandles.Last());
            Webdriver.Navigate().GoToUrl("https://fakermail.com/");
            System.Threading.Thread.Sleep(3000);
            Webdriver.FindElement(By.XPath("/html/body/div/div[2]/div/div[2]/div[2]/div/div/div/div/div[1]/span")).Click();
            //Arcelik
            Webdriver.SwitchTo().Window(Webdriver.WindowHandles[0]);
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.CssSelector("a[title='Üyelik']")).Click();
            cookie();
            Webdriver.FindElement(By.CssSelector("button[title='Üye ol']")).Click();
            Webdriver.FindElement(By.Id("firstName")).SendKeys("Hakan");
            Webdriver.FindElement(By.Id("lastName")).SendKeys("Can");
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.CssSelector("button[title='Devam Et']")).Click();
            System.Threading.Thread.Sleep(1000);
            //Webdriver.FindElement(By.Id("email")).Click();
            element = Webdriver.FindElement(By.Id("email"));
            element.SendKeys(OpenQA.Selenium.Keys.Control + "V");
            Webdriver.FindElement(By.Id("mobileNumber")).SendKeys("000000005");
            Webdriver.FindElement(By.Id("pwd")).SendKeys("12345furkan");
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div[1]/div[2]/form/div[2]/div[2]/ul/li[4]/button")).Click();
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.Id("personalDataProtectionLawApply")).Click();
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.Id("personalDataTransferApproving")).Click();
            System.Threading.Thread.Sleep(2000);
            IJavaScriptExecutor jse = (IJavaScriptExecutor)Webdriver;
            jse.ExecuteScript("scroll(0,500);");
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div[1]/div[2]/form/div[2]/div[3]/ul/li[9]/button")).Click();
            System.Threading.Thread.Sleep(1000);
            jse.ExecuteScript("scroll(500,0);");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("signup_confirm_code")));
            //Hybris Onay Kodu
            ((IJavaScriptExecutor)Webdriver).ExecuteScript("window.open();");
            Webdriver.SwitchTo().Window(Webdriver.WindowHandles.Last());
            Webdriver.Navigate().GoToUrl("https://backoffice.c1m0wu3z2z-arcelikas1-s1-public.model-t.cc.commerce.ondemand.com/backoffice/login.zul");
            Webdriver.FindElement(By.CssSelector("input[name='j_username']")).SendKeys("muhammedfurkan_gunduz@arcelik.com");
            Webdriver.FindElement(By.CssSelector("input[name='j_password']")).SendKeys("123");
            Webdriver.FindElements(By.CssSelector("option[class='z-option']"))[2].Click();
            Webdriver.FindElement(By.CssSelector("button[class='login_btn y-btn-primary z-button']")).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[2]/div/div[2]/div/div/button")));
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[2]/div/div[2]/div/div/button")).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("input[class='yw-explorerTree-filterTextbox yw-filter-textbox y-general-textinput z-textbox']")));
            Webdriver.FindElement(By.CssSelector("input[class='yw-explorerTree-filterTextbox yw-filter-textbox y-general-textinput z-textbox']")).SendKeys("Types");
            System.Threading.Thread.Sleep(6000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div/div/div[2]/div/div[3]/div/div/div/table/tbody/tr[2]/td/div/div[1]/span")).Click();
            System.Threading.Thread.Sleep(3000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/button")).Click();
            System.Threading.Thread.Sleep(3000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr[1]/td[3]/div/input")).SendKeys("otp");
            System.Threading.Thread.Sleep(5000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/div[2]/button")).Click();
            System.Threading.Thread.Sleep(3000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[3]/div[2]/div[1]/div[2]/div/div[3]/table/tbody[1]/tr[1]")).Click();
            System.Threading.Thread.Sleep(3000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div/div[3]/div/div/div[2]/div[1]/div/table/tbody/tr/td/table/tbody/tr/td[5]/div/table/tbody/tr/td/table/tbody/tr/td/img")).Click();
            System.Threading.Thread.Sleep(3000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[1]/div/span/a")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[4]/ul/li[4]")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[2]/span/a/i")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[4]/ul/li[3]")).Click();
            System.Threading.Thread.Sleep(2000);
            switch (dt.Month)
            {
                case 1:
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[3]/div/div/table/tbody/tr/td/table/tbody/tr/td/span/input")).SendKeys("Jan" + " " + dt.Day + "," + " "  + dt.Year + " " + dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
                    break;
                case 2:
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[3]/div/div/table/tbody/tr/td/table/tbody/tr/td/span/input")).SendKeys("Feb" + " " + dt.Day + "," + " " + dt.Year + " " + dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
                    break;
                case 3:
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[3]/div/div/table/tbody/tr/td/table/tbody/tr/td/span/input")).SendKeys("Mar" + " " + dt.Day + "," + " " + dt.Year + " " + dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
                    break;
                case 4:
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[3]/div/div/table/tbody/tr/td/table/tbody/tr/td/span/input")).SendKeys("Apr" + " " + dt.Day + "," + " " + dt.Year + " " + dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
                    break;
                case 5:
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[3]/div/div/table/tbody/tr/td/table/tbody/tr/td/span/input")).SendKeys("May" + " " + dt.Day + "," + " " + dt.Year + " " + dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
                    break;
                case 6:
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[3]/div/div/table/tbody/tr/td/table/tbody/tr/td/span/input")).SendKeys("Jun" + " " + dt.Day + "," + " " + dt.Year + " " + dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
                    break;
                case 7:
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[3]/div/div/table/tbody/tr/td/table/tbody/tr/td/span/input")).SendKeys("Jul" + " " + dt.Day + "," + " " + dt.Year + " " + dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
                    break;
                case 8:
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[3]/div/div/table/tbody/tr/td/table/tbody/tr/td/span/input")).SendKeys("Aug" + " " + dt.Day + "," + " " + dt.Year + " " + dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
                    break;
                case 9:
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[3]/div/div/table/tbody/tr/td/table/tbody/tr/td/span/input")).SendKeys("Sep" + " " + dt.Day + "," + " " + dt.Year + " " + dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
                    break;
                case 10:
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[3]/div/div/table/tbody/tr/td/table/tbody/tr/td/span/input")).SendKeys("Oct" + " " + dt.Day + "," + " " + dt.Year + " " + dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
                    break;
                case 11:
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[3]/div/div/table/tbody/tr/td/table/tbody/tr/td/span/input")).SendKeys("Nov" + " " + dt.Day + "," + " " + dt.Year + " " + dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
                    break;
                case 12:
                    Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[3]/div/div/table/tbody/tr/td/table/tbody/tr/td/span/input")).SendKeys("Dec" + " " + dt.Day + "," + " " + dt.Year + " " + dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
                    break;
            }
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr/td[5]/div/button")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/div[2]/button")).Click();
            System.Threading.Thread.Sleep(2000);
            string verCode = Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[3]/div[2]/div[1]/div[2]/div/div[3]/table/tbody[1]/tr/td[4]/div/span")).Text.Trim().ToString();
            System.Threading.Thread.Sleep(2000);
            Webdriver.SwitchTo().Window(Webdriver.WindowHandles[0]);

            //Arçelik VerCode
            Webdriver.FindElement(By.Id("signup_confirm_code")).SendKeys(verCode);
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/main/div/div/div[2]/div[1]/div[2]/form/div[2]/div[4]/ul/li[3]/button")).Click();
            System.Threading.Thread.Sleep(5000);
            //Email VerLink
            Webdriver.SwitchTo().Window(Webdriver.WindowHandles[1]);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div/main/main/div/div/div[2]/div/ul/li/a")));
            Webdriver.FindElement(By.XPath("/html/body/div/main/main/div/div/div[2]/div/ul/li/a")).Click();
            System.Threading.Thread.Sleep(2000);
            jse.ExecuteScript("scroll(0,500);");
            System.Threading.Thread.Sleep(2000);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div/main/main/div/div/div[2]/div[2]/dl/div[2]/custom/custom/table/tbody/tr/td[2]/div[1]/div[2]/div[1]/a")));
            Webdriver.FindElement(By.XPath("/html/body/div/main/main/div/div/div[2]/div[2]/dl/div[2]/custom/custom/table/tbody/tr/td[2]/div[1]/div[2]/div[1]/a")).Click();
            cookie();
            System.Threading.Thread.Sleep(5000);
            Webdriver.FindElement(By.CssSelector("button[title='Kapat']")).Click();

            //Clear Phone Values
            Webdriver.SwitchTo().Window(Webdriver.WindowHandles[2]);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div/div/div[2]/div/div[2]/div/input")).Clear();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div/div/div[2]/div/div[2]/div/input")).SendKeys("Customers");
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div/div/div[2]/div/div[3]/div/div/div/table/tbody/tr[2]/td/div/div[1]/span")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr[5]/td[3]/div/input")).SendKeys("05000000005");
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[1]/div/div[3]/table/tbody[1]/tr[2]/td[3]/div/input")).SendKeys("Hakan Can");
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[1]/div/div/div/div/div/div/div/div[2]/button")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div/div[2]/div/div/div[2]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/div[2]/div[3]/div[2]/div[1]/div[2]/div/div[3]/table/tbody[1]/tr/td[1]/div/span[1]/i")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.CssSelector("img[title='Bulk Edit']")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.CssSelector("input[class='y-attributepicker-values-filter z-textbox']")).SendKeys("phone");
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/div[2]/div/div/div[4]/div/div/div[1]/div/div/div/div/div[1]/div/div[3]/table/tbody/tr[1]")).Click();
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/div[2]/div/div/div[4]/div/div/div[1]/div/div/div/div/div[1]/div/div[3]/table/tbody/tr[2]")).Click();
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/div[2]/div/div/div[4]/div/div/div[1]/div/div/div/div/div[1]/div/div[3]/table/tbody/tr[3]")).Click();
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/div[2]/div/div/div[4]/div/div/div[1]/div/div/div/div/div[2]/button[2]")).Click();
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/div[2]/div/div/div[4]/div/div/div[1]/div/div/div/div/div[3]/div/div[3]/table/tbody/tr[1]")).Click();
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/div[2]/div/div/div[4]/div/div/div[1]/div/div/div/div/div[3]/div/div[3]/table/tbody/tr[2]")).Click();
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/div[2]/div/div/div[4]/div/div/div[1]/div/div/div/div/div[3]/div/div[3]/table/tbody/tr[3]")).Click();
            System.Threading.Thread.Sleep(1000);
            Webdriver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/div[2]/div/div/div[4]/div/div/div[2]/div/div[2]/div[2]/button")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/div[2]/div/div/div[4]/div/div/div[1]/div/div[1]/div[1]/div[3]/span/label[2]")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/div[2]/div/div/div[4]/div/div/div[1]/div/div[1]/div[2]/div[3]/span/label[2]")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/div[2]/div/div/div[4]/div/div/div[1]/div/div[1]/div[3]/div[3]/span/label[2]")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/div[2]/div/div/div[4]/div/div/div[2]/div/div[2]/div[2]/button")).Click();
            System.Threading.Thread.Sleep(2000);
            Webdriver.Quit();
        }


        public void cookie()
        {
            WebDriverWait wait = new WebDriverWait(Webdriver, TimeSpan.FromSeconds(50));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("onetrust-accept-btn-handler")));
            Webdriver.FindElement(By.Id("onetrust-accept-btn-handler")).Click();
        }
    }
}
