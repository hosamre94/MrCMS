using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Web.Apps.DynamicWidget.Models;

namespace MrCMS.Web.Apps.DynamicWidget.Core;

public class TextBoxDynamicWidgetRender : IDynamicWidgetPropertyRender
{
    public string Name => "text";

    public async Task<IHtmlContent> RenderAsync(IHtmlHelper helper, string name, string existingValue,
        AttributeItem[] attributes = null)
    {
        var tagBuilder = new TagBuilder("input");
        tagBuilder.Attributes["id"] = TagBuilder.CreateSanitizedId(name, "-");
        tagBuilder.Attributes["name"] = name;
        tagBuilder.Attributes["type"] = "text";
        tagBuilder.Attributes["value"] = existingValue;
        tagBuilder.Attributes["data-dynamic-input"] = null;
        ;

        tagBuilder.AddCssClass("form-control");

        return await Task.FromResult(tagBuilder);
    }
}