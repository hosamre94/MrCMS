using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Web.Apps.DynamicWidget.Models;

namespace MrCMS.Web.Apps.DynamicWidget.Core;

public interface IDynamicWidgetPropertyParser
{
    public string Name { get; }

    public Task<string> ParseAsync(IHtmlHelper helper, string name, string existingValue,
        AttributeItem[] attributes = null);
}