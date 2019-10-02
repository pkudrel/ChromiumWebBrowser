using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using ChromiumWebBrowser.Features.Chromium.Controls;

namespace ChromiumWebBrowser.Features.MainFormView.Views
{
    public partial class MainForm : Form
    {
        public MainForm(Func<string, AdvanceChromiumWebBrowser> browserFn)
        {
            InitializeComponent();

            // Browser = new AdvanceChromiumWebBrowser("www.google.com")

            Browser = browserFn("http://test.hornetpk.lan/");
            splitContainer1.Panel2.Controls.Add(Browser);
        }

        public AdvanceChromiumWebBrowser Browser { get; set; }

        private void InitForm()
        {
            AutoScaleMode = AutoScaleMode.Font;
            double factor = CreateGraphics().DpiX / 96f;
            splitContainer1.SplitterDistance = (int) Math.Round(60 * factor);
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            Browser.AddressChanged += OnAddressChanged;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitForm();
        }

        private void OnAddressChanged(object sender, AddressChangedEventArgs e)
        {
            this.InvokeEx(x => { AddressTextBox.Text = e.Address; });
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Browser.AddressChanged -= OnAddressChanged;
        }

        private async void AddressTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await NavigateToTextBoxAddress();
        }

        private async Task NavigateToTextBoxAddress()
        {
            var address = AddressTextBox.Text;
            await Browser.Navigate(address, CancellationToken.None);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await NavigateToTextBoxAddress();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Browser.ShowDevTools();
        }
    }
}