using MessagePack;
using System.Reflection;
using System.Text;

var assembly = Assembly.Load("Queen.Protocols");
var types = assembly.GetTypes();

StringBuilder sb = new StringBuilder();
sb.AppendLine("using System;\r\nusing System.Collections.Generic;\r\n\r\nnamespace Queen.Protocols.Common\r\n{\r\n    public partial class ProtoPack\r\n    {\r\n        /// <summary>\r\n        /// 协议号定义\r\n        /// </summary>\r\n        private static Dictionary<ushort, Type> messageDict = new()\r\n        {");
uint id = 10001;
foreach (var type in types)
{
    if (null == type.GetInterface("INetMessage") || null == type.GetCustomAttribute<MessagePackObjectAttribute>()) continue;
    sb.AppendLine($"            {{ {id}, typeof({type.FullName})}},");
    id++;
}
sb.AppendLine("        };\r\n    }\r\n}\r\n");
var code = sb.ToString();
var index = AppDomain.CurrentDomain.BaseDirectory.IndexOf("Queen.Protocols.Gen");
var path = AppDomain.CurrentDomain.BaseDirectory.Substring(0, index);
File.WriteAllText($"{path}/Queen.Protocols/Common/__PROTO__DEFINE__.cs", code);

Console.WriteLine("Queen.Protocols.Gen Finised.");
