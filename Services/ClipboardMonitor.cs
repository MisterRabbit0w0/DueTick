using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DueTick.Services
{
    public class ClipboardMonitor : IDisposable
    {
        public event EventHandler<string> ClipboardChanged;

        private IntPtr _nextClipboardViewer;
        private IntPtr _handle;

        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32.dll")]
        private static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private const int WM_DRAWCLIPBOARD = 0x0308;
        private const int WM_CHANGECBCHAIN = 0x030D;

        public ClipboardMonitor()
        {
            var form = new Form();
            _handle = form.Handle;
            form.ShowInTaskbar = false;
            form.WindowState = FormWindowState.Minimized;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Show();
            form.Hide();

            _nextClipboardViewer = SetClipboardViewer(_handle);
            Application.AddMessageFilter(new ClipboardMessageFilter(this));
        }

        private class ClipboardMessageFilter : IMessageFilter
        {
            private readonly ClipboardMonitor _monitor;

            public ClipboardMessageFilter(ClipboardMonitor monitor)
            {
                _monitor = monitor;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_DRAWCLIPBOARD)
                {
                    _monitor.OnClipboardChanged();
                    SendMessage(_monitor._nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                }
                else if (m.Msg == WM_CHANGECBCHAIN)
                {
                    if (m.WParam == _monitor._nextClipboardViewer)
                    {
                        _monitor._nextClipboardViewer = m.LParam;
                    }
                    else
                    {
                        SendMessage(_monitor._nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    }
                }
                return false;
            }
        }

        private void OnClipboardChanged()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    var text = Clipboard.GetText();
                    ClipboardChanged?.Invoke(this, text);
                }
            }
            catch
            {
                // Ignore clipboard access errors
            }
        }

        public void Dispose()
        {
            ChangeClipboardChain(_handle, _nextClipboardViewer);
        }
    }
}
