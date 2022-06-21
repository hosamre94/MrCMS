using MrCMS.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MrCMS.Web.Apps.DynamicWidget.Models;

namespace MrCMS.Web.Apps.DynamicWidget.Core;

public class MediaSelectorDynamicWidgetParser : IDynamicWidgetPropertyParser
{
    public string Name => "media";

    public async Task<string> ParseAsync(IHtmlHelper helper, string name, string existingValue,
        AttributeItem[] attributes = null)
    {
        var classes = (string)null;
        var width = 0;
        var height = 0;
        if (attributes != null)
            foreach (var attr in attributes)
            {
                if (attr.Key.Equals("width"))
                    int.TryParse(attr.Value, out width);

                if (attr.Key.Equals("height"))
                    int.TryParse(attr.Value, out height);

                if (attr.Key.Equals("class"))
                    classes = attr.Value;
            }

        var size = default(Size);
        if (width > 0)
            size = new Size(width, width);

        if (height > 0)
            size.Height = height;

        using (var writer = new StringWriter())
        {
            (await helper.RenderImage(existingValue, size, attributes: new { @class = classes }))
                .WriteTo(writer, HtmlEncoder.Default);

            return writer.ToString();
        }
    }
}