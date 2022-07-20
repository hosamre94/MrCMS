﻿using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.Documents.Metadata
{
    public class RedirectMetadata : WebpageMetadataMap<Redirect>
    {
        public override string IconClass => "ti ti-switch-3";

        public override int DisplayOrder => 6;

        //public override string EditPartialView { get { return "RedirectEdit"; } }
        public override bool HasBodyContent => false;

        public override string WebGetController => "Redirect";
    }
}