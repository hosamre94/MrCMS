using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Web.Apps.DynamicWidget.Models;

namespace MrCMS.Web.Apps.DynamicWidget.Core;

public class TextAreaDynamicWidgetParser : IDynamicWidgetPropertyParser
{
    public string Name => "textarea";

    public async Task<string> ParseAsync(IHtmlHelper helper, string name, string existingValue,
        AttributeItem[] attributes = null)
    {
        return await Task.FromResult(existingValue);
    }
}