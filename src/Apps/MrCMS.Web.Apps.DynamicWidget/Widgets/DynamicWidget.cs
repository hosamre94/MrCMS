using MrCMS.Entities.Widget;
using MrCMS.Website;

namespace MrCMS.Web.Apps.DynamicWidget.Widgets;

[WidgetOutputCacheable]
public class DynamicWidget : Widget
{
    public virtual string Text { get; set; }
    public virtual string Properties { get; set; }
}