using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;
using MrCMS.Web.Apps.DynamicWidget.Models;

namespace MrCMS.Web.Apps.DynamicWidget.Core;

public class UniqueKeyDynamicWidgetRender : IDynamicWidgetPropertyRender
{
    public string Name => "uniqueKey";

    public async Task<IHtmlContent> RenderAsync(IHtmlHelper helper, string name, string existingValue,
        AttributeItem[] attributes = null)
    {
        var id = string.IsNullOrEmpty(existingValue) ? "uk_" + Guid.NewGuid().ToString("n")[..5] : existingValue;

        IHtmlContentBuilder htmlContent = new HtmlContentBuilder();

        var tagBuilder = new TagBuilder("input");
        tagBuilder.Attributes["type"] = "hidden";
        tagBuilder.Attributes["name"] = name;
        tagBuilder.Attributes["value"] = id;

        tagBuilder.Attributes["data-unique-key"] = null;
        tagBuilder.Attributes["data-dynamic-input"] = null;

        var elemntId = TagBuilder.CreateSanitizedId(name, "-");
        tagBuilder.Attributes["id"] = elemntId;

        htmlContent.AppendHtml(tagBuilder);

        var label = new TagBuilder("span");
        label.InnerHtml.Append(id);
        label.AddCssClass("font-weight-bold");

        htmlContent.AppendHtml(label);

        return await Task.FromResult(htmlContent);
    }
}