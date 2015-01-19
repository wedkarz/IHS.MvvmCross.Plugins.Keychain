using System;
using Foundation;
using Security;
using UIKit;

namespace IHS.MvvmCross.Plugins.Keychain.Touch
{
    public class TouchKeychain : IKeychain
    {
		bool? _isIos8;

		public bool IsIos8
		{
			get { return (_isIos8 ?? (_isIos8 = Convert.ToInt16(UIDevice.CurrentDevice.SystemVersion.Split ('.')[0].ToString()) >= 8)).Value; }
		}

		public bool SetPassword(string password, string serviceName, string account, bool enableTouchId = false)
		{
			var record = new SecRecord(SecKind.GenericPassword)
			{
				Service = serviceName,
				Account = account
			};

			var updateCode = SecKeyChain.Remove(record);

			SecRecord newRecord = null;

			if (updateCode == SecStatusCode.Success || updateCode == SecStatusCode.ItemNotFound)
			{
				if (IsIos8 && enableTouchId)
				{
					var secObject = new SecAccessControl (SecAccessible.WhenPasscodeSetThisDeviceOnly, SecAccessControlCreateFlags.UserPresence);

					newRecord = new SecRecord (SecKind.GenericPassword) 
					{
						Service = serviceName,
						Account = account,
						ValueData = password != null ? NSData.FromString (password, NSStringEncoding.UTF8) : null,
						UseNoAuthenticationUI = true,
						AccessControl = secObject
					};
				}
				else
				{
					newRecord = new SecRecord (SecKind.GenericPassword) 
					{
						Service = serviceName,
						Account = account,
						ValueData = password != null ? NSData.FromString (password, NSStringEncoding.UTF8) : null
					};
				}

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
            return SetPassword(String.Empty, serviceName, account);
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

        public bool DeleteAccount(string serviceName, string account)
        {
            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = serviceName,
                Account = account
            };

            var removalStatus = SecKeyChain.Remove(record);

            return removalStatus == SecStatusCode.Success;
        }
    }
}
