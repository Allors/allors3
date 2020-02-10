// <copyright file="WebDriverExtensions.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Components
{
    using System;
    using System.Threading;
    using OpenQA.Selenium;

    public static partial class WebDriverExtensions
    {
        public static void WaitForAngular(this IWebDriver @this)
        {
            const string Function =
                @"
if(window.getAngularTestability){
    var testability = window.getAngularTestability(document.querySelector('app-root'));
    if(testability){
        return testability.isStable();
    }
}

return false;
";
            var timeOut = DateTime.Now.AddMinutes(1);

            var javascriptExecutor = (IJavaScriptExecutor)@this;
            var isStable = false;
            while (isStable == false && timeOut > DateTime.Now)
            {
                isStable = (bool)javascriptExecutor.ExecuteScript(Function);
                Thread.Sleep(100);
            }
        }

        public static void WaitForCondition(this IWebDriver @this, Func<IWebDriver, bool> condition)
        {
            for (var i = 0; i < 30; i++)
            {
                if (condition(@this))
                {
                    return;
                }

                Thread.Sleep(1000);
            }

            throw new Exception("Condition not met");
        }

        public static bool SelectorIsVisible(this IWebDriver @this, By selector)
        {
            @this.WaitForAngular();
            var elements = @this.FindElements(selector);
            return (elements.Count == 1) && elements[0].Displayed;
        }
    }
}
