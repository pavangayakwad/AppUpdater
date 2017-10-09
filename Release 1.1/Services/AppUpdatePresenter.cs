using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using System.ComponentModel;
using Srushti.Updates.Interfaces;
using System.Threading;

namespace Srushti.Updates
{
    /// <summary>
    /// Downloads updated file from the server
    /// along with managing user interface (UI)
    /// </summary>
    public class AppUpdatePresenter
    {
        /// <summary>
        /// UI contract with which this presenter is most comfirtable. 
        /// </summary>
        private IView window = null;

        private AppUpdatePresenterHelper helper;
        
        /// <summary>
        /// When download of *all* update files from update server is finished,
        /// DownloadsCompleted is raised. 
        /// </summary>
        public event EventHandler DownloadsCompleted;

        /// <summary>
        /// Most frequently raised event to post download status to UI. 
        /// This event provides information of each step taking place in the background 
        /// when updater is running.
        /// </summary>
        public event EventHandler UpdateMessageEvents;
        
        #region Event raise methods
        private void RaiseUpdateMessageEvents(string message, MessageType infoType)
        {
            if (UpdateMessageEvents != null)
            {
                UpdatesEventArgs e = new UpdatesEventArgs();
                e.Message = message;
                e.InfoType = infoType;
                UpdateMessageEvents(this, e);
            }
        }

