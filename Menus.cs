using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace HealthAid_Hub_Final_
{
    public class Menus
    {
        public static void MainMenu()
        {
            string title = "Welcome to HealthAid Hub!\n";
            string[] options = { "Aid Giver", "Aid Requestor", "Aid Administrator", "Exit" };
            MenuNavigation mainMenu = new MenuNavigation(title, options);
            int choice = mainMenu.RunNavigation();

            switch (choice)
            {
                case 0:
                    AidGiverMenu();
                    break;
                case 1:
                    AidRequestorMenu();
                    break;
                case 2:
                    AdminLogin();
                    break;
                case 3:
                    Console.WriteLine("Thank you for using HealthAid Hub. Goodbye!");
                    Environment.Exit(0);
                    break;
            }
        }
        public static void AidGiverMenu()
        {
            string title = "Aid Giver Menu\n";
            string[] options = { "Donate", "View Inventory",
                "View Requests", "View Donations", "Back to MainMenu" };
            MenuNavigation aidgiverMenu = new MenuNavigation(title, options);
            int AidGchoice = aidgiverMenu.RunNavigation();
            AidGiver aidGiver = new AidGiver();

            switch (AidGchoice) {
                case 0:
                    aidGiver.Donate();
                    break;
                case 1:
                    aidGiver.ViewInventory();
                    break;
                case 2:
                    aidGiver.ViewRequests();
                    break;
                case 3:
                    aidGiver.ViewDonations();
                    break;
                case 4:
                    MainMenu();
                    break;
            }
        }
        public static void AidRequestorMenu()
        {
            string title = "Aid Requestor Menu\n";
            string[] options = { "Request Delivery", "View Inventory",
                                "View Requests", "Back to MainMenu" };
            MenuNavigation aidrequestorMenu = new MenuNavigation(title, options);
            int AidRchoice = aidrequestorMenu.RunNavigation();
            AidRequestor aidRequestor = new AidRequestor();

            switch (AidRchoice)
            {
                case 0:
                    aidRequestor.RequestDelivery();
                    break;
                case 1:
                    aidRequestor.ViewInventory();
                    break;
                case 2:
                    aidRequestor.ViewRequests();
                    break;
                case 3:
                    MainMenu();
                    break;
            }
        }
        public static void AidAdminMenu()
        {
            string title = "Aid Administrator Menu\n";
            string[] options = { "View Inventory", "View Donations", "View And Deliver Requests", "View Delivered Items", "Logout" };
            MenuNavigation aidAdminMenu = new MenuNavigation(title, options);
            int AidAchoice = aidAdminMenu.RunNavigation();
            AidAdmin aidAdmin = new AidAdmin();

            switch (AidAchoice)
            {
                case 0:
                    aidAdmin.ViewInventory();
                    break;
                case 1:
                    aidAdmin.ViewDonations();
                    break;
                case 2:
                    aidAdmin.ViewAndDeliverRequests();
                    break;
                case 3:
                    aidAdmin.ViewDeliveredItems();
                    break;
                case 4:
                    MainMenu();
                    break;
            }
        }
        public static void AdminLogin()
        {
            Console.Clear();
            Console.WriteLine("Aid Administrator Login\n");
            Console.Write("Enter admin name: ");
            string name = Console.ReadLine();
            Console.Write("Enter admin password: ");
            string password = Console.ReadLine();

            if (name == "admin" && password == "admin123")
            {
                AidAdminMenu();
            }
            else
            {
                Console.WriteLine("Invalid Manager Credentials.");
                Console.WriteLine("\nPress any key to return to Main Menu");
                Console.ReadKey();
                MainMenu();
            }

        }
    }
    public class MenuNavigation
    {
        private int SelectedIndex;
        private string[] Options;
        private string Title;

        public MenuNavigation(string title, string[] options)
        {
            Title = title;
            Options = options;
            SelectedIndex = 0;
        }
        public void DisplayOptions()
        {
            Console.WriteLine(Title);
            for (int i = 0; i < Options.Length; i++)
            {
                string currentOption = Options[i];
                if (i == SelectedIndex)
                {
                    ForegroundColor = ConsoleColor.Black;
                    BackgroundColor = ConsoleColor.White;
                }
                else
                {
                    ForegroundColor = ConsoleColor.White;
                    BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine($"[{currentOption}]");
            }
            ResetColor();
        }
        public int RunNavigation()
        {
            ConsoleKey pressedKey;
            do
            {
                Console.Clear();
                DisplayOptions();

                ConsoleKeyInfo keyInfo = ReadKey(true);
                pressedKey = keyInfo.Key;

                if (pressedKey == ConsoleKey.UpArrow || pressedKey == ConsoleKey.W)
                {
                    SelectedIndex--;
                    if (SelectedIndex == -1)
                    {
                        SelectedIndex = Options.Length - 1;
                    }
                }
                else if (pressedKey == ConsoleKey.DownArrow || pressedKey == ConsoleKey.S)
                {
                    SelectedIndex++;
                    if (SelectedIndex == Options.Length)
                    {
                        SelectedIndex = 0;
                    }
                }
            } while (pressedKey != ConsoleKey.Enter);
            return SelectedIndex;
        }
    }
}
