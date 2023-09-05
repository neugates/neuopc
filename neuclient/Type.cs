using System;
using System.ComponentModel;
using neulib;

namespace neuclient
{
    public enum ServerStatus
    {
        Disconnected = 0,
        Connected = 1
    }

    public class ReadItem
    {
        public object Value { get; set; }

        [DefaultValue(Quality.Uncertain)]
        public Quality Quality { get; set; }

        public DateTime SourceTimestamp { get; set; }

        public DateTime ServerTimestamp { get; set; }
    }

    public class ReadItem<T>
    {
        public T Value { get; set; }

        [DefaultValue(Quality.Uncertain)]
        public Quality Quality { get; set; }

        public DateTime SourceTimestamp { get; set; }

        public DateTime ServerTimestamp { get; set; }
    }

    public class Node
    {
        public string Name { get; set; }

        public string ItemName { get; set; }

        public string ItemPath { get; set; }

        public bool IsItem { get; set; }

        public Type Type { get; set; }

        public ReadItem Item { get; set; }
    }
}
