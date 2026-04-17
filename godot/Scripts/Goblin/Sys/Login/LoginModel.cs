using Goblin.Sys.Common;

namespace Goblin.Sys.Login
{
    public class LoginModel : Model<LoginProxy>
    {
        public string uuid { get; set; }
        public bool signined { get; set; }
    }
}
