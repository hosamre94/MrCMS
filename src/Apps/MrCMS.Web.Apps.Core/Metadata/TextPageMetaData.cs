using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Metadata
{
    public class TextPageMetaData : WebpageMetadataMap<TextPage>
    {
        public override string IconClass => "ti ti-book-2";
        public override string WebGetController => "TextPage";
    }
}