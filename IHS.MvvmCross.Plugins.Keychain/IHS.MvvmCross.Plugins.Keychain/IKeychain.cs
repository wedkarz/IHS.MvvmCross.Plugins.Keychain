namespace IHS.MvvmCross.Plugins.Keychain
{
    public interface IKeychain
    {
//        + (NSArray *)allAccounts;
//+ (NSArray *)accountsForService:(NSString *)serviceName;
//+ (NSString *)passwordForService:(NSString *)serviceName account:(NSString *)account;
//+ (BOOL)deletePasswordForService:(NSString *)serviceName account:(NSString *)account;
//+ (BOOL)setPassword:(NSString *)password forService:(NSString *)serviceName account:(NSString *)account;
        bool SetPassword(string password, string serviceName, string account);
        string GetPassword(string serviceName, string account);
        bool DeletePassword(string serviceName, string account);
    }
}