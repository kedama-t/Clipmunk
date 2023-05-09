using System.Xml.Serialization;

namespace Clipmunk
{
    public class SettingsForm : Form
    {
        private FolderBrowserDialog folderBrowserDialog;
        private TextBox saveFolderPathTextBox;
        private Button browseButton;
        private ComboBox imageFormatComboBox;
        private CheckBox showSuccessNotificationCheckBox;
        private TrackBar compressionLevelTrackBar;
        private Button saveButton;
        private Button cancelButton;
        private AppSettings appSettings;

        public SettingsForm(AppSettings appSettings)
        {
            this.appSettings = appSettings;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // FolderBrowserDialog
            folderBrowserDialog = new FolderBrowserDialog();

            // SaveFolderPathLabel
            var saveFolderPathLabel = new Label
            {
                Location = new Point(12, 15),
                Text = "保存フォルダ:",
                AutoSize = true
            };

            // SaveFolderPathTextBox
            saveFolderPathTextBox = new TextBox
            {
                Location = new Point(100, 12),
                Size = new Size(230, 20),
                Text = appSettings.SaveFolderPath
            };

            // BrowseButton
            browseButton = new Button
            {
                Location = new Point(335, 12),
                AutoSize = true,
                Text = "フォルダを選択…",
            };
            browseButton.Click += BrowseButton_Click;

            // ImageFormatLabel
            var imageFormatLabel = new Label
            {
                Location = new Point(12, 50),
                Text = "保存フォーマット:",
                AutoSize = true
            };

            // ImageFormatComboBox
            imageFormatComboBox = new ComboBox
            {
                Location = new Point(100, 47),
                AutoSize = true,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            imageFormatComboBox.Items.AddRange(Enum.GetNames(typeof(AppSettings.Format)));
            imageFormatComboBox.SelectedIndex = (int)appSettings.ImageFormat;

            // CompressionLevelLabel
            var compressionLevelLabel = new Label
            {
                Location = new Point(12, 85),
                Text = "圧縮率:",
                AutoSize = true
            };

            // CompressionLevelTrackBar
            compressionLevelTrackBar = new TrackBar
            {
                Location = new Point(100, 85),
                Size = new Size(230, 45),
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 10,
                Value = appSettings.CompressionLevel
            };

            // ShowSuccessNotificationCheckBox
            showSuccessNotificationCheckBox = new CheckBox
            {
                Text = "保存成功時に通知を表示",
                AutoSize = true,
                Location = new Point(12, 140),
                Checked = appSettings.ShowSuccessNotification
            };

            // SaveButton
            saveButton = new Button
            {
                Location = new Point(200, 180),
                Size = new Size(75, 23),
                Text = "保存",
            };
            saveButton.Click += SaveButton_Click;

            // CancelButton
            cancelButton = new Button
            {
                Location = new Point(280, 180),
                Size = new Size(75, 23),
                Text = "キャンセル",
            };
            cancelButton.Click += CancelButton_Click;

            // Form settings
            ClientSize = new Size(450, 215);
            Text = "設定";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            // Add controls to the form
            Controls.Add(saveFolderPathLabel);
            Controls.Add(saveFolderPathTextBox);
            Controls.Add(browseButton);
            Controls.Add(imageFormatLabel);
            Controls.Add(imageFormatComboBox);
            Controls.Add(compressionLevelLabel);
            Controls.Add(compressionLevelTrackBar);
            Controls.Add(showSuccessNotificationCheckBox);
            Controls.Add(saveButton);
            Controls.Add(cancelButton);
        }



        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                saveFolderPathTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            appSettings.SaveFolderPath = saveFolderPathTextBox.Text;
            appSettings.ImageFormat = (AppSettings.Format)imageFormatComboBox.SelectedIndex;
            appSettings.CompressionLevel = compressionLevelTrackBar.Value;
            appSettings.ShowSuccessNotification = showSuccessNotificationCheckBox.Checked;

            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
            using (StreamWriter writer = new StreamWriter("settings.xml"))
            {
                serializer.Serialize(writer, appSettings);
            }

            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
