namespace Intranet.Tests
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    public class Element : Component
    {
        public Element(IWebDriver driver, By selector)
        : base(driver)
        {
            this.Selector = selector;
        }

        public By Selector { get; }

        public bool IsVisible => this.SelectorIsVisible(this.Selector);

        public void Click()
        {
            this.Driver.WaitForAngular();

            var element = this.Driver.FindElement(this.Selector);
            this.ScrollToElement(element);
            element.Click();

            this.Driver.WaitForAngular();
        }
    }
}