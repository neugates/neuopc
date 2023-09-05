using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuopc
{
    internal class MetaInfo
    {
        public static string Name = "NeuOPC";

        public static int Major = 0;
        public static int Minor = 3;
        public static int Patch = 1;

        public static string Version
        {
            get
            {
                return $"{Major}.{Minor}.{Patch}";
            }
        }

        public static string Description = "NeuOPC is an application that converts OPC DA to OPC UA. \r\n"
            + "It is intended to provide support for bridging OPC DA for Neuron projects and can also be used for other OPC UA clients. \r\n"
            + "NeuOPC is open sourced under the GPLv2, please ensure that the source code is available to users when distributing this application. \r\n";

        public static string Documenation = "https://neugates.io/docs/en/latest/configuration/south-devices/opc-da/overview.html";

        public static string License = "https://github.com/neugates/neuopc/blob/main/LICENSE";

        public static string NeuopcProject = "https://github.com/neugates/neuopc";
        public static string NeuronProject = "https://github.com/emqx/neuron";

        public static string OpcdaProject = "https://opcfoundation.org/";
        public static string OpcuaProject = "https://github.com/OPCFoundation/UA-.NETStandard";
        public static string SerilogProject = "https://github.com/serilog/serilog";

        public static string Disclaimer = "The following binaries belong to the OPC Foundation. You must become a registered user in order to use them:\r\n"
            + "Opc.Ua.Configuration.dll\r\n"
            + "Opc.Ua.Core.dll\r\n"
            + "Opc.Ua.Security.Certificates.dll\r\n"
            + "Opc.Ua.Server.dll\r\n"
            + "OpcComRcw.dll\r\n"
            + "OpcNetApi.Com.dll\r\n"
            + "OpcNetApi.dll\r\n"
            + "You must agree to the terms and condition exposed on the OPC Foundation website. NeuOPC is not responsible of their usage and cannot be held responsible.";
    }
}
