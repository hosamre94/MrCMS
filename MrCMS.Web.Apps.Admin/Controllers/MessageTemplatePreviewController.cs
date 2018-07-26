﻿using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class MessageTemplatePreviewController : MrCMSAdminController
    {
        private readonly IMessageTemplatePreviewService _messageTemplatePreviewService;

        public MessageTemplatePreviewController(IMessageTemplatePreviewService messageTemplatePreviewService)
        {
            _messageTemplatePreviewService = messageTemplatePreviewService;
        }

        public ViewResult Get(string type)
        {
            return View(_messageTemplatePreviewService.GetTemplate(type));
        }

        public PartialViewResult Preview(string type, int id)
        {
            return PartialView(_messageTemplatePreviewService.GetPreview(type, id));
        }
    }
}