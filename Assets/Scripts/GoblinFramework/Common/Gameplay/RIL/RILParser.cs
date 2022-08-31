using GoblinFramework.Core;
using GoblinFramework.Common.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Common.Gameplay.RIL
{
    public class RILParser : Comp
    {
        public BinaryRW BinaryRW;

        protected override void OnCreate()
        {
            base.OnCreate();

            // 二进制读写
            BinaryRW = AddComp<BinaryRW>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public byte[] RILPosToBytes(RILPos rilPos)
        {
            var writer = BinaryRW.GenWriter();

            writer.Write(rilPos.actorId);
            writer.Write(rilPos.dire);
            writer.Write(rilPos.x);
            writer.Write(rilPos.y);
            writer.Write(rilPos.z);
            var bytes = BinaryRW.ToArray(writer.BaseStream);

            BinaryRW.EndWriter(writer);

            return bytes;
        }

        public RILPos BytesToRILPos(byte[] bytes)
        {
            var reader = BinaryRW.GenReader(bytes);

            var rilPos = new RILPos();
            rilPos.actorId = reader.ReadInt32();
            rilPos.dire = reader.ReadInt32();
            rilPos.x = reader.ReadInt32();
            rilPos.y = reader.ReadInt32();
            rilPos.z = reader.ReadInt32();

            BinaryRW.EndReader(reader);

            return rilPos;
        }
    }
}
