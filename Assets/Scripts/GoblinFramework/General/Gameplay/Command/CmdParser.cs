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
            writer.Write(Fixed64Vector3ToBytes(cmd.pos));
            var bytes = BinaryRW.ToArray(writer.BaseStream);

            BinaryRW.EndWriter(writer);

            return bytes;
        }

        public SyncPosCmd BytesToSyncPosCmd(byte[] bytes)
        {
            var reader = BinaryRW.GenReader(bytes);

            var cmd = new SyncPosCmd();
            cmd.actorId = reader.ReadInt32();
            cmd.pos = new Fixed64Vector3(reader.ReadInt64(), reader.ReadInt64(), reader.ReadInt64());

            BinaryRW.EndReader(reader);

            return cmd;
        }

        public byte[] Fixed64Vector2ToBytes(Fixed64Vector2 vec2)
        {
            var writer = BinaryRW.GenWriter();

            writer.Write(vec2.x.RawValue);
            writer.Write(vec2.y.RawValue);
            var bytes = BinaryRW.ToArray(writer.BaseStream);

            BinaryRW.EndWriter(writer);

            return bytes;
        }

        public byte[] Fixed64Vector3ToBytes(Fixed64Vector3 vec3)
        {
            var writer = BinaryRW.GenWriter();

            writer.Write(vec3.x.RawValue);
            writer.Write(vec3.y.RawValue);
            writer.Write(vec3.z.RawValue);
            var bytes = BinaryRW.ToArray(writer.BaseStream);

            BinaryRW.EndWriter(writer);

            return bytes;
        }
    }
}
