namespace TFSQueryWPF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    public class WorkItemModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string State { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<Changeset> ChangesSets { get; set; }
        public WorkItemType Type { get; set; }

        public string TypeString
        {
            get
            {
                return this.Type.Name;
            }
        }

        public DateTime? FirsCommitDate { get; set; }
        public DateTime? LastCommitDate { get; set; }
        public String Feature { get; set; }
    }


    public static class WorkItemModelExtensions
    {
        const int RoadMapFeature = 10046;

        public static WorkItemModel ToModel(this WorkItem workItem, IList<Changeset> changes)
        {
            var feature = workItem.Fields.TryGetById(RoadMapFeature);
            var changesSets = GetChangesSets(workItem, changes);


            return new WorkItemModel
            {
                Id = workItem.Id,
                Title = workItem.Title,
                State = workItem.State,
                CreatedDate = workItem.CreatedDate,
                Type = workItem.Type,
                Feature = feature != null ? feature.Value.ToString() : "",

                ChangesSets = changesSets,
                FirsCommitDate = changesSets != null && changesSets.Any() ? changesSets.First().CreationDate : (DateTime?)null,
                LastCommitDate = changesSets != null && changesSets.Any() ? changesSets.Last().CreationDate : (DateTime?)null,
            };
        }

        private static IList<Changeset> GetChangesSets(WorkItem workItem, IList<Changeset> changes)
        {
            var directCheckIn = changes.Where(change => change.WorkItems.Any(c => c.Id == workItem.Id)).OrderBy(x => x.CreationDate).ToList();
            if (directCheckIn.Any())
                return directCheckIn;

            var childLinks = workItem.WorkItemLinks.Cast<WorkItemLink>().Where(x => x.LinkTypeEnd.Name == "Child");
            var childCheckIn = changes.Where(change => change.WorkItems.Any(wi => childLinks.Any(link => link.TargetId == wi.Id))).ToList();
            if (childCheckIn.Any())
                return childCheckIn;

            return null;
        }
    }
}