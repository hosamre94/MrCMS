﻿using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.Documents.Metadata
{
    public class TagPageContainerMetadata : WebpageMetadataMap<TagPageContainer>
    {
        public override string WebGetController => "TagPageContainer";
        public override string IconClass => "ti ti-list-details";
        public override ChildrenListType ChildrenListType => ChildrenListType.WhiteList;
        public override IEnumerable<Type> ChildrenList
        {
            get
            {
                yield return typeof(TagPage);
            }
        }
        public override SortBy SortBy => SortBy.PublishedOnDesc;
        public override bool RevealInNavigation => false;
    }
}