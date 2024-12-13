using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAid_Hub_Final_
{
    internal abstract class ItemRecord
    {
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Item { get; set; }
        public string Quantity { get; set; }
        public string Location { get; set; }
        public string ExpDate { get; set; }
        public ItemRecord() { }
        public ItemRecord(string name, string contactnum, string item, string quantity, string location, string expdate)
        {
            Name = name;
            ContactNumber = contactnum;
            Item = item;
            Quantity = quantity;
            Location = location;
            ExpDate = expdate;
        }
    }
}
