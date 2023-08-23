using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace neuclient
{
    public enum ServerStatus
    {
        Disconnected = 0,
        Connected = 1
    }

    public enum Quality
    {
        GoodLocalOverrideValueForced = 216,
        Good = 192,
        UncertainValueFromMultipleSources = 88,
        UncertainEngineeringUnitsExceeded = 84,
        UncertainSensorNotAccurate = 80,
        UncertainLastUsableValue = 68,
        Uncertain = 64,
        BadWaitingForInitialData = 32,
        BadOutOfService = 28, // Also happens when item or group is inactive
        BadCommFailure = 24,
        BadLastKnowValuePassed = 20,
        BadSensorFailure = 16,
        BadDeviceFailure = 12,
        BadNotConnected = 8,
        BadConfigurationErrorInServer = 4,
        Bad = 0,
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
    }
}
