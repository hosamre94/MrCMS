﻿using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs
{
    public class MediaBreadcrumb : Breadcrumb
    {
        public override decimal Order => 1;
        public override string Controller => "MediaCategory";
        public override string Action => "Index";
        public override bool IsNav => true;
        public override string CssClass => "ti ti-photo";
    }
}