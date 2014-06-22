using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Security.Cryptography;

namespace IHS.MvvmCross.Plugins.Keychain.WindowsPhone
{
    /// <summary>
    /// Store username and password in two separate isolated storage files.
    /// Filename determines if we are getting password (serviceName+account) or account(serviceName)
    /// </summary>
    public class WindowsPhoneKeychain : IKeychain
    {
        
        /// <summary>
        /// Set password for service and account
        /// </summary>
        /// <param name="password"></param>
        /// <param name="serviceName"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool SetPassword(string password, string serviceName, string account)
        {

            // Convert the password to a byte[].
            var passwordByte = Encoding.UTF8.GetBytes(password);

            // Encrypt the password by using the Protect() method.
            var protectedPasswordByte = ProtectedData.Protect(passwordByte, null);

            // Store the encrypted password in isolated storage.
            WriteToFile(protectedPasswordByte, serviceName + account);

            // Convert the username to a byte[].
            var usernameByte = Encoding.UTF8.GetBytes(account);

            // Encrypt the username by using the Protect() method.
            var protectedUsernameByte = ProtectedData.Protect(usernameByte, null);

            // Store the encrypted username in isolated storage.
            WriteToFile(protectedUsernameByte, serviceName);

            return true;
        }

        /// <summary>
        /// Write a string to isolated storage
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        private void WriteToFile(byte[] data, string filePath)
        {
            // Create a file in the application's isolated storage.
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream writestream = new IsolatedStorageFileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, file);

            // Write pinData to the file.
            Stream writer = new StreamWriter(writestream).BaseStream;
            writer.Write(data, 0, data.Length);
            writer.Close();
            writestream.Close();
        }

        public string GetPassword(string serviceName, string account)
        {            
            return ReadIsolatedStorage(serviceName+account);
        }

        public string GetUsername(string serviceName)
        {
            return ReadIsolatedStorage(serviceName);
        }

        /// <summary>
        /// Given a filename read the item from isolated storage
        /// </summary>
        /// <param name="filename">defines item developer is looking for in isolated storage</param>
        /// <returns>value found in isolated storage</returns>
        private string ReadIsolatedStorage(string filename)
        {
            using (var folder = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string returnValue = null; //null if not found
                if (folder.FileExists(filename))
                {
                    // Retrieve the item from isolated storage.
                    byte[] protectedItemByte = this.ReadFromFile(filename);

                    // Decrypt the item by using the Unprotect method.
                    byte[] itemByte = ProtectedData.Unprotect(protectedItemByte, null);

                    // Convert the password from byte to string and display it in the text box.
                    returnValue = Encoding.UTF8.GetString(itemByte, 0, itemByte.Length);
                }
                return returnValue;
            }
        }

        
        /// <summary>
        /// Read value from isolated storage
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private byte[] ReadFromFile(string filePath)
        {
            // Access the file in the application's isolated storage.
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream readstream = new IsolatedStorageFileStream(filePath, System.IO.FileMode.Open, FileAccess.Read, file);

            // Read the PIN from the file.
            Stream reader = new StreamReader(readstream).BaseStream;
            byte[] pinArray = new byte[reader.Length];

            reader.Read(pinArray, 0, pinArray.Length);
            reader.Close();
            readstream.Close();

            return pinArray;
        }

        /// <summary>
        /// Delete password
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool DeletePassword(string serviceName, string account)
        {
            DeleteFileIsolatedStorage(serviceName+account);
            return true;
        }

        private static void DeleteFileIsolatedStorage(string file)
        {
            using (var folder = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (folder.FileExists(file))
                {
                    folder.DeleteFile(file);
                }
            }
        }

        /// <summary>
        /// Delete username
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public bool DeleteUsername(string serviceName)
        {
            DeleteFileIsolatedStorage(serviceName);
            return true;
        }

        public LoginDetails GetLoginDetails(string serviceName)
        {
            var returnValue = new LoginDetails
            {
                Username = null,
                Password = null
            };

            using (var folder = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (folder.FileExists(serviceName))
                {
                    returnValue.Username = GetUsername(serviceName);
                }            
                if (null != returnValue.Username && folder.FileExists(serviceName+returnValue.Username))
                {
                    returnValue.Password = GetPassword(serviceName, returnValue.Username);
                }
            }
            return null == returnValue.Username ? null : returnValue;
        }

        public bool DeleteAccount(string serviceName, string account)
        {
            DeletePassword(serviceName, account);
            DeleteUsername(serviceName);
            return true;
        }
    }
}
