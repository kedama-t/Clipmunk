namespace Clipmunk
{
  [Serializable]
    public class AppSettings
    {
        public enum Format
        {
            Bmp,
            Jpg,
            Png
        }

        public string SaveFolderPath { get; set; }
        public Format ImageFormat { get; set; }
        public int CompressionLevel { get; set; }
        public bool ShowSuccessNotification { get; set; }

        public AppSettings()
        {
            SaveFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "ClipboardImages");
            ImageFormat = Format.Png;
            CompressionLevel = 75;
            ShowSuccessNotification = true;
        }
    }
}
