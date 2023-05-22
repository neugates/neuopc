using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace neulib
{
    public class Serializer
    {
        public static byte[] Serialize<T>(T t) where T : class
        {
            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, t);
            stream.Flush();
            return stream.GetBuffer();
        }

        public static T Deserialize<T>(byte[] input) where T : class {
            using var stream = new MemoryStream(input);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream) as T;
        }
    }
}
