using ThemeForge.Themes.Dialogs;

namespace ThemeForge.Themes
{
    public static class DialogManager
    {
        public static string ShowOpenFileDialog()
        {
            var dialog = new CustomOpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                return dialog.SelectedFilePath;
            }
            return null;
        }

        public static string ShowSaveFileDialog()
        {
            var dialog = new CustomSaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                return dialog.FilePath;
            }
            return null;
        }
    }
}

