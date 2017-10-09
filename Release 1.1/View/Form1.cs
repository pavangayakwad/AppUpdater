using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Srushti.Updates
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ObjectHostHelper o = new ObjectHostHelper();
            //o.RunIt();

            //TestMethods t = new TestMethods();
            //t.Test();

            //NetUtils.UpdateDownloadProgressChangeEvent += new NetUtils.AppUpdateDownloadPrograessChange(NetUtils_UpdateDownloadProgressChangeEvent);
            //NetUtils.UpdateDownloadCompletedEvent += new NetUtils.AppUpdateDownloadCompleted(NetUtils_UpdateDownloadCompletedEvent);
            //NetUtils.DownloadFile("http://www.martinhunter.co.nz/articles/", "C:\\temp\\", "MVPC.pdf");
        }

        void NetUtils_UpdateDownloadCompletedEvent(AsyncCompletedEventArgs e)
        {
            label1.Text = "Done";
        }

        void NetUtils_UpdateDownloadProgressChangeEvent(System.Net.DownloadProgressChangedEventArgs e)
        {
            label1.Text = e.BytesReceived.ToString() + " - " + e.ProgressPercentage.ToString();
        }
    }
}
