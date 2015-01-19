using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSQueryConsole
{
    using System.Collections;

    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.VersionControl.Client;

    using TFSQueryLib;

    class Program
    {
        static void Main(string[] args)
        {
            var uri = new Uri("http://it-pc4:8080/tfs/neoSON_Collection");
            var changesets = new TFSQueryRunner(uri).GetChangeset(null);
            foreach (var changeset in changesets)
            {
                foreach (var w in changeset.WorkItems)
                {
                    Console.WriteLine("WorkItemId:" + w.Id);
                    Console.WriteLine("WorkItemTitle:" + w.Title);

                } 
            }
            
            Console.ReadLine();
        }
    }
}
