using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Opc.Da;

namespace neuclient
{
    public class DaBrowse
    {
        public static IEnumerable<Node> AllNode(Server server)
        {
            var nodes = new List<Node>();
            BrowseElement[] elements;
            var filters = new BrowseFilters
            {
                BrowseFilter = browseFilter.all
            };
            try
            {
                elements = server.Browse(null, filters, out BrowsePosition position);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (null != elements)
            {
                elements.Select(x => new Node
                {
                    Name = x.Name,
                    ItemName = x.ItemName,
                    ItemPath = x.ItemPath,
                    IsItem = x.IsItem,
                    Type = x.GetType()
                });
            }

            return nodes;
        }
    }
}
