using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MrCMS.Web.Admin.Infrastructure.TagHelpers;

[HtmlTargetElement("label", Attributes = ForAttributeName)]
public class LabelRequiredTagHelper : LabelTagHelper
{
    private const string ForAttributeName = "asp-for";

    public LabelRequiredTagHelper(IHtmlGenerator generator) : base(generator)
    {
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await base.ProcessAsync(context, output);

        var metadata = For.Metadata as DefaultModelMetadata;
        var hasRequiredAttribute = metadata
            ?.Attributes
            .PropertyAttributes!
            .Any(i => i.GetType() == typeof(RequiredAttribute)) ?? false;

        if (hasRequiredAttribute)
        {
            output.AddClass("required", HtmlEncoder.Default);
        }
    }
}