using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace IHS.MvvmCross.Plugins.Keychain.WindowsPhone
{
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.RegisterType<IKeychain, WindowsPhoneKeychain>();
        }
    }
}
