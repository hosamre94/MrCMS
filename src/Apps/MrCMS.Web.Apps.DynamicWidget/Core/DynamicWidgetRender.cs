using MrCMS.Helpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MrCMS.Web.Apps.DynamicWidget.Helper;
using MrCMS.Web.Apps.DynamicWidget.Models;

namespace MrCMS.Web.Apps.DynamicWidget.Core;

public class DynamicWidgetRender : IDynamicWidgetRender
{
    private static readonly Regex ShortcodeMatcher =
        new(@"\[([\w-_]+)([^\]]*)?\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Dictionary<string, IDynamicWidgetPropertyRender> _renderers;

    private readonly string _checkboxPrefix = "CBI_";

    public DynamicWidgetRender(IEnumerable<IDynamicWidgetPropertyRender> renderers)
    {
        _renderers = renderers.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
    }

    public bool CanRender(string typeName)
    {
        return _renderers.ContainsKey(typeName);
    }

    private bool IsArray(string typeName)
    {
        return string.Equals(typeName, "array", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(typeName, "array-end", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<IHtmlContent> RenderAsync(IHtmlHelper helper, string text, JsonElement properties)
    {
        if (string.IsNullOrEmpty(text))
            return HtmlString.Empty;

        return await RenderProperties(helper, text, properties);
    }

    private async Task<IHtmlContentBuilder> RenderProperties(IHtmlHelper helper, string text, JsonElement properties)
    {
        var htmlContent = new HtmlContentBuilder();
        var matchCollection = ShortcodeMatcher.Matches(text);
        if (!matchCollection.Any())
            return htmlContent;

        var treeNodeList = BuildPropertiesTree(matchCollection);

        //TODO
        //check if it can run in parallel
        foreach (var node in treeNodeList)
            htmlContent.AppendHtml(await RenderProperties(helper, node, properties));

        return htmlContent;
    }

    private async Task<IHtmlContent> RenderProperties(IHtmlHelper helper, RenderNode node, JsonElement properties,
        string prefix = null)
    {
        return node.Children != null && node.Children.Any()
            ? await RenderArray(helper, node, properties, prefix)
            : await RenderProperty(helper, node, properties, prefix);
    }

    #region Tree

    private List<RenderNode> BuildPropertiesTree(MatchCollection matchCollection)
    {
        var tree = new List<RenderNode>();
        for (var i = 0; i < matchCollection.Count; i++)
        {
            var match = matchCollection[i];
            if (match.Groups.Count != 3)
                continue;

            var attributes = AttributesHelper.GetAttributes(match.Groups[2].Value);
            if (attributes.Length == 0)
                continue;

            var nameValueTuple = attributes.FirstOrDefault(x => x.Key == "name");
            if (string.IsNullOrEmpty(nameValueTuple.Value))
                continue;

            var typeName = match.Groups[1].Value;
            var node = new RenderNode(nameValueTuple.Value, typeName, attributes);
            if (string.Equals(typeName, "array", StringComparison.OrdinalIgnoreCase))
                node.Children = GetChildren(matchCollection, ref i, typeName);

            if (!tree.Any(x => x.Name == nameValueTuple.Value))
                tree.Add(node);
        }

        return tree;
    }

    private List<RenderNode> GetChildren(MatchCollection matchCollection, ref int i, string parentName)
    {
        var children = new List<RenderNode>();
        for (i += 1; i < matchCollection.Count; i++)
        {
            var match = matchCollection[i];
            if (match.Groups.Count != 3)
                continue;

            var typeName = match.Groups[1].Value;
            if (string.Equals(typeName, "array-end", StringComparison.OrdinalIgnoreCase))
                return children;

            var attributes = AttributesHelper.GetAttributes(match.Groups[2].Value);
            if (attributes.Length == 0)
                continue;

            var nameValueTuple = attributes.FirstOrDefault(x => x.Key == "name");
            if (string.IsNullOrEmpty(nameValueTuple.Value))
                continue;

            var node = new RenderNode(nameValueTuple.Value, typeName, attributes);
            if (string.Equals(typeName, "array", StringComparison.OrdinalIgnoreCase))
                node.Children = GetChildren(matchCollection, ref i, nameValueTuple.Value);

            if (!children.Any(x => x.Name == nameValueTuple.Value))
                children.Add(node);
        }

        throw new Exception($"array '{parentName}' dose not have a closing tag");
    }

    #endregion

    #region List Render

    private async Task<IHtmlContent> RenderArray(IHtmlHelper helper, RenderNode node, JsonElement properties,
        string prefix = null)
    {
        var name = node.Name;
        var props = new JsonElement();
        if (properties.ValueKind != JsonValueKind.Null && properties.ValueKind != JsonValueKind.Undefined)
            properties.TryGetProperty(name, out props);


        //Table
        var table = new TagBuilder("table");
        table.AddCssClass("m-0 table table-bordered table-sm dynamic-table");

        //Add headers
        var header = GetTableHeader(node.Children);
        table.InnerHtml.AppendHtml(header);

        //Add rows
        prefix = $"{prefix}[{name}]";
        var rows = await GetTableRows(node.Children, props, helper, prefix);
        table.InnerHtml.AppendHtml(rows);

        return table;
    }

    #endregion

    #region Property Render

    private async Task<IHtmlContent> RenderProperty(IHtmlHelper helper, RenderNode node, JsonElement properties,
        string prefix)
    {
        var typeName = node.TypeName;
        var label = node.Name;

        if (!CanRender(typeName) || IsArray(typeName))
            return null;

        var existingProperty = properties.GetNullableProperty(label);
        var existingValue = existingProperty?.GetString();
        var showLabel = prefix == null;
        prefix = prefix == null ? label : $"{prefix}[{label}]";

        var elementContainer = GetPropertyContainer();
        var elementHtml = await GetProperty(helper, typeName, label, prefix, node.Attributes, existingValue, showLabel);

        if (showLabel)
        {
            var enabledValue = existingProperty.HasValue
                ? properties.GetProperty($"{_checkboxPrefix}{prefix}").GetString()
                : "true";
            var enabledHtml = GetEnabledProperty(prefix, enabledValue);
            elementContainer.InnerHtml.AppendHtml(enabledHtml);
        }


        elementContainer.InnerHtml.AppendHtml(elementHtml);
        return elementContainer;
    }

    private async Task<IHtmlContent> GetProperty(IHtmlHelper helper, string typeName, string label, string name,
        AttributeItem[] attributes, string existingValue, bool showLabel)
    {
        IHtmlContentBuilder elementHtml = new HtmlContentBuilder();

        if (showLabel)
            elementHtml.AppendHtml(GetLabel(name, label));

        var render = await _renderers[typeName].RenderAsync(helper, name, existingValue, attributes);
        elementHtml.AppendHtml(render);

        return elementHtml;
    }

    private IHtmlContent GetLabel(string name, string label)
    {
        var tagBuilder = new TagBuilder("label");
        var elementId = TagBuilder.CreateSanitizedId(name, "-");
        label = label.Split('(')[0];

        tagBuilder.Attributes["for"] = elementId;

        tagBuilder.InnerHtml.Append(label.BreakUpString());

        return tagBuilder;
    }

    private TagBuilder GetPropertyContainer()
    {
        var elementContainer = new TagBuilder("div");
        elementContainer.AddCssClass("form-group");
        return elementContainer;
    }

    private IHtmlContent GetEnabledProperty(string name, string existingValue, bool addMargin = true,
        bool isForList = false)
    {
        var tagBuilder = new TagBuilder("input");
        if (addMargin)
            tagBuilder.AddCssClass("mr-2");
        tagBuilder.Attributes["type"] = "checkbox";
        tagBuilder.Attributes["name"] = isForList ? $"{name}[][{_checkboxPrefix}]" : $"{_checkboxPrefix}{name}";
        tagBuilder.Attributes["value"] = "true";
        if (existingValue == "true")
            tagBuilder.Attributes["checked"] = "checked";

        tagBuilder.Attributes["data-dynamic-input"] = null;

        return tagBuilder;
    }

    #endregion

    #region TableHeader

    private IHtmlContent GetTableHeader(List<RenderNode> nodes)
    {
        var tr = new TagBuilder("tr");

        //Adding first #
        tr.InnerHtml.AppendHtml(GetHeaderSecondaryCol("#"));
        tr.InnerHtml.AppendHtml(GetHeaderSecondaryCol());

        foreach (var node in nodes)
        {
            var typeName = node.TypeName;
            var name = node.Name;

            name = string.Create(name.Length, name, (chars, state) =>
            {
                state.AsSpan().CopyTo(chars); // No slicing to save some CPU cycles
                chars[0] = char.ToUpper(chars[0]);
            });

            var th = new TagBuilder("th");

            th.InnerHtml.Append(name.BreakUpString());
            tr.InnerHtml.AppendHtml(th);
        }

        //Add btn
        tr.InnerHtml.AppendHtml(GetHeaderAddButton());

        return tr;
    }

    private IHtmlContent GetHeaderSecondaryCol(string text = "")
    {
        var thash = new TagBuilder("th");
        thash.InnerHtml.Append(text);
        thash.AddCssClass("table-secondary");
        thash.Attributes.Add("width", "25");

        return thash;
    }

    private IHtmlContent GetHeaderAddButton()
    {
        var th = new TagBuilder("th");
        th.InnerHtml.AppendHtml(
            $"<button data-row-id='{Guid.NewGuid()}' type='button' class='btn btn-secondary btn-sm add-row'><i class='fa fa-plus'></i></button>");
        th.Attributes.Add("width", "25");

        return th;
    }

    #endregion

    #region TableRows

    private async Task<IHtmlContentBuilder> GetTableRows(List<RenderNode> nodes, JsonElement props, IHtmlHelper helper,
        string prefix = null)
    {
        IHtmlContentBuilder bodyHtml = new HtmlContentBuilder();

        var i = 0;
        var existingPropsCount =
            props.ValueKind == JsonValueKind.Undefined ? 0 :
            props.ValueKind == JsonValueKind.Array ? props.EnumerateArray().Count() :
            props.EnumerateObject().Count();

        //always add at least a row
        do
        {
            var tr = new TagBuilder("tr");
            var nodeProps = props.ValueKind == JsonValueKind.Undefined ? new JsonElement() : props[i];
            var enabledValue = nodeProps.GetNullableProperty(_checkboxPrefix)?.GetString() ?? "true";

            //Adding number col
            tr.InnerHtml.AppendHtml(GetNumberCol(i + 1));
            tr.InnerHtml.AppendHtml(GetEnabledCol(prefix, enabledValue));

            foreach (var node in nodes)
            {
                var typeName = node.TypeName;
                var name = node.Name;

                var td = new TagBuilder("td");

                var _prefix = $"{prefix}[]";

                var render = await RenderProperties(helper, node, nodeProps, _prefix);
                if (render != null)
                    td.InnerHtml.AppendHtml(render);

                tr.InnerHtml.AppendHtml(td);
            }

            bodyHtml.AppendHtml(tr);
            i++;


            tr.InnerHtml.AppendHtml(GetDeleteCol());
        } while (i < existingPropsCount);


        return bodyHtml;
    }

    private IHtmlContent GetNumberCol(int index)
    {
        var td = new TagBuilder("td");
        td.InnerHtml.Append(index.ToString());
        td.AddCssClass("table-secondary");

        return td;
    }

    private IHtmlContent GetEnabledCol(string name, string existingValue)
    {
        var td = new TagBuilder("td");
        var enabledHtml = GetEnabledProperty(name, existingValue, false, true);
        td.InnerHtml.AppendHtml(enabledHtml);
        td.AddCssClass("table-secondary");

        return td;
    }

    private IHtmlContent GetDeleteCol()
    {
        var td = new TagBuilder("td");
        td.InnerHtml.AppendHtml(
            "<button type='button' class='btn btn-danger btn-sm delete-row'><i class='fa fa-trash'></i></button>");

        return td;
    }

    #endregion
}