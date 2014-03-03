using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace IHS.MvvmCross.Plugins.Keychain.Droid
{
    public class DroidKeychain : IKeychain
    {
        public bool SetPassword(string password, string serviceName, string account)
        {
            throw new NotImplementedException();
        }

        public string GetPassword(string serviceName, string account)
        {
            throw new NotImplementedException();
        }

        public bool DeletePassword(string serviceName, string account)
        {
            throw new NotImplementedException();
        }
    }

    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.RegisterType<IKeychain, DroidKeychain>();
        }
    }
}
