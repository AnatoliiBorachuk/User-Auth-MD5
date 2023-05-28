using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace UserAuth
{
    [Serializable]
    public class UserDatabase
    {
        public static string GetMd5Hash(string password)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(password);
                var hashBytes = md5.ComputeHash(inputBytes);
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hash;
            }
        }

        public string FullPath =>
            Application.StartupPath + "/Database.dat";

        public UserCredentials[] KnownUsers;

        public UserDatabase()
        {
            Load();
        }

        private void Save()
        {
            using (var stream = File.Open(FullPath, FileMode.OpenOrCreate))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                formatter.Serialize(stream, this);

                stream.Close();
            }
        }

        private void Load ()
        {
            KnownUsers = new UserCredentials[0];
            if(File.Exists(FullPath))
            {
                using (var stream = File.Open(FullPath, FileMode.Open))
                {
                    var formatter = new BinaryFormatter();
                    stream.Seek(0, SeekOrigin.Begin);
                    var loaded = (UserDatabase)formatter.Deserialize(stream);
                    KnownUsers = loaded.KnownUsers;

                    stream.Close();
                }
            }
        }

        public void AddUser (UserCredentials user)
        {
            if(ContainsUser(user.Login) == false)
            {
                var tmp = KnownUsers.ToList();
                tmp.Add(user);
                KnownUsers = tmp.ToArray();
                Save();
            }
        }

        public bool ContainsUser (string login)
        {
            foreach (var user in KnownUsers)
                if (user.Login == login)
                    return true;

            return false;
        }

        public bool TryLogin (string login, string password)
        {
            var mdPass = GetMd5Hash(password);
            foreach (var user in KnownUsers)
                if (user.Login == login && mdPass == user.Password)
                    return true;

            return false;
        }
    }

    [Serializable]
    public class UserCredentials
    {
        public string Login;
        public string Password;

        public UserCredentials(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
