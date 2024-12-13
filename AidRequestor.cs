using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace HealthAid_Hub_Final_
{
    class AidRequestor : ItemRecord, IRequests
    {
        public AidRequestor() { }
        public AidRequestor(string name, string contactnum, string item, string quantity, string location) { }
        public void RequestDelivery()
        {
            bool requestAgain = true;

            while (requestAgain)
            {
                Console.Clear();
                Console.WriteLine("Requesting Delivery Process:");
                int promptLine = Console.CursorTop;

                string name = GetValidatedInput(
                    promptLine,
                    "Enter your name: ",
                    input => !string.IsNullOrEmpty(input) && 
                    !input.Any(char.IsDigit) &&
                    input.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)),
                    "Invalid input. Please try again!"
                );

                string contactNumber = GetValidatedInput(
                    promptLine + 1,
                    "Enter your contact number (e.g., 09978755840): ",
                    input => Regex.IsMatch(input, @"^09\d{9}$"),
                    "Invalid contact number. Please try again!"
                );

                string itemName = GetValidatedInput(
                    promptLine + 2,
                    "Enter item name: ",
                    input => !string.IsNullOrEmpty(input),
                    "Item name cannot be empty. Please try again!"
                );

                string quantity = GetValidatedInput(
                    promptLine + 3,
                    "Enter quantity: ",
                    input => int.TryParse(input, out _),
                    "Invalid quantity. Please enter a valid number."
                );

                string location = GetValidatedInput(
                    promptLine + 4,
                    "Enter location: ",
                    input => !string.IsNullOrEmpty(input),
                    "Location cannot be empty. Please try again!"
                );

                string[] requestData = { name, contactNumber, itemName, quantity, location };
                CsvFileHandler.AppendToCsv("requests.csv", requestData);

                Console.WriteLine("\nRequest Added Succesfully!");

                string response = GetValidatedInput(
                    promptLine + 5,
                    "Do you want to request again? (y/n): ",
                    input => input == "y" || input == "n",
                    "Invalid input. Please enter 'y' to request again or 'n' to stop."
                );

                if (response == "n")
                {
                    requestAgain = false;
                    Menus.AidRequestorMenu();
                }
            }
        }
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
                            Menus.AidRequestorMenu();
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
                    Console.WriteLine("\nPress any key to return to AidRequestor Menu.");
                    Console.ReadKey();
                    Menus.AidRequestorMenu();
                }
            }
        }
        public void ViewRequests()
        {
            Console.Clear();
            List<string[]> requestsData = CsvFileHandler.ReadFromCsv("requests.csv");

            if (requestsData.Count == 0)
            {
                Console.WriteLine("There are no Requests available.");
                Console.WriteLine("\nPress any key to return to AidRequestor Menu");
                Console.ReadKey();
                Menus.AidRequestorMenu();
            }
            else
            {
                bool Searching = true;

                while (Searching)
                {
                    Console.Clear();
                    var requestTable = new Table();
                    requestTable.Border = TableBorder.Square;
                    requestTable.ShowRowSeparators = true;
                    requestTable.Alignment(Justify.Right);

                    requestTable.AddColumn("ID");
                    requestTable.AddColumn("Name");
                    requestTable.AddColumn("Contact");
                    requestTable.AddColumn("Item");
                    requestTable.AddColumn("Quantity");
                    requestTable.AddColumn("Location");

                    foreach (var request in requestsData)
                    {
                        requestTable.AddRow((requestsData.IndexOf(request) + 1).ToString(), request[0], request[1], request[2], request[3], request[4]);
                    }

                    Console.SetCursorPosition(62, 0);
                    Console.WriteLine("REQUESTS: ");
                    AnsiConsole.Write(requestTable);

                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine("\nSearch by Name, Item or Location (leave blank to view all): ");
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

                            filteredTable.AddColumn("ID");
                            filteredTable.AddColumn("Name");
                            filteredTable.AddColumn("Contact");
                            filteredTable.AddColumn("Item");
                            filteredTable.AddColumn("Quantity");
                            filteredTable.AddColumn("Location");

                            foreach (var request in filteredRequests)
                            {
                                filteredTable.AddRow((filteredRequests.IndexOf(request) + 1).ToString(), request[0], request[1], request[2], request[3], request[4]);
                            }

                            Console.SetCursorPosition(62, 0);
                            Console.WriteLine("SEARCH RESULTS:");
                            AnsiConsole.Write(filteredTable);

                            do
                            {
                                Console.SetCursorPosition(0, 0);
                                Console.WriteLine("\nOptions: ");
                                Console.WriteLine("1. Update a Request");
                                Console.WriteLine("2. Delete a Request");
                                Console.WriteLine("3. Delete All Requests");
                                Console.WriteLine("4. Return to AidRequestor Menu");
                                Console.Write("\nChoose an option: ");
                                string option = Console.ReadLine()?.Trim();

                                switch (option)
                                {
                                    case "1":
                                        Console.Write("\nEnter the ID of the request you want to update: ");
                                        int updateId = int.Parse(Console.ReadLine()?.Trim() ?? "0");

                                        if (updateId > 0 && updateId <= filteredRequests.Count)
                                        {
                                            var requestToUpdate = filteredRequests[updateId - 1];
                                            UpdateRequest(requestToUpdate, requestsData);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid ID.");
                                        }
                                        break;

                                    case "2":
                                        Console.Write("\nEnter the ID of the request you want to delete: ");
                                        int deleteId = int.Parse(Console.ReadLine()?.Trim() ?? "0");

                                        if (deleteId > 0 && deleteId <= filteredRequests.Count)
                                        {
                                            DeleteRequest(filteredRequests[deleteId - 1], requestsData);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid ID.");
                                        }
                                        break;

                                    case "3":
                                        Console.Write("\nAre you sure you want to delete all searched requests? (y/n): ");
                                        string deleteAllResponse = Console.ReadLine()?.Trim().ToLower();

                                        if (deleteAllResponse == "y")
                                        {
                                            foreach (var request in filteredRequests)
                                            {
                                                requestsData.Remove(request);
                                            }
                                            CsvFileHandler.WriteToCsv("requests.csv", requestsData);
                                            Console.WriteLine("All searched requests have been deleted.");
                                            Console.ReadKey();
                                            Menus.AidRequestorMenu();
                                        }
                                        break;

                                    case "4":
                                        Searching = false;
                                        Menus.AidRequestorMenu();
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
                                Menus.AidRequestorMenu();
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
                        Console.WriteLine("\nPress any key to return to AidRequestor Menu.");
                        Console.ReadKey();
                        Menus.AidRequestorMenu();
                    }
                }
            }
        }
        private void UpdateRequest(string[] requestToUpdate, List<string[]> allRequests)
        {
            Console.Clear();
            Console.WriteLine("Update Request:");

            Console.Write($"Current Name: {requestToUpdate[0]}\nEnter new Name (or press Enter to keep): ");
            string newName = Console.ReadLine()?.Trim();
            requestToUpdate[0] = string.IsNullOrEmpty(newName) ? requestToUpdate[0] : newName;

            Console.Write($"Current Contact: {requestToUpdate[1]}\nEnter new Contact (or press Enter to keep): ");
            string newContact = Console.ReadLine()?.Trim();
            requestToUpdate[1] = string.IsNullOrEmpty(newContact) ? requestToUpdate[1] : newContact;

            Console.Write($"Current Item: {requestToUpdate[2]}\nEnter new Item (or press Enter to keep): ");
            string newItem = Console.ReadLine()?.Trim();
            requestToUpdate[2] = string.IsNullOrEmpty(newItem) ? requestToUpdate[2] : newItem;

            Console.Write($"Current Quantity: {requestToUpdate[3]}\nEnter new Quantity (or press Enter to keep): ");
            string newQuantity = Console.ReadLine()?.Trim();
            requestToUpdate[3] = string.IsNullOrEmpty(newQuantity) ? requestToUpdate[3] : newQuantity;

            Console.Write($"Current Location: {requestToUpdate[4]}\nEnter new Location (or press Enter to keep): ");
            string newLocation = Console.ReadLine()?.Trim();
            requestToUpdate[4] = string.IsNullOrEmpty(newLocation) ? requestToUpdate[4] : newLocation;

            CsvFileHandler.WriteToCsv("requests.csv", allRequests);

            Console.WriteLine("\nRequest updated successfully and saved to file.");
            Console.WriteLine("Press any key to return.");
            Console.ReadKey();
            Menus.AidRequestorMenu();
        }
        private void DeleteRequest(string[] request, List<string[]> requestsData)
        {
            requestsData.Remove(request);
            CsvFileHandler.WriteToCsv("requests.csv", requestsData);
            Console.WriteLine("Request has been deleted.");
            Console.ReadKey();
            Menus.AidRequestorMenu();
        }
        public string GetValidatedInput(int line, string prompt, Func<string, bool> validationFunc, string errorMessage)
        {
            string input;
            while (true)
            {
                Console.SetCursorPosition(0, line);
                Console.Write(prompt);
                Console.Write(new string(' ', Console.WindowWidth - prompt.Length));
                Console.SetCursorPosition(prompt.Length, line);
                input = Console.ReadLine()?.Trim();

                if (validationFunc(input))
                {
                    Console.SetCursorPosition(0, line + 1);
                    Console.Write(new string(' ', Console.WindowWidth));
                    break;
                }
                Console.SetCursorPosition(0, line + 1);
                Console.Write(errorMessage);
            }
            return input;
        }
    }
}
