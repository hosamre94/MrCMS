using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Shortcodes;
using MrCMS.Web.Apps.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class InPageAdminService : IInPageAdminService
    {
        private readonly ISession _session;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IShortcodeParser _shortcodeParser;

        public InPageAdminService(ISession session, IStringResourceProvider stringResourceProvider,
            IShortcodeParser shortcodeParser)
        {
            _session = session;
            _stringResourceProvider = stringResourceProvider;
            _shortcodeParser = shortcodeParser;
        }

        public SaveResult SaveBodyContent(UpdatePropertyData updatePropertyData)
        {
            HashSet<Type> types = TypeHelper.GetAllConcreteTypesAssignableFrom<SystemEntity>();
            Type entityType = types.FirstOrDefault(t => t.Name == updatePropertyData.Type);
            if (entityType == null)
                return new SaveResult(false, string.Format(_stringResourceProvider.GetValue("Admin Inline Editing Save Entity Not Found", "Could not find entity type '{0}'"), updatePropertyData.Type));
            object entity = _session.Get(entityType, updatePropertyData.Id);
            if (entity == null)
                return new SaveResult(false,
                    string.Format(_stringResourceProvider.GetValue("Admin InlineEditing Save Not Found", "Could not find entity of type '{0}' with id {1}"), updatePropertyData.Type,
                        updatePropertyData.Id));
            PropertyInfo propertyInfo =
                entityType.GetProperties().FirstOrDefault(info => info.Name == updatePropertyData.Property);
            if (propertyInfo == null)
                return new SaveResult(false,
                    string.Format(_stringResourceProvider.GetValue("Admin InlineEditing Save NotFound", "Could not find entity of type '{0}' with id {1}"), updatePropertyData.Type,
                        updatePropertyData.Id));
            var stringLengthAttribute = propertyInfo.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(StringLengthAttribute)) as StringLengthAttribute;
            if (stringLengthAttribute != null && updatePropertyData.Content.Length > stringLengthAttribute.MaximumLength)
            {
                return new SaveResult
                {
                    success = false,
                    message = string.Format(_stringResourceProvider.GetValue("Admin InlineEditing Save MaxLength", "Could not save property. The maximum length for this field is {0} characters."), stringLengthAttribute.MaximumLength)
                };
            }
            if (string.IsNullOrWhiteSpace(updatePropertyData.Content) &&
                propertyInfo.GetCustomAttributes(typeof (RequiredAttribute), false).Any())
            {
                return new SaveResult(false,
                    string.Format(_stringResourceProvider.GetValue("Admin InlineEditing Save Required", "Could not edit '{0}' as it is required"), updatePropertyData.Property));
            }

            try
            {
                propertyInfo.SetValue(entity, updatePropertyData.Content, null);
                _session.Transact(session => session.SaveOrUpdate(entity));
            }
            catch (Exception ex)
            {
                //CurrentRequestData.ErrorSignal.Raise(ex);
                // TODO: logging

                return new SaveResult(false, string.Format(_stringResourceProvider.GetValue("Admin InlineEditing Save Error", "Could not save to database '{0}' due to unknown error. Please check log."), updatePropertyData.Property));
            }

            return new SaveResult();
        }

        public string GetUnformattedBodyContent(GetPropertyData getPropertyData)
        {
            return GetContent(getPropertyData).Content;
        }

        public string GetFormattedBodyContent(GetPropertyData getPropertyData, Controller controller)
        {
            ContentInfo content = GetContent(getPropertyData);

            //if (entity is Webpage)
            //    CurrentRequestData.CurrentPage = entity as Webpage;
            //IHtmlHelper htmlHelper = null;
            return _shortcodeParser.Parse(controller.GetHtmlHelper(), content.Content).ToString();
        }

        private ContentInfo GetContent(GetPropertyData getPropertyData)
        {
            HashSet<Type> types = TypeHelper.GetAllConcreteTypesAssignableFrom<SystemEntity>();
            Type entityType = types.FirstOrDefault(t => t.Name == getPropertyData.Type);
            if (entityType == null)
                return new ContentInfo();
            object entity = _session.Get(entityType, getPropertyData.Id);
            if (entity == null)
                return new ContentInfo();
            PropertyInfo propertyInfo =
                entityType.GetProperties().FirstOrDefault(info => info.Name == getPropertyData.Property);
            if (propertyInfo == null)
                return new ContentInfo();
            return new ContentInfo
            {
                Content = Convert.ToString(propertyInfo.GetValue(entity, null)),
                Entity = entity
            };
        }

        private struct ContentInfo
        {
            public string Content { get; set; }
            public object Entity { get; set; }
        }
    }
}