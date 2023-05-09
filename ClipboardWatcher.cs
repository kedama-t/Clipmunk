using System.Runtime.InteropServices;

namespace Clipmunk
{
  public class ClipboardWatcher : NativeWindow, IDisposable
    {
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        private DateTime lastEventRaisedAt = DateTime.MinValue;
        private TimeSpan debounceTime = TimeSpan.FromMilliseconds(500); // デバウンス時間を500ミリ秒に設定

        public event EventHandler ClipboardImageChanged;

        public ClipboardWatcher()
        {
            CreateHandle(new CreateParams());
            AddClipboardFormatListener(Handle);
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE && Clipboard.ContainsImage())
            {
                DateTime now = DateTime.Now;
                if (now - lastEventRaisedAt > debounceTime) // 前回のイベントからデバウンス時間が経過しているかチェック
                {
                    ClipboardImageChanged?.Invoke(this, EventArgs.Empty);
                    lastEventRaisedAt = now; // イベント発生時刻を更新
                }
            }
            base.WndProc(ref m);
        }

        public void Dispose()
        {
            RemoveClipboardFormatListener(Handle);
            DestroyHandle();
        }
    }
}
