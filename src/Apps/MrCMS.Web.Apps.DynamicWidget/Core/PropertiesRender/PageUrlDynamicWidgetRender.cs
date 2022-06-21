using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Web.Apps.DynamicWidget.Models;
using NHibernate;

namespace MrCMS.Web.Apps.DynamicWidget.Core;

public class PageUrlDynamicWidgetRender : IDynamicWidgetPropertyRender
{
    private readonly ISession _session;
    private readonly IGetCurrentPage _getCurrentPage;


    public PageUrlDynamicWidgetRender(ISession session, IGetCurrentPage getCurrentPage)
    {
        _session = session;
        _getCurrentPage = getCurrentPage;
    }

    public string Name => "pageUrl";

    public async Task<IHtmlContent> RenderAsync(IHtmlHelper helper, string name, string existingValue,
        AttributeItem[] attributes = null)
    {
        var tagBuilder = new TagBuilder("input")
        {
            TagRenderMode = TagRenderMode.SelfClosing,
            Attributes =
            {
                ["id"] = TagBuilder.CreateSanitizedId(name, "-"),
                ["name"] = name,
                ["type"] = "text",
                ["readonly"] = "readonly",
                ["data-dynamic-input"] = null
            }
        };

        if (string.IsNullOrEmpty(existingValue))
        {
            // var culture = _getCurrentPage.GetCurrentPageUICulture();
            var page = _session.Query<Webpage>()
                .FirstOrDefault(x => x.WebpageType.EndsWith(name));

            existingValue = page?.UrlSegment;
        }

        tagBuilder.Attributes["value"] = existingValue;
        tagBuilder.AddCssClass("form-control");

        return await Task.FromResult(tagBuilder);
    }
}