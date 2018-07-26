using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ILayoutAdminService
    {
        Layout GetAddLayoutModel(int? id);
        void Add(Layout layout);
        void Update(Layout layout);
        void Delete(Layout layout);
        List<SortItem> GetSortItems(Layout parent);
        void SetOrders(List<SortItem> items);
        bool UrlIsValidForLayout(string urlSegment, int? id);
        void SetParent(Layout layout, int? parentId);
        IEnumerable<SelectListItem> GetValidParents(Layout layout);
    }
}