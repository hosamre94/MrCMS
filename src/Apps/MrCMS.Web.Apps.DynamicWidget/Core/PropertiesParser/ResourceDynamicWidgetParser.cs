using MrCMS.Services.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Web.Apps.DynamicWidget.Models;

namespace MrCMS.Web.Apps.DynamicWidget.Core;

public class ResourceDynamicWidgetParser : IDynamicWidgetPropertyParser
{
    private readonly IStringResourceProvider _stringResourceProvider;
    // private readonly IGetCurrentCultureInfo _getCurrentUserCultureInfo;

    public ResourceDynamicWidgetParser(IStringResourceProvider stringResourceProvider
        /*IGetCurrentCultureInfo getCurrentUserCultureInfo*/)
    {
        _stringResourceProvider = stringResourceProvider;
        // _getCurrentUserCultureInfo = getCurrentUserCultureInfo;
    }


    public string Name => "resource";

    public async Task<string> ParseAsync(IHtmlHelper helper, string name, string existingValue,
        AttributeItem[] attributes = null)
    {
        // var culture = _getCurrentUserCultureInfo.GetPageOrDefault();
        var defaultValue = attributes?.FirstOrDefault(x => x.Key == "default");
        return await _stringResourceProvider.GetValue(name, defaultValue?.Value);
    }
}