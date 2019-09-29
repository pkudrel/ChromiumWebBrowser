using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChromiumWebBrowser.Features.Chromium.Controls;

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

            tabPage1.Controls.Add(Browser);
        }

        public AdvanceChromiumWebBrowser Browser { get; set; }
    }
}
