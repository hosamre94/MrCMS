﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class WidgetController : MrCMSAdminController
    {
        private readonly IWidgetAdminService _widgetService;
        private readonly ISetWidgetAdminViewData _setAdminViewData;
        private readonly IModelBindingHelperAdapter _modelBindingHelperAdapter;

        public WidgetController(IWidgetAdminService widgetService, ISetWidgetAdminViewData setAdminViewData, IModelBindingHelperAdapter modelBindingHelperAdapter)
        {
            _widgetService = widgetService;
            _setAdminViewData = setAdminViewData;
            _modelBindingHelperAdapter = modelBindingHelperAdapter;
        }

        [HttpGet]
        public PartialViewResult Add(int layoutAreaId, int? pageId, string returnUrl = null)
        {
            TempData["returnUrl"] = returnUrl;
            var model = new AddWidgetModel
            {
                LayoutAreaId = layoutAreaId,
                WebpageId = pageId
            };
            return PartialView(model);
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<JsonResult> Add_POST(AddWidgetModel model)
        {
            var additionalPropertyModel = _widgetService.GetAdditionalPropertyModel(model.WidgetType);
            if (additionalPropertyModel != null)
            {
                await _modelBindingHelperAdapter.TryUpdateModelAsync(this, additionalPropertyModel, additionalPropertyModel.GetType(), string.Empty);
            }
            var widget = await _widgetService.AddWidget(model, additionalPropertyModel);

            return Json(widget.Id);
        }

        [HttpGet]
        [ActionName("Edit")]
        public ViewResult Edit_Get(int id, string returnUrl = null)
        {
            var editModel = _widgetService.GetEditModel(id);
            var widget = _widgetService.GetWidget(id);
            _setAdminViewData.SetViewData(ViewData, widget);
            ViewData["widget"] = widget;

            if (!string.IsNullOrEmpty(returnUrl))
            {
                ViewData["return-url"] = Request.Referer();
            }
            else
            {
                ViewData["return-url"] = returnUrl;
            }

            return View(editModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(UpdateWidgetModel model, string returnUrl = null)
        {
            var additionalPropertyModel = _widgetService.GetAdditionalPropertyModel(model.Id);
            if (additionalPropertyModel != null)
            {
                await _modelBindingHelperAdapter.TryUpdateModelAsync(this, additionalPropertyModel, additionalPropertyModel.GetType(), string.Empty);
            }

            var widget = await _widgetService.UpdateWidget(model, additionalPropertyModel);

            return string.IsNullOrWhiteSpace(returnUrl)
                ? widget.WebpageId != null
                    ? RedirectToAction("Edit", "Webpage", new { id = widget.WebpageId })
                    : (ActionResult)RedirectToAction("Edit", "LayoutArea", new { id = widget.LayoutAreaId })
                : Redirect(returnUrl);
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult Delete_Get(int id)
        {
            return PartialView(_widgetService.GetEditModel(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id, string returnUrl)
        {
            var widget = await _widgetService.DeleteWidget(id);

            int webpageId = 0;
            int layoutAreaId = 0;
            if (widget.Webpage != null)
            {
                webpageId = widget.Webpage.Id;
            }

            if (widget.LayoutArea != null)
            {
                layoutAreaId = widget.LayoutArea.Id;
            }

            return !string.IsNullOrWhiteSpace(returnUrl) &&
                   !returnUrl.Contains("widget/edit/", StringComparison.OrdinalIgnoreCase)
                ? (ActionResult)Redirect(returnUrl)
                : webpageId > 0
                    ? RedirectToAction("Edit", "Webpage", new { id = webpageId, layoutAreaId })
                    : RedirectToAction("Edit", "LayoutArea", new { id = layoutAreaId });
        }

        [HttpGet]
        public ActionResult AddProperties(string type)
        {
            var model = _widgetService.GetAdditionalPropertyModel(type);
            if (model != null)
            {
                _setAdminViewData.SetViewDataForAdd(ViewData, type);
                ViewData["type"] = TypeHelper.GetTypeByName(type);
                return PartialView(model);
            }
            return new EmptyResult();
        }
    }
}