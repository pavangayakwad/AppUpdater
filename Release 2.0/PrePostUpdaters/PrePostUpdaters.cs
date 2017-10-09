using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Srushti.Updates.Contracts;

namespace PrePostUpdaters
{
    public class MyAppPreUpdates : PreUpdatesBase
    {
        public override void PerformPreUpdateActivities()
        {
            base.PerformPreUpdateActivities();
        }

        public override void Rollback()
        {
            base.Rollback();
        }
    }

    public class MyAppPostUpdates : PostUpdatesBase
    {
        public override void PerformPostUpdateActivities()
        {
            base.PerformPostUpdateActivities();
        }

        public override void Rollback()
        {
            base.Rollback();
        }
    }

}
