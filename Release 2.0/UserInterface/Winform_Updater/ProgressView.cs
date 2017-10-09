using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Srushti.Updates;
using System.Configuration;

namespace Winform_Updater
{
    public partial class ProgressView : Form, Srushti.Updates.Contracts.IView
    {
        public ProgressView()
        {
            InitializeComponent();
        }

        #region IView Members

        public Product ProductInformation
        {
            set
            {
                if (value != null)
                {
                    lblAppVersion.Text = value.AppVersion;
                    lblUpdateVersion.Text = value.UpdateVersion;
                    lnkInfo.Text = value.ProductURL;
                    webBrowser1.Url = new Uri(value.ProductURL);
                }
                else
                    lblAppVersion.Text = string.Empty;

                lblAppVersion.Refresh();
            }
            
        }

        private UpdateFile _updateFileInformation = new UpdateFile();
        public UpdateFile UpdateFileInformation
        {
            set
            {
                if (value != null)
                {
                    if (value.MoreInfoURL != _updateFileInformation.MoreInfoURL)
                    {
                        if (string.IsNullOrEmpty(value.MoreInfoURL))
                            webBrowser1.Url = null;
                        else
                            webBrowser1.Url = new Uri(value.MoreInfoURL);

                        
                    }
                    _updateFileInformation = value;
                    lblFile.Text = _updateFileInformation.FileName;
                    lblShortInfo.Text = _updateFileInformation.ShortDescription;
                }
                else
                    lblFile.Text = string.Empty;
                
                lblFile.Refresh();
            }
        }

        public int ProgressPercentage
        {
            set
            {
                label3.Text = value.ToString() + "%";
                label3.Refresh();
                progressBar1.Value = value;
            }
        }

        public long BytesReceived
        {
            set
            {
                lblStatus.Text = value.ToString() + " Bytes";
                lblStatus.Refresh();
            }
        }

        public long TotalBytesToReceive
        {
            set
            {
                lblSize.Text = value.ToString() + " Bytes Total";
                lblSize.Refresh();
            }
        }

        #endregion

        AppUpdatePresenter presenter;
        private void ProgressView_Load(object sender, EventArgs e)
        {
            presenter = new AppUpdatePresenter(this,
                ConfigurationManager.AppSettings["LocalProductConfigCacheFilePath"],
                "http://srushtisoft.com/updates/demo/LatestProductUpdateInfo.xml");
            presenter.UpdateMessageEvents += new EventHandler(presenter_UpdateMessageEvents);
            presenter.DownloadsCompleted += new EventHandler(presenter_AllFilesDownloadCompleted);
        }

        void presenter_AllFilesDownloadCompleted(object sender, EventArgs e)
        {
            lblAppVersion.Text = string.Empty;
            lblFile.Text = string.Empty;
            label3.Text = string.Empty;
            lblStatus.Text = string.Empty;
            lblSize.Text = string.Empty;
            lblUpdateInfo.Text = string.Empty;
            button1.Enabled = true;
            lblShortInfo.Text = string.Empty;
        }

        void presenter_UpdateMessageEvents(object sender, EventArgs e)
        {
            UpdatesEventArgs ue = e as UpdatesEventArgs;
            if (ue.InfoType == MessageType.Error)
            {
                lblError.Text = ue.Message;
                button1.Enabled = true;
                //label6.Visible = false;
            }
            else
                lblUpdateInfo.Text = ue.Message;
            
            lblUpdateInfo.Refresh();
            lblError.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            lblError.Visible = lblUpdateInfo.Visible = true;
            lblError.Text = lblUpdateInfo.Text = string.Empty;
            presenter.InitiateUpdateActivities();
            
        }

        private void lnkInfo_Click(object sender, EventArgs e)
        {
            Process.Start(this.lnkInfo.Text);
        }
    }
}
