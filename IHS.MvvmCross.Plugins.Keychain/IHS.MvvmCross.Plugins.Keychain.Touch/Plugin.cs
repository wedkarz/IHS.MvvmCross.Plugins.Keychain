using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;

namespace IHS.MvvmCross.Plugins.Keychain.Touch
{
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.RegisterType<IKeychain, TouchKeychain>();   
        }
    }
}