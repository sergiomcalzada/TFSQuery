using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSQueryLib
{
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    public class TFSQueryRunner
    {
        private readonly Uri serverUri;

        public TFSQueryRunner(Uri serverUri)
        {
            this.serverUri = serverUri;
        }

        public IList<Changeset> GetChangeset(string user)
        {
            try
            {
                var tfs = new TfsTeamProjectCollection(serverUri);
                var svc = tfs.GetService<VersionControlServer>();

                VersionSpec fromDateVersion = new DateVersionSpec(DateTime.Now.AddDays(-1));

                VersionSpec toDateVersion = new DateVersionSpec(DateTime.Now);


                var changeset = svc.QueryHistory("$/", VersionSpec.Latest, 0, RecursionType.Full, null, fromDateVersion, toDateVersion, int.MaxValue, true, true).OfType<Changeset>().ToList();

                return changeset;
            }
            catch (Exception ex)
            {
                return new List<Changeset>();
            }
        }

        public IList<Changeset> GetChangeset(string user, DateTime? dateFrom, DateTime? dateTo)
        {
            if (string.IsNullOrEmpty(user)) user = null;
            var from = dateFrom.HasValue ? dateFrom.Value : DateTime.Now.AddDays(-1);
            var to = dateTo.HasValue ? dateTo.Value : DateTime.Now;
            try
            {
                var tfs = new TfsTeamProjectCollection(serverUri);
                var svc = tfs.GetService<VersionControlServer>();

                VersionSpec fromDateVersion = new DateVersionSpec(from);

                VersionSpec toDateVersion = new DateVersionSpec(to);


                var changeset = svc.QueryHistory("$/", VersionSpec.Latest, 0, RecursionType.Full, user, fromDateVersion, toDateVersion, int.MaxValue, true, true).OfType<Changeset>().ToList();

                return changeset;
            }
            catch (Exception ex)
            {
                return new List<Changeset>();
            }
        }

        public WorkItem GetWorkItemById(int id)
        {
            var tfs = new TfsTeamProjectCollection(serverUri);
            var svc = tfs.GetService<WorkItemStore>();

            var item = svc.GetWorkItem(id);

            return item;
        }
    }
}
