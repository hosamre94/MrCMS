using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Helpers;
using MrCMS.Web.Apps.DynamicWidget.Core;

namespace MrCMS.Web.Apps.DynamicWidget;

public class MrCMSDynamicWidgetApp : StandardMrCMSApp
{
    public override string Name => "DynamicWidgets";
    public override string Version => "1.0";

    public override IServiceCollection RegisterServices(IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<IDynamicWidgetPropertyRender>())
            serviceCollection.AddScoped(typeof(IDynamicWidgetPropertyRender), type);

        foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<IDynamicWidgetPropertyParser>())
            serviceCollection.AddScoped(typeof(IDynamicWidgetPropertyParser), type);

        return serviceCollection;
    }
}