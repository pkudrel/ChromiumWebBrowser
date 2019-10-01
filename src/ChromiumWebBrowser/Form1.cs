﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using ChromiumWebBrowser.Features.Chromium.Controls;
using ChromiumWebBrowser.Features.Chromium.Handlers;
using ChromiumWebBrowser.Features.Chromium.Others;

namespace ChromiumWebBrowser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Browser = new AdvanceChromiumWebBrowser("www.google.com")
            {
                Dock = DockStyle.Fill
            };


            Browser.RequestHandler = new ExampleRequestHandler();
            Browser.LifeSpanHandler = new LifeSpanHandler();

            splitContainer1.Panel2.Controls.Add(Browser);
        }

        public AdvanceChromiumWebBrowser Browser { get; set; }


        private void InitForm()
        {
            splitContainer1.SplitterDistance = 60;
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