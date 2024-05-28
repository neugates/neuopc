using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Opc.Da;

namespace DaClient
{
    public class DaBrowse
    {
        public static IEnumerable<Node> AllNode(
            Server server,
            Opc.ItemIdentifier id = null,
            List<Node> nodes = null
        )
        {
            if (null == nodes)
            {
                nodes = new List<Node>();
            }

            BrowseElement[] elements;
            var filters = new BrowseFilters { BrowseFilter = browseFilter.all };

            try
            {
                elements = server.Browse(id, filters, out BrowsePosition position);
            }
            catch (Exception)
            {
                throw;
            }

            if (null != elements)
            {
                var list = elements
                    .Select(
                        x =>
                            new Node
                            {
                                Name = x.Name,
                                ItemName = x.ItemName,
                                ItemPath = x.ItemPath,
                                IsItem = x.IsItem
                            }
                    )
                    .ToList();
                nodes.AddRange(list);

                foreach (var element in elements)
                {
                    if (element.HasChildren)
                    {
                        id = new Opc.ItemIdentifier(element.ItemPath, element.ItemName);
                        _ = DaBrowse.AllNode(server, id, nodes);
                    }
                }
            }

            return nodes;
        }

        public static Type GetDataType(Server server, string tag, string path)
        {
            //var item = new Item { ItemName = tag, ItemPath = path };
            var item = new Item { ItemName = tag };
            ItemProperty result;

            try
            {
                var propertyCollection = server.GetProperties(
                    new Opc.ItemIdentifier[] { item },
                    new[] { new PropertyID(1) },
                    true
                )[0];
                result = propertyCollection[0];
            }
            catch (Exception)
            {
                throw;
            }

            return (Type)result.Value;
        }
    }
}
