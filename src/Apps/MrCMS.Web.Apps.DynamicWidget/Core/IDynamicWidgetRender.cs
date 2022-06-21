using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.DynamicWidget.Core;

public interface IDynamicWidgetRender
{
    Task<IHtmlContent> RenderAsync(IHtmlHelper helper, string text, JsonElement properties);
}