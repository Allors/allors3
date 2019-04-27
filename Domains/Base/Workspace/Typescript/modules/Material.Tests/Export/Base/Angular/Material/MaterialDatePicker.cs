namespace Angular.Material
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Allors.Meta;

    using Angular;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    public class MaterialDatePicker
    : Directive
    {
        public MaterialDatePicker(IWebDriver driver, RoleType roleType)
        : base(driver)
        {
            this.Selector = new ByChained(By.CssSelector($"div[data-allors-roletype='{roleType.IdAsNumberString}']"), By.CssSelector("input"));
        }

        public By Selector { get; }
        
        public DateTime? Value
        {
            get
            {
                this.Driver.WaitForAngular();
                var element = this.Driver.FindElement(this.Selector);
                var value = element.GetAttribute("value");
                if (!string.IsNullOrEmpty(value))
                {
                    return DateTime.Parse(value);
                }

                return null;
            }

            set
            {
                this.Driver.WaitForAngular();
                var element = this.Driver.FindElement(this.Selector);
                this.ScrollToElement(element);
                element.Clear();

                this.Driver.WaitForAngular();
                element.SendKeys(Keys.Control + "a");
                element.SendKeys(Keys.Delete);

                if (value != null)
                {
                    this.Driver.WaitForAngular();
                    element.SendKeys(value.Value.ToString("d"));
                }

                this.Driver.WaitForAngular();
                element.SendKeys(Keys.Tab);
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class MaterialDatePicker<T> : MaterialDatePicker where T : Component
    {
        public MaterialDatePicker(T page, RoleType roleType)
            : base(page.Driver, roleType)
        {
            this.Page = page;
        }

        public T Page { get; }

        public T Set(DateTime value)
        {
            this.Value = value;
            return this.Page;
        }
    }
}
