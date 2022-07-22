namespace MrCMS.Web.Admin.Helpers
{
    public static class MediaExtensions
    {
        private static string _fileTypesPath = @"\Areas\Admin\Content\img\fileTypes";

        public static string GetFileImage(this string extension) => @$"{_fileTypesPath}\\{GetFileName(extension)}.svg";

        public static string GetFileName(this string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                return "";
            
            switch (extension.ToLower())
            {
                case ".doc":
                case ".docx":
                    return "word";
                case "xls":
                case ".xlsx":
                case ".csv":
                    return "excel";
                case ".pdf":
                    return "pdf";
                case ".txt":
                    return "txt";
                case ".zip":
                case ".rar":
                case ".7z":
                    return "zip";
                case ".ppt":
                case ".pptx":
                    return "ppt";
                case ".mov":
                case ".flv":
                case ".mp4":
                case ".wmv":
                case ".avi":
                case ".mpeg":
                case ".webm":
                case ".ogv":
                    return "video";
                case ".mp3":
                    return "music";
                case ".svg":
                    return "vector";
                case ".htm":
                case ".html":
                    return "php";
            }

            return "file";
        }
    }
}