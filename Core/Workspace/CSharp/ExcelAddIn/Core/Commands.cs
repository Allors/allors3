﻿// <copyright file="Commands.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Excel
{
    using System;
    using System.Threading.Tasks;

    using NLog;

    public partial class Commands
    {
        public Commands(Sheets sheets) => this.Sheets = sheets;

        public Sheets Sheets { get; }

        public bool CanSave => this.Sheets.ActiveSheet != null;

        public bool CanRefresh => this.Sheets.ActiveSheet != null;

        public bool CanNew => this.Sheets.ActiveSheet == null;

        protected Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public async Task Save()
        {
            try
            {
                var sheet = this.Sheets.ActiveSheet;
                if (sheet != null)
                {
                    if (!(await sheet.Save()).HasErrors)
                    {
                        await sheet.Refresh();
                    }
                }
            }
            catch (Exception e)
            {
                e.Handle();
            }
        }

        public async Task Refresh()
        {
            try
            {
                var sheet = this.Sheets.ActiveSheet;
                if (sheet != null)
                {
                    await sheet.Refresh();
                }
            }
            catch (Exception e)
            {
                e.Handle();
            }
        }
    }
}
