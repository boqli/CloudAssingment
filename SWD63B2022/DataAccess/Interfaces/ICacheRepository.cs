using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface ICacheRepository
    {
        public List<MenuItem> GetMenus();

        public void AddMenu(MenuItem item);

        public void updateCredit(Credit credit);

        public void addCredit(Credit credit);

        int buyCredit(int credit);
        public List<Credit> getCredits();

    }
}
