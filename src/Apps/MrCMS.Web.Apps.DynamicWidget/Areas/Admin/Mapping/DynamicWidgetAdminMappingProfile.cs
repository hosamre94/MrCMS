using AutoMapper;
using MrCMS.Web.Apps.DynamicWidget.Areas.Admin.Models.Widgets;

namespace MrCMS.Web.Apps.DynamicWidget.Areas.Admin.Mapping;

public class DynamicWidgetAdminMappingProfile : Profile
{
    public DynamicWidgetAdminMappingProfile()
    {
        CreateMap<Widgets.DynamicWidget, DynamicWidgetModel>().ReverseMap();
    }
}