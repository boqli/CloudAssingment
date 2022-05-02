using Common;
using DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace DataAccess.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private FirestoreDb db { get; set; }


        private IDatabase myDatabase;
        public CacheRepository(string connectionString, string project) {
            ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(connectionString);
            myDatabase = multiplexer.GetDatabase();
            db = FirestoreDb.Create(project);
        }

        public List<MenuItem> GetMenus()
        {
            var myList = myDatabase.StringGet("menuitems");
            if (myList.IsNullOrEmpty)
                return new List<MenuItem>(); //empty list
            else
            {
                var myList_fromString = JsonConvert.DeserializeObject<List<MenuItem>>(myList);
                return myList_fromString;
            }
        }

        public void AddMenu( MenuItem  item)
        {
            var myList = GetMenus();//gets menus from cache
            myList.Add(item); //adding a menu to the list of existing menus
            string myjsonstring = JsonConvert.SerializeObject(myList);
            //ultimately storing back the updated list serialized
            myDatabase.StringSet("menuitems",myjsonstring);
        }

        public int buyCredit(int credit)
        {
            var myList = getCredits();
            var cost=0;
            if(credit == 10)
            {
               cost = myList[0].credit10;
            }
            else if(credit == 20)
            {
                cost = myList[0].credit20;
            }
            else if(credit == 30)
            {
                cost = myList[0].credit30;
            }
            return cost;
        }

        public List<Credit> getCredits()
        {
            var myList = myDatabase.StringGet("creditt");
            if (myList.IsNullOrEmpty)
            {
                return new List<Credit>();
            }
            else
            {
                var myList_fromString = JsonConvert.DeserializeObject<List<Credit>>(myList);

                return myList_fromString;
            }

        }
        public void updateCredit(Credit credit)
        {
            var myList = getCredits();
            myList.Insert(0, credit);// Add(credit);
            if (myList.Count >= 2)
            {
                for (int i = 1; i <= myList.Count; i++)
                {
                    myList.RemoveAt(1);
                }
            }    
            myDatabase.StringSet("creditt", JsonConvert.SerializeObject(myList));
        }

        public void addCredit(Credit credit)
        {
            var myList = getCredits();
            myList.Add(credit);
            //string myjsonString = JsonConvert.SerializeObject(myList);
            myDatabase.StringSet("creditt", JsonConvert.SerializeObject(myList));
        }

    }
}
