using System;
using System.Threading.Tasks;
using System.Linq;

// Libary contains struct Dictionary & List
using System.Collections.Generic;

// Libaries connect Firebase
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;

// Libary to convert JSON string
using Newtonsoft.Json;

namespace SFC.Models
{
    public class DatabaseService
    {

        // Conect to FireBase RealTime by FireSharp
        private static readonly IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "2fY3dy7VvVTyF4WhsgFlaTeG7bO0eL2jL2QJGACz",
            BasePath = "https://fir-bksfc.firebaseio.com/"
        };
        private static readonly FireSharp.FirebaseClient firebaseClient = new FireSharp.FirebaseClient(config);
        static readonly IFirebaseClient client = firebaseClient;



        public static bool DBCheckLink(string link)
        {
            FirebaseResponse response = client.Get(link);
            if (response.Body == "null")
                return false; // Fail connect
            return true; // Success connect
        }

        public static async Task DBWrite<T>(T data, string link)
        {
            await client.SetAsync(link, data);
        }

        // Cannot get structure List just class data
        public static async Task<T> DBGetRecord<T>(string link)
        {
            if (!DBCheckLink(link))
                throw new Exception("Fail Connect! Wrong Link");

            FirebaseResponse response = await client.GetAsync(link);
            T result = response.ResultAs<T>();
            return result;
        }

        public static List<T> DBGetList<T>(string link)
        {
            if (!DBCheckLink(link))
                throw new Exception("Fail Connect! Wrong Link");

            FirebaseResponse response = client.Get(link);

            string json = response.Body;
            if (json == "null") return null;

            if (json[0] == '[')
            {
                var sList = JsonConvert.DeserializeObject<List<T>>(json);
                sList.RemoveAll(item => item == null);
                return sList;
            }
            else
            {
                var mList = JsonConvert.DeserializeObject<IDictionary<string, T>>(json);
                return mList.Select(x => x.Value).ToList();
            }

        }
        public static async Task DBDelete(string link)
        {
            if (!DBCheckLink(link))
                throw new Exception("Fail Connect! Wrong Link");

            await client.DeleteAsync(link);
        }

        public static async Task DBUpdate<T>(T data, string link)
        {
            if (!DBCheckLink(link))
                throw new Exception("Fail Connect! Wrong Link");

            await client.UpdateAsync(link, data);
        }

        public static Dictionary<string, T> DBGetDictionary<T>(string link)
        {
            FirebaseResponse response = client.Get(link);
            string json = response.Body;

            if (json == "null") return null;

            if (json[0] != '[')
            {
                return JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
            }
            else
            {
                return null;
            }
        }
        
    }
}