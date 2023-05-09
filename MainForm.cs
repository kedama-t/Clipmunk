using System.Diagnostics;
using System.Drawing.Imaging;
using System.Reflection;
using System.Xml.Serialization;

namespace Clipmunk
{
    public class MainForm : Form
    {
        readonly string APP_NAME = "Clipmunk";
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem openFolderToolStripMenuItem;
        private ToolStripMenuItem settingsMenuItem;
        private ToolStripMenuItem exitMenuItem;
        private ClipboardWatcher clipboardWatcher;
        private AppSettings appSettings;

        public MainForm()
        {
            ShowInTaskbar = false;
            Visible = false;
            WindowState = FormWindowState.Minimized;

            InitializeComponents();

            LoadSettings();

            clipboardWatcher = new ClipboardWatcher();
            clipboardWatcher.ClipboardImageChanged += ClipboardWatcher_ClipboardImageChanged;
        }


        private void InitializeComponents()
        {

            // ContextMenuStrip
            contextMenuStrip = new ContextMenuStrip();

            openFolderToolStripMenuItem = new ToolStripMenuItem
            {
                Text = "保存先フォルダを開く"
            };
            openFolderToolStripMenuItem.Click += OpenFolderToolStripMenuItem_Click;
            contextMenuStrip.Items.Add(openFolderToolStripMenuItem);

            // SettingsMenuItem
            settingsMenuItem = new ToolStripMenuItem
            {
                Text = "設定...",
            };
            settingsMenuItem.Click += SettingsMenuItem_Click;
            contextMenuStrip.Items.Add(settingsMenuItem);

            // ExitMenuItem
            exitMenuItem = new ToolStripMenuItem
            {
                Text = "終了",
            };
            exitMenuItem.Click += ExitMenuItem_Click;
            contextMenuStrip.Items.Add(exitMenuItem);
            
            // NotifyIcon
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Clipmunk.icon.ico");
            Icon icon = SystemIcons.Application;
            if (stream != null)
            {
                icon = new Icon(stream);
            }
            notifyIcon = new NotifyIcon
            {
                Icon = icon,
                Text = APP_NAME,
                ContextMenuStrip = contextMenuStrip,
                Visible = true,
            };
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

        }

        private void LoadSettings()
        {
            if (File.Exists("settings.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                using (StreamReader reader = new StreamReader("settings.xml"))
                {
                    appSettings = (AppSettings)serializer.Deserialize(reader);
                }
            }
            else
            {
                appSettings = new AppSettings();
                ShowSettingsForm();
            }
        }

        private void ShowSettingsForm()
        {
            SettingsForm settingsForm = new SettingsForm(appSettings);
            settingsForm.ShowDialog();
        }

        private void ClipboardWatcher_ClipboardImageChanged(object sender, EventArgs e)
        {
            Image image = Clipboard.GetImage();
            if (image != null)
            {
                SaveImage(image);
            }
        }

        private void SaveImage(Image image)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
                string extension = appSettings.ImageFormat.ToString().ToLower();
                string fileName = $"{timestamp}.{extension}";
                string filePath = Path.Combine(appSettings.SaveFolderPath, fileName);

                ImageFormat imageFormat;
                switch (appSettings.ImageFormat)
                {
                    case AppSettings.Format.Bmp:
                        imageFormat = ImageFormat.Bmp;
                        break;
                    case AppSettings.Format.Jpg:
                        imageFormat = ImageFormat.Jpeg;
                        break;
                    case AppSettings.Format.Png:
                    default:
                        imageFormat = ImageFormat.Png;
                        break;
                }

                if (appSettings.ImageFormat == AppSettings.Format.Jpg || appSettings.ImageFormat == AppSettings.Format.Png)
                {
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, appSettings.CompressionLevel);
                    ImageCodecInfo codecInfo = GetEncoderInfo(imageFormat);
                    image.Save(filePath, codecInfo, encoderParameters);
                }
                else
                {
                    image.Save(filePath, imageFormat);
                }
                if (appSettings.ShowSuccessNotification)
                {
                    ShowNotification("画像が正常に保存されました。", ToolTipIcon.Info);
                }
            }
            catch (Exception ex)
            {
                // 例外が発生した場合、エラーメッセージを表示
                ShowNotification($"画像の保存に失敗しました。エラー: {ex.Message}", ToolTipIcon.Error);
            }
        }

        private void ShowNotification(string message, ToolTipIcon icon)
        {
            notifyIcon.ShowBalloonTip(3000, APP_NAME, message, icon);
        }

        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            OpenFolder();
        }

        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            ShowSettingsForm();
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OpenFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFolder();
        }

        private void OpenFolder() {
            if (Directory.Exists(appSettings.SaveFolderPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = appSettings.SaveFolderPath,
                    UseShellExecute = true,
                    Verb = "open"
                }); ;
            }
            else
            {
                MessageBox.Show("保存先フォルダが存在しません。設定を確認してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
