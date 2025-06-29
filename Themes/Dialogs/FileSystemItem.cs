
using System;

namespace ThemeForge.Themes.Dialogs;

public partial class FileSystemItem
{
    // Keep only the unique property in this file
    public string FileType { get; set; }

    // Move the implementation of FormattedSize to this file if it's not in the other file
    // This method can stay here since it uses IsDirectory and Size which are defined in the other partial class
    public string FormattedSize
    {
        get
        {
            if (IsDirectory) return "";
            if (Size < 1024) return $"{Size} bytes";
            if (Size < 1024 * 1024) return $"{Size / 1024:N1} KB";
            if (Size < 1024 * 1024 * 1024) return $"{Size / (1024 * 1024):N1} MB";
            return $"{Size / (1024 * 1024 * 1024):N1} GB";
        }
    }
}