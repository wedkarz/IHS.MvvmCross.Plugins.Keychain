using Android.App;
using MvvmCross.Core;
using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;

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