using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;
using MrCMS.Web.Apps.DynamicWidget.Models;

namespace MrCMS.Web.Apps.DynamicWidget.Core;

public class MediaSelectorDynamicWidgetRender : IDynamicWidgetPropertyRender
{
    public string Name => "media";

    public async Task<IHtmlContent> RenderAsync(IHtmlHelper helper, string name, string existingValue,
        AttributeItem[] attributes = null)
    {
        IHtmlContentBuilder htmlContent = new HtmlContentBuilder();

        var tagBuilder = new TagBuilder("input")
        {
            Attributes =
            {
                ["type"] = "text",
                ["name"] = name,
                ["value"] = existingValue,
                ["data-type"] = "media-selector",
                ["id"] = TagBuilder.CreateSanitizedId(name, "-") + Guid.NewGuid().ToString("n")[..5],
                ["data-dynamic-input"] = null
            }
        };
        tagBuilder.AddCssClass("form-control");

        var breakTag = new TagBuilder("br")
        {
            TagRenderMode = TagRenderMode.SelfClosing
        };

        htmlContent.AppendHtml(breakTag);
        htmlContent.AppendHtml(tagBuilder);

        return await Task.FromResult(htmlContent);
    }
}