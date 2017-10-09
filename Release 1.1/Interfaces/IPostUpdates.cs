using System;
using System.Collections.Generic;
using System.Text;

namespace Srushti.Updates.Interfaces
{
    public abstract class CPostUpdates : MarshalByRefObject
    {
        public virtual void PerformUpdates() { }
        public virtual void Rollback() { }
    }

    public interface IPostUpdates
    {
        /// <summary>
        /// Implement logic to execute after 
        /// downloading and overwriting all old files 
        /// with updated ones.
        /// 
        /// Throw unhandled exceptions to perform 
        /// update rollback.
        /// </summary>
        void PerformPostUpdateActivities();

        /// <summary>
        /// Any unhandled exception occured during 
        /// PerformPostUpdateActivities() will abort its
        /// further execution. AppUpdater calls Rollback()
        /// to revert back incomplete post updates.
        /// </summary>
        void Rollback();
    }
}
