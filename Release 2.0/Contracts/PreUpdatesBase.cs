using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Srushti.Updates.Contracts
{
    public class PreUpdatesBase : MarshalByRefObject
    {
        /// <summary>
        /// Implement logic to execute after 
        /// downloading and overwriting all old files 
        /// with updated ones.
        /// 
        /// Throw unhandled exceptions to perform 
        /// update rollback.
        /// </summary>
        public virtual void PerformPreUpdateActivities() { }

        /// <summary>
        /// Any unhandled exception occured during 
        /// PerformPostUpdateActivities() will abort its
        /// further execution. AppUpdater calls Rollback()
        /// to revert back incomplete post updates.
        /// </summary>
        public virtual void Rollback() { }
    }
}
