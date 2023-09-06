using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Opc.Da;
using System.Reflection.Metadata.Ecma335;

namespace neuclient
{
    public class DaBrowse
    {
        private static IEnumerable<Node> AllNode(Server server, Opc.ItemIdentifier id = null, List<Node> nodes = null)
        {
            if (null == nodes)
            {
                nodes = new List<Node>();
            }

            BrowseElement[] elements;
            var filters = new BrowseFilters
            {
                BrowseFilter = browseFilter.all
            };

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
                var list = elements.Select(x => new Node
                {
                    Name = x.Name,
                    ItemName = x.ItemName,
                    ItemPath = x.ItemPath,
                    IsItem = x.IsItem
                }).ToList();
                nodes.AddRange(list);

                foreach (var element in elements)
                {
                    if (element.HasChildren)
                    {
                        id = new Opc.ItemIdentifier(element.ItemPath, element.ItemName);
                        AllNode(server, id, nodes);
                    }
                }
            }

            return nodes;
        }

        public static Type GetDataType(Server server, string tag, string path)
        {
            var item = new Item { ItemName = tag, ItemPath = path };
            ItemProperty result;

            try
            {
                var propertyCollection = server.GetProperties(new Opc.ItemIdentifier[] { item }, new[] { new PropertyID(1) }, true)[0];
                result = propertyCollection[0];
            }
            catch (Exception)
            {
                //System.Diagnostics.Debug.WriteLine($"GetProperties Exception {tag}");
                return null;
            }

            return (Type)result.Value;
        }



        public static IEnumerable<Node> AllItemNode(Server server)
        {
            var nodes = AllNode(server);
            var items = nodes.Where(x => x.IsItem);
            foreach (var item in items)
            {
                item.Type = GetDataType(server, item.ItemName, item.ItemPath);
            }

            return items;
        }
    }
}
