using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZRepacker
{
    class XNBContent
    {
        public static List<string> MainReaders = new List<string>();

        public List<string> ReaderNames = new List<string>();

        public int ResourceCount;

        public static XNBContent FromData(byte[] data) {
            XNBContent content = new XNBContent();

            using var xnbStream = new MemoryStream(data);
            using var xnbReader = new BinaryReader(xnbStream, Encoding.UTF8, false);


            int readerCount = xnbReader.Read7BitEncodedInt();

            for(var i = 0; i < readerCount; i++)
            {
                string readerName = xnbReader.ReadString();
                int readerVersion = xnbReader.ReadInt32();
                var rawName = readerName.Split(',')[0].Split('`')[0];

                content.ReaderNames.Add(readerName);
            }

            // shared resource count plus primary asset
            int resourceCount = xnbReader.Read7BitEncodedInt()+1;

            content.ResourceCount = resourceCount;


            int resourceTypeID = xnbReader.Read7BitEncodedInt();
            string mainReaderName = content.ReaderNames[resourceTypeID - 1];
            var rawMainName = mainReaderName.Split(',')[0].Split('`')[0];

            if (!MainReaders.Contains(rawMainName)) MainReaders.Add(rawMainName);

            //for (var i = 0; i < resourceCount; i++)
            //{
            //    // here read different content resources
            //    int resourceTypeID = xnbReader.Read7BitEncodedInt();
            //}

            return content;
        }
    }
}
