﻿using System;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace MrCMS.FileProviders
{
    public class EmbeddedViewFileProvider : IFileProvider
    {
        private readonly string _prefix;
        private const string Views = "/Views";
        private readonly CaseInsensitiveEmbeddedFileProvider _internalProvider;
        public EmbeddedViewFileProvider(Assembly assembly, string prefix = null)
        {
            _prefix = prefix;
            _internalProvider = new CaseInsensitiveEmbeddedFileProvider(assembly);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            //if (!subpath.StartsWith(Views))
            //    return new NotFoundFileInfo(subpath);

            var fileInfo = _internalProvider.GetFileInfo(subpath);
            if (fileInfo is NotFoundFileInfo // isn't found
                && !string.IsNullOrWhiteSpace(_prefix) // and has prefix
                && subpath.StartsWith(_prefix, StringComparison.InvariantCultureIgnoreCase)) // and prefix matches
            {
                fileInfo = _internalProvider.GetFileInfo(subpath.Substring(_prefix.Length));
            }
            return fileInfo;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            //if (!subpath.StartsWith(Views))
            //    return new NotFoundDirectoryContents();

            var directoryContents = _internalProvider.GetDirectoryContents(subpath);
            if (directoryContents is NotFoundDirectoryContents // isn't found
                && !string.IsNullOrWhiteSpace(_prefix) // and has prefix
                && subpath.StartsWith(_prefix, StringComparison.InvariantCultureIgnoreCase)) // and prefix matches
            {
                directoryContents = _internalProvider.GetDirectoryContents(subpath.Substring(_prefix.Length));
            }
            return directoryContents;
        }

        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }
}