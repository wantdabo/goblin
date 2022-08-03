using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General.Gameplay.RIL
{
    public class BinaryRW : Comp<GGEngine>
    {
        public byte[] ToArray(Stream stream) 
        {
            return (stream as MemoryStream).ToArray();
        }

        public BinaryReader GenReader(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            BinaryReader reader = new BinaryReader(ms);

            return reader;
        }

        public void EndReader(BinaryReader reader)
        {
            reader.Close();
            reader.Dispose();
        }

        public BinaryWriter GenWriter()
        {
            BinaryWriter writer = new BinaryWriter(new MemoryStream());

            return writer;
        }

        public void EndWriter(BinaryWriter writer)
        {
            writer.Close();
            writer.Dispose();
        }

    }
}
