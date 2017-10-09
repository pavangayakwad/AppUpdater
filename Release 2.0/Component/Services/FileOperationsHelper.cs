using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;


namespace Srushti.Updates
{
    internal class FileTable
    {
        public FileTable(string originalFilePath, string backupFilePath)
        {
            OriginalFilePath = originalFilePath;
            BackedUpFilePath = backupFilePath;
        }
        public string OriginalFilePath { get; set; }
        public string BackedUpFilePath { get; set; }
    }
    
    internal static class FileOperationsHelper
    {
        private static string tempBackUpFolder;
        private static List<FileTable> backedUpFiles = new List<FileTable>();

        /// <summary>
        /// Creates folder in current directory.
        /// </summary>
        /// <param name="folderName">Name of the folder to create</param>
        /// <param name="DeleteAndCreate">If true, delete existing 
        /// and create new empty one.</param>
        public static void CreateFolder(string folderName, bool DeleteAndCreate)
        {
            if (DeleteAndCreate) DeleteFolder(folderName);
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
        }

        /// <summary>
        /// Delted directory and sub-directories.
        /// </summary>
        /// <param name="folderName">Folder to delete.</param>
        public static void DeleteFolder(string folderName)
        {
            if (Directory.Exists(folderName))
                Directory.Delete(folderName, true);
        }


        /// <summary>
        /// Check update files read from update configuration XML are
        /// currently available for download. if not, exception will be
        /// raised.
        /// </summary>
        [DebuggerNonUserCodeAttribute()]
        public static void CheckUpdateFilesAvailability(List<UpdateFile> files)
        {
            foreach (UpdateFile item in files)
            {
                string source = item.SourceURI + item.FileName;
                if (NetUtils.CheckOnlineFileExist(source) == false)
                    throw new Exception("One of update files not available for download. Update aborted!", new Exception(source));
            }
        }

        /// <summary>
        /// This method should be called after update files
        /// download from online update server.
        /// </summary>
        /// <param name="postUpdateFileFullPath">Downloaded location of post-update assembly path</param>
        /// <param name="preUpdateFileFullPath">Downloaded location of pre-update assembly path</param>
        public static void CheckPrePostFilesAvailability(string postUpdateFileFullPath, string preUpdateFileFullPath)
        {
            if (!string.IsNullOrEmpty(preUpdateFileFullPath))
            {
                if (!File.Exists(preUpdateFileFullPath))
                    throw new Exception("Pre-update file not downloaded.");
            }

            if (!string.IsNullOrEmpty(postUpdateFileFullPath))
            {
                if (!File.Exists(postUpdateFileFullPath))
                    throw new Exception("Post-update file not downloaded.");
            }
        }

        /// <summary>
        /// Overwrite or copy updated files to the 
        /// application folder to receive updates it run next time.
        /// </summary>
        public static void OverwriteOldFiles(List<UpdateFile> files, string tempFolder)
        {
            string sourcFileFullPath;
            string targetFileFullPath;
            string fileToUpdate;
            foreach (UpdateFile file in files)
            {
                if (file.DestinationFolder.ToLower() != "{ignore}")
                {
                    sourcFileFullPath = Environment.CurrentDirectory + "\\" + tempFolder + "\\" + file.DestinationFolder + "\\" + file.FileName;
                    if (string.IsNullOrEmpty(file.RenameFileTo))
                        fileToUpdate = file.FileName;
                    else
                        fileToUpdate = file.RenameFileTo;

                    targetFileFullPath = Environment.CurrentDirectory + "\\" +
                        file.DestinationFolder + "\\" + fileToUpdate;

                    //create backup
                    CreateBackUp(file.DestinationFolder, fileToUpdate);

                    //No folder will be created if it is all ready exist.
                    FileOperationsHelper.CreateFolder(Environment.CurrentDirectory + "\\" + file.DestinationFolder, false);
                    //now, overwrite the file.
                    File.Copy(sourcFileFullPath, targetFileFullPath, true);
                }
            }
        }

        /// <summary>
        /// Back up a file before replace it with the updated one.
        /// </summary>
        /// <param name="destinationFolder">Folder of a file to be updated.</param>
        /// <param name="fileName">File to backup.</param>
        private static void CreateBackUp(string destinationFolder, string fileName)
        {
            string existingFile = Environment.CurrentDirectory + "\\" + destinationFolder
                + "\\" + fileName;
            //do nothing if there is no file to overwrite by update process.
            if (!File.Exists(existingFile)) return;

            //create unique backup folder
            if(string.IsNullOrEmpty(tempBackUpFolder))
                tempBackUpFolder = Guid.NewGuid().ToString();

            //Have backup folder location handy.
            string backupLocation = tempBackUpFolder + "\\" + destinationFolder;
            string fileToBackup = backupLocation + "\\" + fileName;

            //Follow original folder structure in backup folder too.
            if (!string.IsNullOrEmpty(destinationFolder))
                FileOperationsHelper.CreateFolder
                    (backupLocation,false);
            
            //Create back up of original file
            File.Copy(existingFile, fileToBackup, true);

            //keep track what is backed-up from where.
            backedUpFiles.Add(new FileTable(existingFile,fileToBackup));
        }

        /// <summary>
        /// Restore old file.
        /// </summary>
        public static void RevertBackOverwriteOperation()
        {
            foreach (FileTable file in backedUpFiles)
            {
                File.Copy(file.BackedUpFilePath, file.OriginalFilePath, true);
            }
            backedUpFiles.Clear();
        }

        /// <summary>
        /// Delete backedup folder.
        /// </summary>
        public static void DeleteBackupFolder()
        {
            DeleteFolder(tempBackUpFolder);
            tempBackUpFolder = string.Empty;
        }
    }
}
