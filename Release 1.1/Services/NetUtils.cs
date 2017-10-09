using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Srushti.Updates
{
    /// <summary>
    /// Performs online file transfer operations.
    /// </summary>
    public static class NetUtils
    {
        /// <summary>
        /// Tries to connect a website. Returns result depending 
        /// on the HTTP respose code.
        /// </summary>
        /// <returns>True if machine is connected to internet</returns>
        public static bool IsConnectedToInternet()
        {
            return CheckOnlineFileExist("http://www.google.com/");
        }

        /// <summary>
        /// Query for a file available online for download
        /// </summary>
        /// <param name="completeURL">file location on remote update server</param>
        /// <returns>True if file available for download</returns>
        public static bool CheckOnlineFileExist(string completeURL)
        {
            HttpWebRequest req;
            HttpWebResponse resp;
            bool returnStatus = false;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(completeURL);
                req.Timeout = 20000;
                resp = (HttpWebResponse)req.GetResponse();

                if (resp.StatusCode.ToString().Equals("OK"))
                    returnStatus = true;   //"Present";
                else
                    returnStatus = false;   //"Unable to connect to internet.";
                
                resp.Close();
                req = null;
            }
            catch
            {
                returnStatus = false;
            }
            
            return returnStatus;
        }

        /// <summary>
        /// Download file available on web.
        /// </summary>
        /// <param name="downloadFromURI">Only domina information, 
        /// Ex:http://www.srushtisoft.com/updates/" </param>
        /// <param name="copyToFolder">Target folder on user machine 
        /// where the downloading file to be stored.</param>
        /// <param name="fileName">The filename to download from web.</param>
        public static void DownloadFile(string downloadFromURI, string copyToFolder, string fileName)
        {
            try
            {
                Uri uri = new Uri(downloadFromURI +  fileName);
                
                //"Downloading updates please wait...";
                System.Net.WebClient webClient = new System.Net.WebClient();
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
                webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
                webClient.DownloadFileAsync(uri, copyToFolder + "\\" + fileName, fileName);
                webClient.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Download progress events

        public delegate void AppUpdateDownloadPrograessChange(DownloadProgressChangedEventArgs e);
        public static event AppUpdateDownloadPrograessChange UpdateDownloadProgressChangeEvent;

        public delegate void AppUpdateDownloadCompleted(System.ComponentModel.AsyncCompletedEventArgs e);
        public static event AppUpdateDownloadCompleted UpdateDownloadCompletedEvent;

        private static void RaiseDownloadCompletedEvent(System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (UpdateDownloadCompletedEvent != null)
                UpdateDownloadCompletedEvent(e);
        }
        static void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            RaiseDownloadCompletedEvent(e); 
        }

        private static void RaiseDownloadProgress(DownloadProgressChangedEventArgs e)
        {
            if (UpdateDownloadProgressChangeEvent != null)
                UpdateDownloadProgressChangeEvent(e);
        }
        static void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            RaiseDownloadProgress(e);
        }
        #endregion
    }
}
