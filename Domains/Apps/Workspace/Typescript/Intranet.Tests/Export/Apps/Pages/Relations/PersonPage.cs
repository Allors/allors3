namespace Intranet.Pages.Relations
{
    using Allors.Meta;

    using Intranet.Tests;

    using OpenQA.Selenium;

    public class PersonPage : MainPage
    {
        public PersonPage(IWebDriver driver)
            : base(driver)
        {
        }

        public MaterialSelect Salutation => new MaterialSelect(this.Driver, roleType: M.Person.Salutation);

        public MaterialInput FirstName => new MaterialInput(this.Driver, roleType: M.Person.FirstName);

        public MaterialInput MiddleName => new MaterialInput(this.Driver, roleType: M.Person.MiddleName);

        public MaterialInput LastName => new MaterialInput(this.Driver, roleType: M.Person.LastName);

        public MaterialTextArea Comment => new MaterialTextArea(this.Driver, roleType: M.Person.Comment);

        public Button Save => new Button(this.Driver, By.XPath("//button/span[contains(text(), 'SAVE')]"));

    }
}
