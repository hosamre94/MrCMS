using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.Documents.Metadata
{
    public class SitemapPlaceholderMetadata : WebpageMetadataMap<SitemapPlaceholder>
    {
        public override string IconClass => "ti ti-switch-3";
        public override int DisplayOrder => 99;
        public override bool HasBodyContent => false;

        public override string WebGetController => "SitemapPlaceholder";
    }
}