using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Web.Apps.DynamicWidget.Models;

namespace MrCMS.Web.Apps.DynamicWidget.Core;

public class TextAreaDynamicWidgetRender : IDynamicWidgetPropertyRender
{
    public string Name => "textarea";

    public async Task<IHtmlContent> RenderAsync(IHtmlHelper helper, string name, string existingValue,
        AttributeItem[] attributes = null)
    {
        var tagBuilder = new TagBuilder("textarea");
        tagBuilder.Attributes["type"] = "text";
        tagBuilder.Attributes["name"] = name;
        tagBuilder.InnerHtml.Append(existingValue);
        tagBuilder.Attributes["data-dynamic-input"] = null;

        tagBuilder.TagRenderMode = TagRenderMode.Normal;
        var elemntId = TagBuilder.CreateSanitizedId(name, "-");
        tagBuilder.Attributes["id"] = elemntId;

        tagBuilder.AddCssClass("form-control");
        return await Task.FromResult(tagBuilder);
    }
}