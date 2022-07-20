﻿using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs
{
    public class SystemBreadcrumb : Breadcrumb
    {
        public override decimal Order => 100;
        public override string Controller => "";
        public override string Action => "";
        public override bool IsPlaceHolder => true;
        public override bool IsNav => true;
        public override string CssClass => "ti ti-settings";
    }
}