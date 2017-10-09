using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using System.IO;
using System.Xml;
using System.Reflection;

using Srushti.Updates.Interfaces;
using System.Threading;
using System.ComponentModel;

namespace Srushti.Updates
{
    /// <summary>
    /// Helper class to perform actuall file updates.
    /// </summary>
    internal class AppUpdatePresenterHelper
    {
        //Temporary download folder created in the folder
        //where appUpdater exe is placed.
        private const string TEMP_FOLDER = "temp";
        
        //List of update files to download one-by-one.
        private UpdateFiles newUpdateFiles;

        //list of files finished downloading and ready to deploy.
        List<UpdateFile> fileTracks = new List<UpdateFile>();

        //Pre and post update DLL instances.
        IPreUpdates preUpdateInstance;
        IPostUpdates postUpdateInstance;

        public delegate void FileDownloadStartingDelegate(UpdateFile file);

        //Notify when a update file download process is about to start.
        public event FileDownloadStartingDelegate FileDownloadStarting;
        //A update file download is completed.
        public event EventHandler FilesDownloadCompleted;
        //Error occured when downloading a update file.
        public event ErrorEventHandler ErrorOccuredEvent;
        //Notify when check for availability of update files on server is done.
        public event EventHandler UpdateFilesCheckCompleted;

        #region Event raisers
        private void RaiseErrorEvent(Exception ex)
        {
            if(ErrorOccuredEvent != null)
                ErrorOccuredEvent(this, new ErrorEventArgs(ex));
        }

        private void RaiseFileDownloadStarting(UpdateFile file)
        {
            if (FileDownloadStarting != null)
                FileDownloadStarting(file);
        }
        #endregion

        /// <summary>
        /// Prodcut configuration pulled from update server.
        /// </summary>
        private Product latestProductConfig = new Product();
        public Product LatestProductConfig
        {
            get { return latestProductConfig; }
            set { latestProductConfig = value; }
        }

        /// <summary>
        /// Product configuration which was pulled from server
        /// during latest software update operation.
        /// </summary>
        private Product localProductConfig = new Product();
        public Product LocalProductConfig
        {
            get { return localProductConfig; }
            set { localProductConfig = value; }
        }

        /// <summary>
        /// This is the file which is currenlty downloading
        /// asynchronously.
        /// </summary>
        private UpdateFile updateFile = new UpdateFile();
        public UpdateFile CurrentUpdateFile
        {
            get { return updateFile; }
            set { updateFile = value; }
        }

        /// <summary>
        /// Load latest configuration from update server and 
        /// local product configuration from application path
        /// into Product objects.
        /// </summary>
        private void LoadProductConfigurations()
        {
            LocalProductConfig = ConfigurationsHelper.LoadLocalProductConfiguration();
            LatestProductConfig = ConfigurationsHelper.LoadNewProductConfiguration();
        }

