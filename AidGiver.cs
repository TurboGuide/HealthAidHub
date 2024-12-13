using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace HealthAid_Hub_Final_
{
    internal class AidGiver : ItemRecord, IActions
    {
        public AidGiver() { }
        public AidGiver(string name, string contactnum, string item, string quantity, string location, string expdate) { }
        public void Donate()
        {
            bool donateAgain = true;

            while (donateAgain)
            {
                Console.Clear();
                Console.WriteLine("Donating Process:");
                int promptLine = Console.CursorTop;

                string name = GetValidatedInput(
                    promptLine,
                    "Enter your Name: ",
                    input => !string.IsNullOrEmpty(input) &&
                            !input.Any(char.IsDigit) &&
                            input.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)),
                    "Invalid input. Please use only letters and spaces (no digits or special characters)."
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

                string expDate = GetValidatedInput(
                    promptLine + 4,
                    "Enter expiration date (yyyy-mm-dd) or leave blank: ",
                    input => string.IsNullOrEmpty(input) || 
                    DateTime.TryParseExact(input, "yyyy-MM-dd", null, 
                    System.Globalization.DateTimeStyles.None, out _),
                    "Invalid date format. Please use yyyy-mm-dd or leave blank."
                );

                expDate = string.IsNullOrEmpty(expDate) ? "N/A" : expDate;

                string[] donationData = { name, contactNumber, itemName, quantity, expDate };
                CsvFileHandler.AppendToCsv("donations.csv", donationData);

                Console.WriteLine("\nDonation Added Succesfully!");

                string response = GetValidatedInput(
                    promptLine + 5,
                    "Do you want to donate again? (y/n): ",
                    input => input == "y" || input == "n",
                    "Invalid input. Please enter 'y' to donate again or 'n' to stop."
                );

                if (response == "n")
                {
                    donateAgain = false;
                    Menus.AidGiverMenu();
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
                    Console.WriteLine("\nPress any key to return to Aidgiver Menu.");
                    Console.ReadKey();
                    Menus.AidGiverMenu();
                }
            }
        }
        public void ViewRequests()
        {
            List<string[]> requestsData = CsvFileHandler.ReadFromCsv("requests.csv");

            Console.Clear();
            if (requestsData.Count == 0)
            {
                Console.WriteLine("There are no Requests available.");
                Console.WriteLine("\nPress any key to return to Aidgiver Menu");
                Console.ReadKey();
                Menus.AidGiverMenu();
            }
            else
            {
                bool Searching = true;

                while (Searching)
                {
                    var requestTable = new Table();
                    requestTable.Border = TableBorder.Square;
                    requestTable.ShowRowSeparators = true;
                    requestTable.Alignment(Justify.Right);

                    requestTable.AddColumn("No.");
                    requestTable.AddColumn("Item");
                    requestTable.AddColumn("Quantity");
                    requestTable.AddColumn("Location");

                    foreach (var request in requestsData)
                    {
                        requestTable.AddRow((requestsData.IndexOf(request) + 1).ToString(), request[2], request[3], request[4]);
                    }

                    Console.Clear();
                    Console.SetCursorPosition(63, 0);
                    Console.WriteLine("REQUESTS: ");
                    AnsiConsole.Write(requestTable);

                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine("Search by Item or Location (leave blank to view all): ");
                    string search = Console.ReadLine()?.Trim();

                    if (!string.IsNullOrEmpty(search))
                    {
                        var filteredRequests = requestsData
                            .Where(request => request[2].Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                              request[4].Contains(search, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (filteredRequests.Count > 0)
                        {
                            var filteredTable = new Table();
                            filteredTable.Border = TableBorder.Square;
                            filteredTable.ShowRowSeparators = true;
                            filteredTable.Alignment(Justify.Right);
                            filteredTable.AddColumn("No.");
                            filteredTable.AddColumn("Item");
                            filteredTable.AddColumn("Quantity");
                            filteredTable.AddColumn("Location");

                            foreach (var request in filteredRequests)
                            {
                                filteredTable.AddRow((filteredRequests.IndexOf(request) + 1).ToString(), request[2], request[3], request[4]);
                            }
                            Console.Clear();
                            Console.SetCursorPosition(63, 0);
                            Console.WriteLine("SEARCH RESULTS:");
                            AnsiConsole.Write(filteredTable);
                        }
                        else
                        {
                            Console.Clear();
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
                        Console.WriteLine("\nPress any key to return to Aidgiver Menu.");
                        Console.ReadKey();
                        Menus.AidGiverMenu();
                    }
                }
            }
        }
        public void ViewDonations()
        {
            Console.Clear();
            List<string[]> donationsData = CsvFileHandler.ReadFromCsv("donations.csv");

            if (donationsData.Count == 0)
            {
                Console.WriteLine("There are no Donations available.");
                Console.WriteLine("\nPress any key to return to Aidgiver Menu");
                Console.ReadKey();
                Menus.AidGiverMenu();
            }
            else
            {
                bool Searching = true;

                while (Searching)
                {
                    Console.Clear();
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

                    Console.SetCursorPosition(62, 0);
                    Console.WriteLine("DONATIONS: ");
                    AnsiConsole.Write(donationTable);

                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine("Search by Name or Item (leave blank to view all): ");
                    string search = Console.ReadLine()?.Trim();

                    List<string[]> filteredDonations = donationsData;

                    if (!string.IsNullOrEmpty(search))
                    {
                        filteredDonations = donationsData
                            .Where(donation => donation[0].Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                               donation[2].Contains(search, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        Console.Clear();
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
                            filteredTable.AddColumn("Expiration Date");

                            foreach (var donation in filteredDonations)
                            {
                                filteredTable.AddRow((filteredDonations.IndexOf(donation) + 1).ToString(), donation[0], donation[1], donation[2], donation[3], donation[4]);
                            }

                            Console.SetCursorPosition(62, 0);
                            Console.WriteLine("SEARCH RESULTS:");
                            AnsiConsole.Write(filteredTable);

                            do
                            {
                                Console.SetCursorPosition(0, 0);
                                Console.WriteLine("\nOptions: ");
                                Console.WriteLine("1. Update a Donation");
                                Console.WriteLine("2. Delete a Donation");
                                Console.WriteLine("3. Delete All Donations");
                                Console.WriteLine("4. Return to Aidgiver Menu");
                                Console.Write("\nChoose an option: ");
                                string option = Console.ReadLine()?.Trim();

                                switch (option)
                                {
                                    case "1":
                                        Console.Write("\nEnter the ID of the donation you want to update: ");
                                        int updateId = int.Parse(Console.ReadLine()?.Trim() ?? "0");

                                        if (updateId > 0 && updateId <= filteredDonations.Count)
                                        {
                                            var donationToUpdate = filteredDonations[updateId - 1];
                                            UpdateDonation(donationToUpdate, donationsData);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid ID.");
                                        }
                                        break;

                                    case "2":
                                        Console.Write("\nEnter the ID of the donation you want to delete: ");
                                        int deleteId = int.Parse(Console.ReadLine()?.Trim() ?? "0");

                                        if (deleteId > 0 && deleteId <= filteredDonations.Count)
                                        {
                                            DeleteDonation(filteredDonations[deleteId - 1], donationsData);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid ID.");
                                        }
                                        break;

                                    case "3":
                                        Console.Write("\nAre you sure you want to delete all searched donations? (y/n): ");
                                        string deleteAllResponse = Console.ReadLine()?.Trim().ToLower();

                                        if (deleteAllResponse == "y")
                                        {
                                            foreach (var donation in filteredDonations)
                                            {
                                                donationsData.Remove(donation);
                                            }
                                            CsvFileHandler.WriteToCsv("donations.csv", donationsData);
                                            Console.WriteLine("All searched donations have been deleted.");
                                            Console.ReadKey();
                                            Menus.AidGiverMenu();
                                        }
                                        break;

                                    case "4":
                                        Searching = false;
                                        Menus.AidGiverMenu();
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
                        Console.WriteLine("\nPress any key to return to Aidgiver Menu.");
                        Console.ReadKey();
                        Menus.AidGiverMenu();
                    }
                }
            }
        }
        private void UpdateDonation(string[] donationToUpdate, List<string[]> allDonations)
        {
            Console.Clear();
            Console.WriteLine("Update Donation:");

            Console.Write($"Current Name: {donationToUpdate[0]}\nEnter new Name (or press Enter to keep): ");
            string newName = Console.ReadLine()?.Trim();
            donationToUpdate[0] = string.IsNullOrEmpty(newName) ? donationToUpdate[0] : newName;

            Console.Write($"Current Contact: {donationToUpdate[1]}\nEnter new Contact (or press Enter to keep): ");
            string newContact = Console.ReadLine()?.Trim();
            donationToUpdate[1] = string.IsNullOrEmpty(newContact) ? donationToUpdate[1] : newContact;

            Console.Write($"Current Item: {donationToUpdate[2]}\nEnter new Item (or press Enter to keep): ");
            string newItem = Console.ReadLine()?.Trim();
            donationToUpdate[2] = string.IsNullOrEmpty(newItem) ? donationToUpdate[2] : newItem;

            Console.Write($"Current Quantity: {donationToUpdate[3]}\nEnter new Quantity (or press Enter to keep): ");
            string newQuantity = Console.ReadLine()?.Trim();
            donationToUpdate[3] = string.IsNullOrEmpty(newQuantity) ? donationToUpdate[3] : newQuantity;

            Console.Write($"Current Expiration Date: {donationToUpdate[4]}\nEnter new Expiration Date (or press Enter to keep): ");
            string newExpDate = Console.ReadLine()?.Trim();
            donationToUpdate[4] = string.IsNullOrEmpty(newExpDate) ? donationToUpdate[4] : newExpDate;

            CsvFileHandler.WriteToCsv("donations.csv", allDonations);

            Console.WriteLine("\nDonation updated successfully and saved to file.");
            Console.WriteLine("Press any key to return.");
            Console.ReadKey();
            Menus.AidGiverMenu();
        }
        private void DeleteDonation(string[] donation, List<string[]> donationsData)
        {
            donationsData.Remove(donation);
            CsvFileHandler.WriteToCsv("donations.csv", donationsData);
            Console.WriteLine("Donation has been deleted.");
            Console.ReadKey();
            Menus.AidGiverMenu();
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
