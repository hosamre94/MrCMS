using MrCMS.Web.Admin.Infrastructure.ModelBinding;

namespace MrCMS.Web.Apps.DynamicWidget.Areas.Admin.Models.Widgets;

public class DynamicWidgetModel : IUpdatePropertiesViewModel<DynamicWidget.Widgets.DynamicWidget>,
    IAddPropertiesViewModel<DynamicWidget.Widgets.DynamicWidget>
{
    public string Text { get; set; }
    public string Properties { get; set; }
}