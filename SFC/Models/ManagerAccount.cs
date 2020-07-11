using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFC.Models
{
    public class ManagerAccount
    {
        public string username { get; set; }
        public string cipherPassword { get; set; }
        public string name { get; set; }
        public int birthYear { get; set; }
        public string email { get; set; }
        public int id { get; set; }
        public ManagerAccount() { }
        public ManagerAccount(string username, string plainPassword, string name, int birthYear, string email, int id)
        {
            this.username = username;
            cipherPassword = ManagerAccountService.ComputeMD5Hash(plainPassword);
            this.name = name;
            this.birthYear = birthYear;
            this.email = email;
            this.id = id;
        }
        public ManagerAccount(ManagerAccount src)
        {
            username = src.username;
            cipherPassword = src.cipherPassword;
            name = src.name;
            birthYear = src.birthYear;
            email = src.email;
            id = src.id;
        }
    }
}