using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Srushti.Updates.Contracts;
using Srushti.Updates;
using System.Configuration;

namespace Wpf_Updater
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, IView
    {
        public Window1()
        {
            InitializeComponent();
        }

        #region IView Members

        public Product ProductInformation
        {
            set { lblProduct.Content = value.ProductURL; }
        }

        private UpdateFile _updateInfo = new UpdateFile();
        BitmapImage img = new BitmapImage();
        public UpdateFile UpdateFileInformation
        {
            set
            {
                lblProduct.Content = value.FileName;
                //if (string.IsNullOrEmpty(_updateInfo.MoreInfoURL) && _updateInfo.MoreInfoURL != value.MoreInfoURL)
                //{
                //    img.UriSource = new Uri(value.MoreInfoURL);
                //    //image1.Source = BitmapImage.UriSourceProperty.
                //}
            }
        }

        public int ProgressPercentage
        {
            set { progressBar1.Value = value; }
        }

        public long BytesReceived
        {
            set {  }
        }

        public long TotalBytesToReceive
        {
            set {  }
        }

        #endregion

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Srushti.Updates.AppUpdatePresenter presenter =
                new AppUpdatePresenter(this,
                    ConfigurationManager.AppSettings["LocalProductConfigCacheFilePath"],
                "http://srushtisoft.com/updates/demo/LatestProductUpdateInfo.xml");
            presenter.UpdateMessageEvents += new EventHandler(presenter_UpdateMessageEvents);
            presenter.InitiateUpdateActivities();
            
        }

        void presenter_UpdateMessageEvents(object sender, EventArgs e)
        {
            UpdatesEventArgs u = e as UpdatesEventArgs;
            lblStatus.Content = u.Message;
        }
    }
}
