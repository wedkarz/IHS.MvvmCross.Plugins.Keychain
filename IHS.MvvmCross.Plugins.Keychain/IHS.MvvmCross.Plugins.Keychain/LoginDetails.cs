using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHS.MvvmCross.Plugins.Keychain
{
    public class LoginDetails
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string Serialize()
        {
            var sb = new StringBuilder();

            sb.Append("__username__=");
            sb.Append(Uri.EscapeDataString(Username));
            sb.Append("&__password__=");
            sb.Append(Uri.EscapeDataString(Password));

            return sb.ToString();
        }

        public static LoginDetails Deserialize(string serializedString)
        {
            var acct = new LoginDetails();

            foreach (var p in serializedString.Split('&'))
            {
                var kv = p.Split('=');

                var key = Uri.UnescapeDataString(kv[0]);
                var val = kv.Length > 1 ? Uri.UnescapeDataString(kv[1]) : "";

                if (key == "__username__")
                {
                    acct.Username = val;
                }
                else if(key == "__password__")
                {
                    acct.Password = val;
                }
            }

            return acct;
        }
    }
}
