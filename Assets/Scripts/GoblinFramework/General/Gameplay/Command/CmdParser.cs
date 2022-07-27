using GoblinFramework.Core;
using GoblinFramework.General.Gameplay.Command.Cmds;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General.Gameplay.Command
{
    public class CmdParser : Comp<GGEngine>
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

        public byte[] SyncPosCmdToBytes(SyncPosCmd cmd)
        {
            var writer = BinaryRW.GenWriter();

            writer.Write(cmd.actorId);
            writer.Write(cmd.dire);
            writer.Write(cmd.x);
            writer.Write(cmd.y);
            writer.Write(cmd.z);
            var bytes = BinaryRW.ToArray(writer.BaseStream);

            BinaryRW.EndWriter(writer);

            return bytes;
        }

        public SyncPosCmd BytesToSyncPosCmd(byte[] bytes)
        {
            var reader = BinaryRW.GenReader(bytes);

            var cmd = new SyncPosCmd();
            cmd.actorId = reader.ReadInt32();
            cmd.dire = reader.ReadInt32();
            cmd.x = reader.ReadInt32();
            cmd.y = reader.ReadInt32();
            cmd.z = reader.ReadInt32();

            BinaryRW.EndReader(reader);

            return cmd;
        }
    }
}
