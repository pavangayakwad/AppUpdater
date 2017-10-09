using System;
using System.Collections.Generic;
using System.Text;

namespace Srushti.Updates.Interfaces
{
    public interface IPreUpdates
    {
        /// <summary>
        /// Implement logic to execute before 
        /// downloading update files from update 
        /// server.
        /// 
        /// Throw unhandled exceptions to stop
        /// update operations to continue.
        /// </summary>
        void PerformPreUpdateActivities();

        /// <summary>
        /// Any unhandled exception occured during 
        /// PerformPreUpdateActivities() will abort its
        /// further execution. AppUpdater calls Rollback()
        /// to revert back incomplete pre-updates.
        /// </summary>
        void Rollback();
    }
}
