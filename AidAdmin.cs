using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAid_Hub_Final_
{
    internal class AidAdmin : ItemRecord, IAdmin
    {
        public AidAdmin() { }
        public AidAdmin(string name, string contactnum, string item, string quantity, string location, string expdate) { }
        public void ViewInventory()
        {
            List<string[]> donationsData = CsvFileHandler.ReadFromCsv("donations.csv");

            var groupedItems = donationsData
                .GroupBy(item => item[2], StringComparer.OrdinalIgnoreCase)
                .Select(group => new
                {
                    ItemName = group.Key,
                    TotalQuantity = group.Sum(item => int.Parse(item[3]))
                }).ToList();

            bool Searching = true;

            while (Searching)
            {
                Console.Clear();
                var table = new Table();
                table.Border = TableBorder.Square;
                table.ShowRowSeparators = true;
                table.Alignment(Justify.Right);
                table.AddColumn("Item Name");
                table.AddColumn("Total Quantity");

                foreach (var item in groupedItems)
                {
                    table.AddRow(item.ItemName, item.TotalQuantity.ToString());
                }

                Console.SetCursorPosition(63, 0);
                Console.WriteLine("INVENTORY:");
                AnsiConsole.Write(table);

                Console.SetCursorPosition(0, 1);
                Console.WriteLine("Search an item (or leave blank to view all): ");
                string search = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(search))
                {
                    var filteredItems = groupedItems
                        .Where(item => item.ItemName.Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    Console.Clear();
                    if (filteredItems.Count > 0)
                    {
                        var filteredTable = new Table();
                        filteredTable.Border = TableBorder.Square;
                        filteredTable.ShowRowSeparators = true;
                        filteredTable.Alignment(Justify.Right);
                        filteredTable.AddColumn("Item Name");
                        filteredTable.AddColumn("Total Quantity");

                        foreach (var item in filteredItems)
                        {
                            filteredTable.AddRow(item.ItemName, item.TotalQuantity.ToString());
                        }
                        Console.SetCursorPosition(63, 0);
                        Console.WriteLine("SEARCH RESULTS:");
                        AnsiConsole.Write(filteredTable);
                    }
                    else
                    {
                        Console.SetCursorPosition(63, 0);
                        Console.WriteLine("SEARCH RESULTS:");
                        Console.SetCursorPosition(92, 2);
                        Console.WriteLine("No items found matching your search.");
                    }

                    bool validInput = false;
                    while (!validInput)
                    {
                        Console.SetCursorPosition(0, 1);
                        Console.Write("\nDo you want to search again? (y/n): ");
                        string response = Console.ReadLine()?.Trim().ToLower();

                        if (response == "y")
                        {
                            Searching = true;
                            validInput = true;
                        }
                        else if (response == "n")
                        {
                            Menus.AidGiverMenu();
                            validInput = true;
                        }
                        else
                        {
                            validInput = false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\nPress any key to return to AidAdmin Menu.");
                    Console.ReadKey();
                    Menus.AidAdminMenu();
                }
            }
        }
        public void ViewDonations()
        {
            List<string[]> donationsData = CsvFileHandler.ReadFromCsv("donations.csv");

            Console.Clear();
            if (donationsData.Count == 0)
            {
                Console.WriteLine("There are no Donations available.");
                Console.WriteLine("\nPress any key to return to AidAdmin Menu");
                Console.ReadKey();
                Menus.AidAdminMenu();
            }
            else
            {
                bool Searching = true;

                while (Searching)
                {
                    var donationTable = new Table();
                    donationTable.Border = TableBorder.Square;
                    donationTable.ShowRowSeparators = true;
                    donationTable.Alignment(Justify.Right);

                    donationTable.AddColumn("No.");
                    donationTable.AddColumn("Name");
                    donationTable.AddColumn("Contact");
                    donationTable.AddColumn("Item");
                    donationTable.AddColumn("Quantity");
                    donationTable.AddColumn("Expiration Date");

                    foreach (var donation in donationsData)
                    {
                        donationTable.AddRow((donationsData.IndexOf(donation) + 1).ToString(), donation[0], donation[1], donation[2], donation[3], donation[4]);

                    }
                    Console.Clear();
                    Console.SetCursorPosition(63, 0);
                    Console.WriteLine("DONATIONS: ");
                    AnsiConsole.Write(donationTable);

                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine("Search by Name or Item (leave blank to view all): ");
                    string search = Console.ReadLine()?.Trim();

                    if (!string.IsNullOrEmpty(search))
                    {
                        var filteredDonations = donationsData
                            .Where(donation => donation[0].Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                                donation[2].Contains(search, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (filteredDonations.Count > 0)
                        {
                            var filteredTable = new Table();
                            filteredTable.Border = TableBorder.Square;
                            filteredTable.ShowRowSeparators = true;
                            filteredTable.Alignment(Justify.Right);
                            filteredTable.AddColumn("No.");
                            filteredTable.AddColumn("Name");
                            filteredTable.AddColumn("Contact");
                            filteredTable.AddColumn("Item");
                            filteredTable.AddColumn("Quantity");
                            filteredTable.AddColumn("Location");

                            foreach (var donation in filteredDonations)
                            {
                                filteredTable.AddRow((filteredDonations.IndexOf(donation) + 1).ToString(), donation[0], donation[1], donation[2], donation[3], donation[4]);
                            }
                            Console.Clear();
                            Console.SetCursorPosition(63, 0);
                            Console.WriteLine("SEARCH RESULTS:");
                            AnsiConsole.Write(filteredTable);
                        }
                        else
                        {
                            Console.SetCursorPosition(63, 0);
                            Console.WriteLine("SEARCH RESULTS:");
                            Console.SetCursorPosition(92, 2);
                            Console.WriteLine("No donations found matching your search.");
                        }

                        bool validInput = false;
                        while (!validInput)
                        {
                            Console.SetCursorPosition(0, 1);
                            Console.Write("\nDo you want to search again? (y/n): ");
                            string response = Console.ReadLine()?.Trim().ToLower();

                            if (response == "y")
                            {
                                Searching = true;
                                validInput = true;
                            }
                            else if (response == "n")
                            {
                                Menus.AidAdminMenu();
                                validInput = true;
                            }
                            else
                            {
                                validInput = false;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nPress any key to return to AidAdmin Menu");
                        Console.ReadKey();
                        Menus.AidAdminMenu();
                    }
                }
            }
        }
        public void ViewAndDeliverRequests()
        {
            List<string[]> requestsData = CsvFileHandler.ReadFromCsv("requests.csv");
            List<string[]> donationsData = CsvFileHandler.ReadFromCsv("donations.csv");

            if (requestsData.Count == 0)
            {
                Console.WriteLine("There are no Requests available.");
                Console.WriteLine("\nPress any key to return to AidAdmin Menu");
                Console.ReadKey();
                Menus.AidAdminMenu();
                return;
            }

            bool Searching = true;

            while (Searching)
            {
                Console.Clear();
                var requestTable = new Table();
                requestTable.Border = TableBorder.Square;
                requestTable.ShowRowSeparators = true;
                requestTable.Alignment(Justify.Right);

                requestTable.AddColumn("No.");
                requestTable.AddColumn("Name");
                requestTable.AddColumn("Item");
                requestTable.AddColumn("Quantity");
                requestTable.AddColumn("Location");

                foreach (var request in requestsData)
                {
                    requestTable.AddRow((requestsData.IndexOf(request) + 1).ToString(), request[0], request[2], request[3], request[4]);
                }

                Console.SetCursorPosition(62, 0);
                Console.WriteLine("REQUESTS: ");
                AnsiConsole.Write(requestTable);

                Console.SetCursorPosition(0, 1);
                Console.WriteLine("\nSearch by Name, Item, or Location (leave blank to view all): ");
                string search = Console.ReadLine()?.Trim();

                List<string[]> filteredRequests = requestsData;

                if (!string.IsNullOrEmpty(search))
                {
                    filteredRequests = requestsData
                        .Where(request => request[0].Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                        request[2].Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                        request[4].Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    Console.Clear();
                    if (filteredRequests.Count > 0)

                    {
                        var filteredTable = new Table();
                        filteredTable.Border = TableBorder.Square;
                        filteredTable.ShowRowSeparators = true;
                        filteredTable.Alignment(Justify.Right);

                        filteredTable.AddColumn("No.");
                        filteredTable.AddColumn("Name");
                        filteredTable.AddColumn("Item");
                        filteredTable.AddColumn("Quantity");
                        filteredTable.AddColumn("Location");

                        foreach (var request in filteredRequests)
                        {
                            filteredTable.AddRow((filteredRequests.IndexOf(request) + 1).ToString(), request[0], request[2], request[3], request[4]);
                        }

                        Console.SetCursorPosition(62, 0);
                        Console.WriteLine("SEARCH RESULTS:");
                        AnsiConsole.Write(filteredTable);

                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine("\nEnter the ID of the request to deliver (or leave blank to return): ");
                        string input = Console.ReadLine()?.Trim();

                        if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int requestId))
                        {
                            if (requestId > 0 && requestId <= filteredRequests.Count)
                            {
                                var selectedRequest = filteredRequests[requestId - 1];
                                string requestedItem = selectedRequest[2];
                                int requestedQuantity = int.Parse(selectedRequest[3]);
                                string requestLocation = selectedRequest[4];

                                var donationsForItem = donationsData.Where(donation => donation[2].Equals(requestedItem, StringComparison.OrdinalIgnoreCase)).ToList();

                                if (donationsForItem.Any())
                                {
                                    int totalAvailableQuantity = donationsForItem.Sum(donation => int.Parse(donation[3]));
                                    int remainingRequestQuantity = requestedQuantity;

                                    foreach (var donation in donationsForItem)
                                    {
                                        int availableDonationQuantity = int.Parse(donation[3]);

                                        if (remainingRequestQuantity <= 0) break;

                                        if (availableDonationQuantity >= remainingRequestQuantity)
                                        {
                                            donation[3] = (availableDonationQuantity - remainingRequestQuantity).ToString();
                                            remainingRequestQuantity = 0;
                                        }
                                        else
                                        {
                                            donation[3] = "0";
                                            remainingRequestQuantity -= availableDonationQuantity;
                                        }
                                    }

                                    if (remainingRequestQuantity > 0)
                                    {
                                        Console.WriteLine($"Not enough stock available for {requestedItem}. Remaining quantity: {remainingRequestQuantity}");
                                    }
                                    else
                                    {
                                        CsvFileHandler.WriteToCsv("donations.csv", donationsData);

                                        var deliveredItem = new string[]
                                        {
                                            selectedRequest[0],
                                            selectedRequest[1],
                                            requestedItem,
                                            requestedQuantity.ToString(),
                                            requestLocation
                                        };
                                        CsvFileHandler.AppendToCsv("deliveredItems.csv", deliveredItem);

                                        requestsData.Remove(selectedRequest);
                                        CsvFileHandler.WriteToCsv("requests.csv", requestsData);

                                        Console.WriteLine("Item delivered successfully.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Item not found in inventory.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid request ID.");
                            }
                        }

                        Console.WriteLine("\nPress any key to return to AidAdmin Menu.");
                        if (string.IsNullOrWhiteSpace(Console.ReadLine()))
                        {
                            Menus.AidAdminMenu();
                            return;
                        }
                    }
                    else
                    {
                        Console.SetCursorPosition(63, 0);
                        Console.WriteLine("SEARCH RESULTS:");
                        Console.SetCursorPosition(92, 2);
                        Console.WriteLine("No requests found matching your search.");
                    }

                    bool validInput = false;
                    while (!validInput)
                    {
                        Console.SetCursorPosition(0, 1);
                        Console.Write("\nDo you want to search again? (y/n): ");
                        string response = Console.ReadLine()?.Trim().ToLower();

                        if (response == "y")
                        {
                            Searching = true;
                            validInput = true;
                        }
                        else if (response == "n")
                        {
                            Menus.AidAdminMenu();
                            validInput = true;
                        }
                        else
                        {
                            validInput = false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\nPress any key to return to AidAdmin Menu");
                    Console.ReadKey();
                    Menus.AidAdminMenu();
                }
            }
        }
        public void ViewDeliveredItems()
        {
            Console.Clear();
            List<string[]> deliveredItemsData = CsvFileHandler.ReadFromCsv("deliveredItems.csv");

            if (deliveredItemsData.Count == 0)
            {
                Console.WriteLine("There are no delivered items available.");
                Console.WriteLine("\nPress any key to return to AidAdmin Menu");
                Console.ReadKey();
                Menus.AidAdminMenu();
            }
            else
            {
                bool Searching = true;

                while (Searching)
                {
                    Console.Clear();
                    var deliveredTable = new Table();
                    deliveredTable.Border = TableBorder.Square;
                    deliveredTable.ShowRowSeparators = true;
                    deliveredTable.Alignment(Justify.Right);

                    deliveredTable.AddColumn("No.");
                    deliveredTable.AddColumn("Name");
                    deliveredTable.AddColumn("Item");
                    deliveredTable.AddColumn("Quantity");
                    deliveredTable.AddColumn("Location");

                    foreach (var deliveredItem in deliveredItemsData)
                    {
                        deliveredTable.AddRow((deliveredItemsData.IndexOf(deliveredItem) + 1).ToString(), deliveredItem[0], deliveredItem[2], deliveredItem[3], deliveredItem[4]);
                    }

                    Console.SetCursorPosition(62, 0);
                    Console.WriteLine("DELIVERED ITEMS: ");
                    AnsiConsole.Write(deliveredTable);

                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine("\nSearch by Name, Item, or Location (leave blank to view all): ");
                    string search = Console.ReadLine()?.Trim();

                    List<string[]> filteredDeliveredItems = deliveredItemsData;

                    if (!string.IsNullOrEmpty(search))
                    {
                        filteredDeliveredItems = deliveredItemsData
                            .Where(item => item[0].Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                           item[2].Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                           item[4].Contains(search, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        Console.Clear();
                        if (filteredDeliveredItems.Count > 0)
                        {
                            var filteredTable = new Table();
                            filteredTable.Border = TableBorder.Square;
                            filteredTable.ShowRowSeparators = true;
                            filteredTable.Alignment(Justify.Right);

                            filteredTable.AddColumn("ID");
                            filteredTable.AddColumn("Name");
                            filteredTable.AddColumn("Item");
                            filteredTable.AddColumn("Quantity");
                            filteredTable.AddColumn("Location");

                            foreach (var item in filteredDeliveredItems)
                            {
                                filteredTable.AddRow((filteredDeliveredItems.IndexOf(item) + 1).ToString(), item[0], item[2], item[3], item[4]);
                            }

                            Console.SetCursorPosition(62, 0);
                            Console.WriteLine("SEARCH RESULTS:");
                            AnsiConsole.Write(filteredTable);

                            do
                            {
                                Console.SetCursorPosition(0, 0);
                                Console.WriteLine("\nOptions: ");
                                Console.WriteLine("1. Delete a Delivered Item");
                                Console.WriteLine("2. Delete All Delivered Items");
                                Console.WriteLine("3. Return to AidAdmin Menu");
                                Console.Write("\nChoose an option: ");
                                string option = Console.ReadLine()?.Trim();

                                switch (option)
                                {
                                    case "1":
                                        Console.Write("\nEnter the ID of the delivered item you want to delete: ");
                                        int deleteId = int.Parse(Console.ReadLine()?.Trim() ?? "0");

                                        if (deleteId > 0 && deleteId <= filteredDeliveredItems.Count)
                                        {
                                            DeleteDeliveredItem(filteredDeliveredItems[deleteId - 1], deliveredItemsData);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid ID.");
                                        }
                                        break;

                                    case "2":
                                        Console.Write("\nAre you sure you want to delete all searched delivered items? (y/n): ");
                                        string deleteAllResponse = Console.ReadLine()?.Trim().ToLower();

                                        if (deleteAllResponse == "y")
                                        {
                                            foreach (var item in filteredDeliveredItems)
                                            {
                                                deliveredItemsData.Remove(item);
                                            }
                                            CsvFileHandler.WriteToCsv("deliveredItems.csv", deliveredItemsData);
                                            Console.WriteLine("All searched delivered items have been deleted.");
                                            Console.ReadKey();
                                            Menus.AidAdminMenu();
                                        }
                                        break;

                                    case "3":
                                        Searching = false;
                                        Menus.AidAdminMenu();
                                        break;

                                    default:
                                        Console.WriteLine("Invalid option, please choose a valid option.");
                                        break;
                                }
                            } while (true);
                        }
                        else
                        {
                            Console.SetCursorPosition(63, 0);
                            Console.WriteLine("SEARCH RESULTS:");
                            Console.SetCursorPosition(92, 2);
                            Console.WriteLine("No delivered items found matching your search.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nPress any key to return to AidAdmin Menu.");
                        Console.ReadKey();
                        Menus.AidAdminMenu();
                    }
                }
            }
        }
        private void DeleteDeliveredItem(string[] deliveredItem, List<string[]> deliveredItemsData)
        {
            deliveredItemsData.Remove(deliveredItem);
            CsvFileHandler.WriteToCsv("deliveredItems.csv", deliveredItemsData);
            Console.WriteLine("Delivered item has been deleted.");
            Console.ReadKey();
            Menus.AidAdminMenu();
        }
    }
}