        private void RaiseAllFilesDownloadCompleted()
        {
            if (DownloadsCompleted != null)
                DownloadsCompleted(this, null);
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view">Instance of winform implementing IView</param>
        public AppUpdatePresenter(IView view)
        {
            window = view;
            helper = new AppUpdatePresenterHelper();
            helper.ErrorOccuredEvent += new System.IO.ErrorEventHandler(helper_ErrorOccuredEvent);
        }

        /// <summary>
        /// Will be raised when presenter helper class experience exceptions during 
        /// the download process.
        /// </summary>
        /// <param name="sender">most of the time AppUpdatePresenterHelper</param>
        /// <param name="e">Exception instance</param>
        void helper_ErrorOccuredEvent(object sender, System.IO.ErrorEventArgs e)
        {
            RaiseUpdateMessageEvents(e.GetException().Message, MessageType.Error);
        }
   
        /// <summary>
        /// The entry method to start update process also
        /// a trigger method for UI to manually invoke 
        /// update operation.
        /// </summary>
        public void InitiateUpdateActivities()
        {
            try
            {
                //is computer connected to internet?
                if (NetUtils.IsConnectedToInternet() == false)
                {
                    RaiseUpdateMessageEvents("Unable to connect internet.", MessageType.Error);
                    return;
                }

                RaiseUpdateMessageEvents("Checking new updates..", MessageType.Info);
                //Is current software require updates?
                if (helper.IsUpdateRequired())
                {
                    window.ProductInformation = helper.LatestProductConfig;
                    RaiseUpdateMessageEvents("Retrieving update files.", MessageType.Info);
                    //Get list of update files to download.
                    if (helper.GetUpdateFilesList())
                    {
                        RaiseUpdateMessageEvents("Validating update files availability.", MessageType.Info);
                        //event to capture validate check are finished.
                        //Doing this, we are keeping UI responsive for view related actions.
                        helper.UpdateFilesCheckCompleted += new EventHandler(helper_UpdateFilesCheckCompleted);
                        
                        /*
                         * Update files check runs on different thread and when
                         * it is finished, the UpdateFilesCheckCompleted event is
                         * called. Event handler for UpdateFilesCheckCompleted event
                         * i.e, helper_UpdateFilesCheckCompleted is then
                         * invoked which will in turn start continued method [StartFilesDownload()]
                         * for downloading update files.
                        */
                        helper.CheckUpdateFilesAvailability();

                        
                    }
                }
                else
                    RaiseUpdateMessageEvents("No updates available at this time.", MessageType.Success);
            }
            catch (Exception ex)
            {
                RaiseUpdateMessageEvents("Error occured: " + ex.Message, MessageType.Error);
            }
        }

        /// <summary>
        /// Event handler to start continued operations of
        /// update process.
        /// </summary>
        /// <param name="sender">file checker component</param>
        /// <param name="e">none</param>
        void helper_UpdateFilesCheckCompleted(object sender, EventArgs e)
        {
            StartFilesDownload();
        }

        #region UI update event handlers
        void helper_FileDownloadStarting(UpdateFile file)
        {
            window.UpdateFileInformation = file;
            window.BytesReceived = 0;
            window.ProgressPercentage = 0;
            window.TotalBytesToReceive = 0;
        }

        void NetUtils_UpdateDownloadProgressChangeEvent(System.Net.DownloadProgressChangedEventArgs e)
        {
            window.UpdateFileInformation = helper.CurrentUpdateFile;
            window.BytesReceived = e.BytesReceived;
            window.ProgressPercentage = e.ProgressPercentage;
            window.TotalBytesToReceive = e.TotalBytesToReceive;
        }
        #endregion

        private void StartFilesDownload()
        {
            //Prepare temp download folder
            helper.PrepareEnvironment();

            RaiseUpdateMessageEvents("Please wait, Downloading updates..", MessageType.Info);
            /* 
            Unsubscribe and subscribe events to avoid multiple event subscriptions
            when user attempts to repeate download operation.
             */
            NetUtils.UpdateDownloadProgressChangeEvent -= NetUtils_UpdateDownloadProgressChangeEvent;
            NetUtils.UpdateDownloadProgressChangeEvent += new NetUtils.AppUpdateDownloadPrograessChange(NetUtils_UpdateDownloadProgressChangeEvent);
            helper.FileDownloadStarting -= helper_FileDownloadStarting;
            helper.FileDownloadStarting += new AppUpdatePresenterHelper.FileDownloadStartingDelegate(helper_FileDownloadStarting);
            helper.FilesDownloadCompleted -= helper_FilesDownloadCompleted;
            helper.FilesDownloadCompleted += new EventHandler(helper_FilesDownloadCompleted);

            //Start downloading files one-by-one asynchronously.
            helper.DownloadUpdateFiles();

            //When asynchronous download finishes downloading
            //all files from the update files list, 
            //FilesDownloadCompleted event will be fired and 
            //event handler for FilesDownloadCompleted invokes
            //continued set of operation for application update.
            //Logically, ContinueAfterUpdateFilesDownload() 
            //method will be called after download of all files 
            //is successful.
        }

        /// <summary>
        /// Event handler which invokes next set of update operations.
        /// </summary>
        /// <param name="sender">AppUpdatePresenter</param>
        /// <param name="e">Not used.</param>
        void helper_FilesDownloadCompleted(object sender, EventArgs e)
        {
            //Let UI know download of files is completed.
            RaiseAllFilesDownloadCompleted();

            //start next set of update operations.
            ContinueAfterUpdateFilesDownload();
        }

        /// <summary>
        /// After downloading all update files from update server,
        /// this method performs next set of operations of software
        /// update.
        /// </summary>
        private void ContinueAfterUpdateFilesDownload()
        {
            try
            {
                //Run pre-update routines, if it fail, 
                //run rollback and abort update process.
                RaiseUpdateMessageEvents("Running pre-updates...", MessageType.Info);
                helper.RunPreUpdates();

                //Deploy update files from temp folder to target location.
                //and update prodcut information config file to store
                //update version of future checks.
                RaiseUpdateMessageEvents("Copying files..", MessageType.Info);
                helper.CopyUpdateFiles();

                //Run post-update routines, run rollback.
                RaiseUpdateMessageEvents("Running post-updates...", MessageType.Info);
                helper.RunPostUpdates();

                //Delete temp folder created for download.
                RaiseUpdateMessageEvents("Cleaning...", MessageType.Success);
                helper.PerformCleaningOperations();

                //Updates successful!
                RaiseUpdateMessageEvents("Software update successful.", MessageType.Success);
            }
            catch (Exception ex)
            {
                RaiseUpdateMessageEvents("Error: " + ex.Message, MessageType.Error);
            }
        }


        
    }
}
