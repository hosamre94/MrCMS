using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MrCMS.Web.Apps.DynamicWidget.Helper;
using MrCMS.Web.Apps.DynamicWidget.Models;
using StackExchange.Profiling;

namespace MrCMS.Web.Apps.DynamicWidget.Core;

public class DynamicWidgetParser : IDynamicWidgetParser
{
    private static readonly Regex ShortcodeMatcher =
        new(@"\[([\w-_]+)([^\]]*)?\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ShortcodeSplitMatcher =
        new(@"(\[[\w-_]+[^\]]*?\])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly string _checkboxPrefix = "CBI_";


    private readonly Dictionary<string, IDynamicWidgetPropertyParser> _parsers;

    public DynamicWidgetParser(IEnumerable<IDynamicWidgetPropertyParser> parsers)
    {
        _parsers = parsers.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
    }

    public bool CanParse(string typeName)
    {
        if (string.Equals(typeName, "array", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(typeName, "array-end", StringComparison.OrdinalIgnoreCase))
            return false;

        return _parsers.ContainsKey(typeName);
    }

    public async Task<IHtmlContent> ParseAsync(IHtmlHelper helper, string text, string props)
    {
        using (MiniProfiler.Current.Step("DynamicWidget"))
        {
            if (string.IsNullOrEmpty(props))
                return new HtmlString(text);

            var properties = JToken.Parse(props);
            var matches = ShortcodeSplitMatcher.Split(text);

            var tree = BuildPropertiesTree(matches, false);
            var result = await ParseShortcodes(helper, tree, properties);
            return new HtmlString(result);
        }
    }


    #region Tree

    private List<ParseNode> BuildPropertiesTree(string[] matchCollection, bool isArray)
    {
        var index = 0;
        return GetChildren(matchCollection, ref index, true);
    }


    public List<ParseNode> GetChildren(string[] matchCollection, ref int i, bool topLevel)
    {
        var tree = new List<ParseNode>();
        for (; i < matchCollection.Length; i++)
        {
            var text = matchCollection[i];

            //skip html parts
            if (text.StartsWith('[') && text.EndsWith(']'))
            {
                var match = ShortcodeMatcher.Match(text);
                if (match.Groups.Count != 3)
                    continue;

                var typeName = match.Groups[1].Value;
                if (!topLevel && string.Equals(typeName, "array-end", StringComparison.OrdinalIgnoreCase))
                    return tree;

                var attributes = AttributesHelper.GetAttributes(match.Groups[2].Value);
                if (attributes.Length == 0)
                    continue;

                var nameValueTuple = attributes.FirstOrDefault(x => x.Key == "name");
                if (string.IsNullOrEmpty(nameValueTuple.Value))
                    continue;

                var node = new ParseNode(nameValueTuple.Value, typeName, attributes);
                if (string.Equals(typeName, "array", StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    node.Children = GetChildren(matchCollection, ref i, false);
                }

                tree.Add(node);
            }
            else
            {
                tree.Add(new ParseNode(text));
            }
        }

        if (!topLevel && matchCollection.Length == i)
            throw new Exception($"array dose not have a closing tag");

        return tree;
    }

    #endregion

    #region Parsing

    private async Task<string> ParseShortcodes(IHtmlHelper helper, List<ParseNode> tree, JToken properties)
    {
        var stb = new StringBuilder();
        for (var i = 0; i < tree.Count; i++)
        {
            var node = tree[i];
            if (node.Text != null)
                stb.Append(node.Text);
            else if (node.Children != null && node.Children.Any())
                stb.Append(await ParseArray(helper, node, properties));
            else if (node.Name != null && node.TypeName != null)
                stb.Append(await ParseProperty(helper, node, node.Attributes, properties));
        }

        return stb.ToString();
    }

    private async Task<string> ParseArray(IHtmlHelper helper, ParseNode node, JToken props = null)
    {
        if (props == null)
            return string.Empty;

        var name = node.Name;

        var _props = props[name];
        if (!(_props is JArray _array))
            return string.Empty;

        if (props.Value<string>(_checkboxPrefix) == "false")
            return string.Empty;

        var sb = new StringBuilder(_array.Count);
        foreach (var item in _array)
            sb.Append(await ParseShortcodes(helper, node.Children, item));

        return sb.ToString();
    }

    private async Task<string> ParseProperty(IHtmlHelper helper, ParseNode node, AttributeItem[] attributes,
        JToken props = null)
    {
        if (props == null)
            return string.Empty;

        var typeName = node.TypeName;
        if (!CanParse(typeName))
            return string.Empty;

        var name = node.Name;
        if (props[name] == null)
            return string.Empty;

        if (props.Value<string>($"{_checkboxPrefix}{name}") == "false")
            return string.Empty;

        return await _parsers[typeName].ParseAsync(helper, name, props.Value<string>(name), attributes);
    }

    #endregion
}