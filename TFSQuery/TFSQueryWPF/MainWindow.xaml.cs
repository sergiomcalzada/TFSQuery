using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TFSQueryWPF
{
    using System.Data;

    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    using TFSQueryLib;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TFSQueryRunner tfs;


        public MainWindow()
        {
            InitializeComponent();
            tfs = new TFSQueryRunner(new Uri(ConfigurationManager.AppSettings["tfsServer"]));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dgChanges = this.GetChangeset();
            this.dgChangeSets.ItemsSource = dgChanges;
        }

        private IEnumerable<WorkItemModel> GetChangeset()
        {
            var changes = this.tfs.GetChangeset(txtUser.Text, dpFrom.SelectedDate, dpTo.SelectedDate);
            var workItems = GetWorkItems(changes);
            var wis = workItems.Select(x => x.ToModel(changes)).ToList();

            return wis;
        }

        private IEnumerable<WorkItem> GetWorkItems(IEnumerable<Changeset> changes)
        {
            var chageSetWorkItems = changes.SelectMany(x => x.WorkItems);
            var pbiWorkItems = this.GetPBI_Items(chageSetWorkItems);
            return pbiWorkItems.OrderBy(x => x.Id);
        }

        private IEnumerable<WorkItem> GetPBI_Items(IEnumerable<WorkItem> items)
        {
            var parentList = new List<WorkItem>();
            var childList = new List<WorkItem>();
            foreach (var workItem in items)
            {
                if (workItem.Type.Name == "Task")
                {
                    var parentLink = workItem.WorkItemLinks.Cast<WorkItemLink>().FirstOrDefault(x => x.LinkTypeEnd.Name == "Parent");
                    if (parentLink != null)
                    {
                        var parentWorkItem = tfs.GetWorkItemById(parentLink.TargetId);
                        childList.Add(parentWorkItem);
                    }
                    
                }
                else
                {
                    parentList.Add(workItem); 
                }
            }
            if (childList.Any())
            {
                var childResult = this.GetPBI_Items(childList);
                parentList.AddRange(childResult);
            }
            var result = parentList.Distinct(new WorkItemEqualityComparer());
            return result;
        }

        private void dgChangeSets_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                var grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    var dgr = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem);
                    dgr.DetailsVisibility = GetVisivility(dgr.DetailsVisibility);
                }
            }
        }

        private Visibility GetVisivility(Visibility visibility)
        {
            if (visibility == Visibility.Visible) return Visibility.Collapsed;
            return Visibility.Visible;
        }
    }
}
