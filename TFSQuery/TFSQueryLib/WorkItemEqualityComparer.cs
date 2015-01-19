namespace TFSQueryLib
{
    using System.Collections.Generic;

    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    public class WorkItemEqualityComparer
        : IEqualityComparer<WorkItem>
    {
        #region Implementation of IEqualityComparer<in WorkItem>

        public bool Equals(WorkItem x, WorkItem y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(WorkItem obj)
        {
            return obj.Id.GetHashCode();
        }

        #endregion
    }
}