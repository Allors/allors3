namespace Angular.Material
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using Allors.Meta;

    using Angular;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;

    public class MaterialFiles : Directive
    {
        public MaterialFiles(IWebDriver driver, RoleType roleType)
            : base(driver)
        {
            this.Selector = By.CssSelector($"a-mat-files *[data-allors-roletype='{roleType.IdAsNumberString}']");
        }

        public By Selector { get; }

        public IWebElement Input => this.Driver.FindElement(new ByChained(this.Selector, By.CssSelector("input[type='file']")));

        public IWebElement Delete => this.Driver.FindElement(new ByChained(this.Selector, By.LinkText("DELETE")));

        public void Upload(string fileName)
        {
            var file = new FileInfo(fileName);

            this.Driver.WaitForAngular();
            this.ScrollToElement(this.Input);
            this.Input.SendKeys(file.FullName);
        }

        public void Remove()
        {
            this.Driver.WaitForAngular();
            this.ScrollToElement(this.Delete);
            this.Delete.Click();
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FilesMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class MaterialFiles<T> : MaterialFiles where T : Component
    {
        public MaterialFiles(T page, RoleType roleType)
            : base(page.Driver, roleType)
        {
            this.Page = page;
        }

        public T Page { get; }

        public new T Upload(string fileName)
        {
            base.Upload(fileName);
            return this.Page;
        }

        public new T Remove()
        {
            base.Remove();
            return this.Page;
        }
    }
}
