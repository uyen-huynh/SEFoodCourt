using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFC.Models
{
    public class ManagerAccountService
    {

        public static ManagerAccount current = new ManagerAccount();


        public static ManagerAccount copyInfo(ManagerAccount src)
        {
            current = new ManagerAccount(src);
            return current;
        }
        public static void logOut()
        {
            current = new ManagerAccount();
        }

        // Attribute
        private static readonly string Salt = "x41#";
        private static Dictionary<string, ManagerAccount> accountDict = DatabaseService.DBGetDictionary<ManagerAccount>("Account/VendorOwner");

        // Method
        public static string ComputeMD5Hash(string rawData)
        {
            // ComputeHash - returns byte array  
            byte[] bytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(rawData + Salt));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static ManagerAccount CheckLogin(string username, string password)
        {
            if (password.Length == 0 && username.Length == 0)
            {
                throw new Exception("You have not entered anything !");
            }
            if (password.Length < 8)
            {
                throw new Exception("Your password's length < 8");
            }
            if (username.Length < 8)
            {
                throw new Exception("Your user name's length < 8");
            }

            var regexItem = new Regex("^[A-Za-z0-9_-]*$");
            if (!regexItem.IsMatch(username))
            {
                throw new Exception("You user name have special character");
            }
            if (!regexItem.IsMatch(password))
            {
                throw new Exception("You password have special character");
            }
            if (!accountDict.ContainsKey(username))
                throw new Exception("Your account does not exist !");

            ManagerAccount account = accountDict[username]; 
            if (ComputeMD5Hash(password) == account.cipherPassword)
            {
                copyInfo(account);
                return account;
            }
            else
            {
                throw new Exception("Your password is wrong");
            }

        }
        public static async Task<bool> CheckSignUp(string username, string password, string name, int birthYear, string email, int id)
        {
            if (password.Length < 8)
            {
                throw new Exception("Your password's length < 8");
            }
            if (username.Length < 8)
            {
                throw new Exception("Your user name's length < 8");
            }

            // Check username
            if (accountDict != null)
                if (accountDict.ContainsKey(username))
                    throw new Exception("Your username is exist !");

            // Check email format
            bool checkEmailFormat = Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            if (!checkEmailFormat)
            {
                throw new Exception("Your email format is wrong !");
            }

            // Write to Database
            ManagerAccount newAccount = new ManagerAccount(username, password, name, birthYear, email, id);
            await DatabaseService.DBWrite(newAccount, "Account/VendorOwner/" + username);
            accountDict.Add(username, newAccount);

            return true;
        }

    }
}
