using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Runtime;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Droid;
using Java.IO;
using Java.Security;
using Javax.Crypto;


namespace IHS.MvvmCross.Plugins.Keychain.Droid
{
    public class DroidKeychain : IKeychain
    {
        private KeyStore _keyStore;
        KeyStore.PasswordProtection _passwordProtection;

        static readonly object fileLock = new object();

        const string FileName = "Xamarin.Social.Accounts";
        static readonly char[] Password = "3295043EA18CA264B2C40E0B72051DEF2D07AD2B4593F43DDDE1515A7EC32617".ToCharArray();

        private const string PASSWORD_KEY = "password";

        private Context _context;
        private Context Context
        {
            get { return _context ?? (_context = Mvx.Resolve<IMvxAndroidGlobals>().ApplicationContext); }
            set { _context = value; }
        }

        public DroidKeychain()
        {
            _keyStore = KeyStore.GetInstance(KeyStore.DefaultType);
            _passwordProtection = new KeyStore.PasswordProtection(Password);

            try
            {
                lock (fileLock)
                {
                    using (var s = Context.OpenFileInput(FileName))
                    {
                        _keyStore.Load(s, Password);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                LoadEmptyKeyStore(Password);
            }
        }

        public bool SetPassword(string password, string serviceName, string account)
        {
            var storedAccount = FindAccountsForService(serviceName).FirstOrDefault(ac => ac.Username == account);
            if (storedAccount != null)
            {
                storedAccount.Password = password;
            }
            else
            {
                storedAccount = new LoginDetails() { Password = password, Username = account };
            }

            Save(storedAccount, serviceName);

            return true;
        }

        public string GetPassword(string serviceName, string account)
        {
            var storedAccount = FindAccountsForService(serviceName).FirstOrDefault(ac => ac.Username == account);
            return storedAccount != null ? storedAccount.Password : null;
        }

        public bool DeletePassword(string serviceName, string account)
        {
            var storedAccount = FindAccountsForService(serviceName).FirstOrDefault(ac => ac.Username == account);
            if (storedAccount == null)
                return true;

            storedAccount.Password = string.Empty;
            Save(storedAccount, serviceName);

            return true;
        }

        public LoginDetails GetLoginDetails(string serviceName)
        {
            var storedAccount = FindAccountsForService(serviceName).FirstOrDefault();

            return storedAccount;
        }

        public bool DeleteAccount(string serviceName, string account)
        {
            var storedAccount = FindAccountsForService(serviceName).FirstOrDefault(ac => ac.Username == account);
            if (storedAccount == null)
                return true;

            Delete(storedAccount, serviceName);

            return true;
        }

        #region Port from Xamarin.Secutiry
        private IEnumerable<LoginDetails> FindAccountsForService(string serviceId)
        {
            var r = new List<LoginDetails>();

            var postfix = "-" + serviceId;

            var aliases = _keyStore.Aliases();
            while (aliases.HasMoreElements)
            {
                var alias = aliases.NextElement().ToString();
                if (alias.EndsWith(postfix))
                {
                    var e = _keyStore.GetEntry(alias, _passwordProtection) as KeyStore.SecretKeyEntry;
                    if (e != null)
                    {
                        var bytes = e.SecretKey.GetEncoded();
                        var serialized = Encoding.UTF8.GetString(bytes);
                        var acct = LoginDetails.Deserialize(serialized);
                        r.Add(acct);
                    }
                }
            }

            r.Sort((a, b) => a.Username.CompareTo(b.Username));

            return r;
        }

        private void Save(LoginDetails account, string serviceId)
        {
            var alias = MakeAlias(account, serviceId);

            var secretKey = new SecretAccount(account);
            var entry = new KeyStore.SecretKeyEntry(secretKey);
            _keyStore.SetEntry(alias, entry, _passwordProtection);

            Save();
        }

        private void Delete(LoginDetails account, string serviceId)
        {
            var alias = MakeAlias(account, serviceId);

            _keyStore.DeleteEntry(alias);
            Save();
        }

        private void Save()
        {
            lock (fileLock)
            {
                using (var s = Context.OpenFileOutput(FileName, FileCreationMode.Private))
                {
                    _keyStore.Store(s, Password);
                }
            }
        }

        private static string MakeAlias(LoginDetails account, string serviceId)
        {
            return account.Username + "-" + serviceId;
        }

        private class SecretAccount : Java.Lang.Object, ISecretKey
        {
            byte[] bytes;
            public SecretAccount(LoginDetails account)
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(account.Serialize());
            }
            public byte[] GetEncoded()
            {
                return bytes;
            }
            public string Algorithm
            {
                get
                {
                    return "RAW";
                }
            }
            public string Format
            {
                get
                {
                    return "RAW";
                }
            }
        }

        private static IntPtr id_load_Ljava_io_InputStream_arrayC;

        /// <summary>
        /// Work around Bug https://bugzilla.xamarin.com/show_bug.cgi?id=6766
        /// </summary>
        private void LoadEmptyKeyStore(char[] password)
        {
            if (id_load_Ljava_io_InputStream_arrayC == IntPtr.Zero)
            {
                id_load_Ljava_io_InputStream_arrayC = JNIEnv.GetMethodID(_keyStore.Class.Handle, "load", "(Ljava/io/InputStream;[C)V");
            }
            IntPtr intPtr = IntPtr.Zero;
            IntPtr intPtr2 = JNIEnv.NewArray(password);
            JNIEnv.CallVoidMethod(_keyStore.Handle, id_load_Ljava_io_InputStream_arrayC, new JValue[]
				{
					new JValue (intPtr),
					new JValue (intPtr2)
				});
            JNIEnv.DeleteLocalRef(intPtr);
            if (password != null)
            {
                JNIEnv.CopyArray(intPtr2, password);
                JNIEnv.DeleteLocalRef(intPtr2);
            }
        }

        #endregion
    }
}
