using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAid_Hub_Final_
{
    internal interface IActions
    {
        void Donate();
        void ViewDonations();
        void ViewRequests();
        void ViewInventory();

    }
    public interface IRequests
    {
        void RequestDelivery();
        void ViewRequests();
        void ViewInventory();
    }
    public interface IAdmin
    {
        void ViewInventory();
        void ViewDonations();
        void ViewAndDeliverRequests();
        void ViewDeliveredItems();
    }
}
