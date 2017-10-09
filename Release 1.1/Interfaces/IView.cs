using System;
using System.Collections.Generic;
using System.Text;

namespace Srushti.Updates.Interfaces
{
    public interface IView
    {

        /// <summary>
        /// Product and update information.
        /// </summary>
        Product ProductInformation { set; }

        /// <summary>
        /// Files to download and apply for software updates.
        /// </summary>
        UpdateFile UpdateFileInformation { set; }

        /// <summary>
        /// Progress in percentage.
        /// </summary>
        int ProgressPercentage { set; }

        /// <summary>
        /// Total bytes downloaded.
        /// </summary>
        long BytesReceived { set; }

        /// <summary>
        /// Totla size of currently downloading file.
        /// </summary>
        long TotalBytesToReceive { set; }

        
    }
}
