using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoTouch.Foundation;
using MonoTouch.Security;

namespace IHS.MvvmCross.Plugins.Keychain.Touch
{
    public class TouchKeychain : IKeychain
    {
        public bool SetPassword(string password, string serviceName, string account)
        {
            var updateCode = SecStatusCode.NotAvailable;

            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = serviceName,
                Account = account
            };

            SecStatusCode status;
            var match = SecKeyChain.QueryAsRecord(record, out status);
            if (status == SecStatusCode.Success)
            {
                var newAtributes = new SecRecord(SecKind.GenericPassword)
                {
                    ValueData = password != null ? NSData.FromString(password, NSStringEncoding.UTF8) : null
                };

                updateCode = SecKeyChain.Update(match, newAtributes);
            } 
            else if (status == SecStatusCode.ItemNotFound)
            {
                var newRecord = new SecRecord(SecKind.GenericPassword)
                {
                    Service = serviceName,
                    Account = account,
                    ValueData = password != null ? NSData.FromString(password, NSStringEncoding.UTF8) : null
                };

                updateCode = SecKeyChain.Add(newRecord);
            }

            return updateCode == SecStatusCode.Success;
        }

        public string GetPassword(string serviceName, string account)
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = serviceName,
                Account = account
            };

            SecStatusCode status;
            var match = SecKeyChain.QueryAsRecord(record, out status);
            if (status == SecStatusCode.Success && match.ValueData != null)
            {
                return NSString.FromData(match.ValueData, NSStringEncoding.UTF8);
            }

            return null;
        }

        public bool DeletePassword(string serviceName, string account)
        {
            return SetPassword(null, serviceName, account);
        }

        public LoginDetails GetLoginDetails(string serviceName)
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = serviceName
            };

            SecStatusCode status;
            var match = SecKeyChain.QueryAsRecord(record, out status);
            if (status == SecStatusCode.Success)
            {
                var loginDetails = new LoginDetails()
                {
                    Password = NSString.FromData(match.ValueData, NSStringEncoding.UTF8),
                    Username = match.Account
                };

                return loginDetails;
            }

            return null;
        }
    }
}
