using System;
using System.Windows.Forms;
using ChromiumWebBrowser.Features.Downloader.Models;
using ChromiumWebBrowser.Features.Downloader.Services;

namespace ChromiumWebBrowser.Features.Downloader.View
{
    public partial class DownloadForm : Form
    {
        private readonly CefRegistry _reg;

        public DownloadForm(CefRegistry reg)
        {
            _reg = reg;
            InitializeComponent();
        }


        private void Log(string message)
        {
            LogBox.AppendText($"{message}{Environment.NewLine}");
        }


        private void Progress(int value)
        {
         
            progressBar.Value = value;
        }

        private async void DownloadForm_Load(object sender, EventArgs e)
        {
            progressBar.Maximum = 100;
            progressBar.Minimum = 0;

         
            PackageName.Text = _reg.CefSharpPackageName;
            CefSharpPath.Text = _reg.GlobalCefSharpEnvPath;
            var cefSharpEnvBuilder = new CefSharpEnvBuilder(_reg, Log, Progress);
            await cefSharpEnvBuilder.Do();
            Close();
        }
    }
}