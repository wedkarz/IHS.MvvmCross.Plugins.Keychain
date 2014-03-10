using Android.App;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace IHS.MvvmCross.Plugins.Keychain.Droid
{
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.RegisterType<IKeychain, DroidKeychain>();
        }
    }
}