        /// <summary>
        /// Compare UpdateVersion in Latest and Local product
        /// version configuration file.
        /// </summary>
        /// <returns>True, if versions in both file found differnt.</returns>
        public bool IsUpdateRequired()
        {
            bool result = true;
            try
            {
                LoadProductConfigurations();
                if (LocalProductConfig.UpdateVersion == LatestProductConfig.UpdateVersion)
                    result= false;
            }
            catch(Exception ex) 
            {
                result = false;
                if (ex.InnerException == null)
                    RaiseErrorEvent(ex);
                else
                    RaiseErrorEvent(ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// Get list of update files from update server.
        /// </summary>
        /// <returns>True if update files configuration is to 
        /// read from update server.</returns>
        public bool GetUpdateFilesList()
        {
            bool result = true;
            try
            {
                newUpdateFiles = ConfigurationsHelper.LoadUpdatesConfiguration(LatestProductConfig.UpdateFilesXMLPath);
            }
            catch(Exception ex) 
            { 
                result= false;
                RaiseErrorEvent(ex);
            }
            return result;
        }

        /// <summary>
        /// Create temp folder is not found. Delete if
        /// there was an existing temp folder.
        /// </summary>
        public void PrepareEnvironment()
        {
            FileOperationsHelper.CreateFolder(TEMP_FOLDER, true);
            NetUtils.UpdateDownloadCompletedEvent -= NetUtils_UpdateDownloadCompletedEvent;
            NetUtils.UpdateDownloadCompletedEvent += new NetUtils.AppUpdateDownloadCompleted(NetUtils_UpdateDownloadCompletedEvent);
        }

        /// <summary>
        /// Check online availability of update files 
        /// if not, exception will be raised.
        /// </summary>
        public void CheckUpdateFilesAvailability()
        {
            try
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.RunWorkerAsync();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                RaiseErrorEvent(e.Error);
                return;
            }

            if (UpdateFilesCheckCompleted != null)
                UpdateFilesCheckCompleted(this, null);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            FileOperationsHelper.CheckUpdateFilesAvailability(newUpdateFiles.Files);
        }

        /// <summary>
        /// Check pre and post files are downloaded and available.
        /// </summary>
        public void CheckPrePostFilesAvailability()
        {
            try
            {
                FileOperationsHelper.CheckPrePostFilesAvailability
                  (newUpdateFiles.PostUpdateAssemblyPath, newUpdateFiles.PreUpdateAssemblyPath);
            }
            catch (Exception ex)
            {
                RaiseErrorEvent(ex);
            }
        }

        /// <summary>
        /// Do the actual asynchronous download of update files 
        /// from the update server.
        /// </summary>
        public void DownloadUpdateFiles()
        {
            if (newUpdateFiles.Files != null && newUpdateFiles.Files.Count > 0)
            {
                UpdateFile file = newUpdateFiles.Files[0];
                this.CurrentUpdateFile = file;
                RaiseFileDownloadStarting(file);
                string destinationTempFolder = Environment.CurrentDirectory + "\\" + TEMP_FOLDER;
                try
                {
                    if (string.IsNullOrEmpty(file.DestinationFolder))
                        NetUtils.DownloadFile(file.SourceURI, destinationTempFolder, file.FileName);
                    else
                    {
                        string destinationSubFolder = destinationTempFolder + "\\" + file.DestinationFolder;
                        FileOperationsHelper.CreateFolder(destinationSubFolder, false);
                        NetUtils.DownloadFile(file.SourceURI,
                            destinationSubFolder, file.FileName);
                    }
                }
                catch (Exception ex)
                {
                    RaiseErrorEvent(ex.InnerException);
                    PerformCleaningOperations();
                    return;
                }
                fileTracks.Add(file);
                newUpdateFiles.Files.RemoveAt(0);
            }
            else if (newUpdateFiles.Files != null && newUpdateFiles.Files.Count == 0)
            {
                newUpdateFiles.Files = null;
                if (FilesDownloadCompleted != null)
                    FilesDownloadCompleted(this, null);
            }
            
        }

        /// <summary>
        /// When current file download is finished, 
        /// call DownloadUpdateFiles() method to consider
        /// next file in the update files list for download.
        /// </summary>
        /// <param name="e"></param>
        void NetUtils_UpdateDownloadCompletedEvent(System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                fileTracks.Clear();
                RaiseErrorEvent(e.Error);
            }
            else 
                DownloadUpdateFiles();
            
        }

        /// <summary>
        /// Delete temp folder and clear fils track cache
        /// used to keep track files downloaded from 
        /// update server and do the actuall update deployment
        /// on application.
        /// </summary>
        public void PerformCleaningOperations()
        {
            fileTracks.Clear();

            preUpdateInstance = null;
            postUpdateInstance = null;

            try
            {
                //For security reasons, deleting update server details.
                LatestProductConfig.UpdateFilesXMLPath = string.Empty;
                Product.Save(LatestProductConfig, ConfigurationManager.AppSettings["LocalProductConfigCacheFilePath"]);

                FileOperationsHelper.DeleteFolder(TEMP_FOLDER);
                FileOperationsHelper.DeleteBackupFolder();
            }
            catch { }//It is okay if we are not able to delete temp folders.
        }

        /// <summary>
        /// Overwrite or copy updated files to the 
        /// application folder. 
        /// </summary>
        public void CopyUpdateFiles()
        {
            try
            {
                FileOperationsHelper.OverwriteOldFiles(fileTracks, TEMP_FOLDER);
            }
            catch (Exception ex)
            {
                RaiseErrorEvent(ex);
                //throw ex;
            }
        }

        public T CreateInstance<T>(string assemblyFullPath)
        {
            Assembly a = Assembly.LoadFile(assemblyFullPath);
            return (T) a.CreateInstance(typeof(T).ToString());
        }


 
        private void TryLoadPrePostDLLs()
        {
            try
            {
                if(!string.IsNullOrEmpty(newUpdateFiles.PreUpdateAssemblyPath))
                    preUpdateInstance = CreateInstance<IPreUpdates>(newUpdateFiles.PreUpdateAssemblyPath);
                if(!string.IsNullOrEmpty(newUpdateFiles.PostUpdateAssemblyPath))
                    postUpdateInstance = CreateInstance<IPostUpdates>(newUpdateFiles.PostUpdateAssemblyPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Run IPreUpdates interface methods.
        /// </summary>
        public void RunPreUpdates()
        {
            try
            {
                if(preUpdateInstance != null)
                    preUpdateInstance.PerformPreUpdateActivities();
            }
            catch(Exception ex) 
            {
                preUpdateInstance.Rollback();
                throw new Exception("Pre update process failed!" + ex.Message, ex);
            }
        }

        /// <summary>
        /// Run IPostUpdates interface methods.
        /// </summary>
        public void RunPostUpdates()
        {
            try
            {
                if(postUpdateInstance !=null)
                    postUpdateInstance.PerformPostUpdateActivities();
            }
            catch (Exception ex)
            {
                try
                {
                    postUpdateInstance.Rollback();
                }
                catch { }
                FileOperationsHelper.RevertBackOverwriteOperation();
                FileOperationsHelper.DeleteBackupFolder();
                throw new Exception("Post update process failed!" + ex.Message, ex);
            }
        }
    }
}
