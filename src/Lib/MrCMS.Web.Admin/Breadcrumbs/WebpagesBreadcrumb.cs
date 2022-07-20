using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs
{
    public class WebpagesBreadcrumb : Breadcrumb
    {
        public override string Controller => "Webpage";
        public override string Action => "Index";
        public override bool IsNav => true;
        public override string CssClass => "ti ti-file-code-2";
    }
}