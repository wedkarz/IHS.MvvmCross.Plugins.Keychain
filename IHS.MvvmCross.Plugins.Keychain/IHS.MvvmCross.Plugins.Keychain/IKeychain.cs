namespace IHS.MvvmCross.Plugins.Keychain
{
    public interface IKeychain
    {
		bool SetPassword(string password, string serviceName, string account, bool enableTouchId = false);
        string GetPassword(string serviceName, string account);
        bool DeletePassword(string serviceName, string account);
        LoginDetails GetLoginDetails(string serviceName);
        bool DeleteAccount(string serviceName, string account);
    }
}