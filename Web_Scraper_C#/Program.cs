using HtmlAgilityPack;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Linq;
using static Web_Scraper.Program;
using System.Runtime.InteropServices.Marshalling;
using System.Reflection;

namespace Web_Scraper
{
    class Program
    {
        
        public class Vehicle
        {
            public string Name { get; set; }
            public string Link { get; set; }
            public string Details { get; set; }
            public string Mileage { get; set; }
            public string FuelType { get; set; }
            public string Gearbox { get; set; }
            public string Year { get; set; }
            public string Location { get; set; }
            public string Price {  get; set; }
        }

        // CarDetails inherits from class Vehicle //
        public class MenuMethods : Vehicle
        {
            // Displays all of the scraped vehicles //
            public virtual void DisplayAllVehicles(List<Vehicle> vehicles)
            {
                int i = 1;
                Console.Clear();
                Console.WriteLine("[]--------------------------------------------------------------------------------------[]");

                foreach (var vehicle in vehicles)
                {
                    Console.WriteLine($"                         {i}. {vehicle.Name}");
                    i++;
                }
                Console.WriteLine("[]--------------------------------------------------------------------------------------[]");
                Console.WriteLine(" ");

            }

            // Displays details of car that's number has been provided //
            public virtual void DisplayCarDetails(int car, List<Vehicle> vehicles)
            {

                Console.WriteLine($"[]-----------------------------------------------------------------------------------[]");
                Console.WriteLine($"                                         {car}                                         ");
                Console.WriteLine($"[]-----------------------------------------------------------------------------------[]");
                Console.WriteLine(" ");
                Console.WriteLine($"Vehicle: {vehicles[car - 1].Name}");
                Console.WriteLine($"Details: {vehicles[car - 1].Details}");
                Console.WriteLine($"Mileage: {vehicles[car - 1].Mileage}");
                Console.WriteLine($"Fuel Type: {vehicles[car - 1].FuelType}");
                Console.WriteLine($"Gearbox: {vehicles[car - 1].Gearbox}");
                Console.WriteLine($"Year: {vehicles[car - 1].Year}");
                Console.WriteLine($"Location: {vehicles[car - 1].Location}");
                Console.WriteLine($"Price in PLN: {vehicles[car - 1].Price}");
                Console.WriteLine($"Link: {vehicles[car - 1].Link}");

                Console.WriteLine(" ");
                Console.WriteLine("[]--------------------------------------------------------------------------------------[]");
                Console.WriteLine(" ");
            }

            private string PriceRange(List<int> prices)
            {
                double min = Queryable.Min(prices.AsQueryable());
                double max = Queryable.Max(prices.AsQueryable());
                string res = $"{min} - {max} PLN";
                return res;
            }

            // Method which calculates average price of model found //
            private void ModelAveragePrice(string model, List<Vehicle> vehicles)
            {
                List<int> pricesOfModel = new List<int>();



                
                while (!pricesOfModel.Any())
                {

                    foreach (var vehicle in vehicles)
                    {
                        if (vehicle.Name.ToLower().Replace(" ", "").Contains(model.ToLower().Replace(" ", "")))
                        {
                            pricesOfModel.Add(Int32.Parse(vehicle.Price.Replace(" ", "")));
                        }
                    }

                    if (pricesOfModel.Any())
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("The model you specified doesn't exist. Would you like to try another model?(Y/N)");
                        while (true)
                        {
                            string yn = Console.ReadLine();
                            if (yn.ToLower() == "y")
                            {
                                Console.Write("Specify model: ");
                                model = Console.ReadLine();
                                break;
                            }
                            else if (yn.ToLower() == "n")
                            {
                                return;
                            }
                            else
                            {
                                Console.Write("You need to type Y or N: ");
                            }
                        }
                    }
                }

                double avgOfModel = Queryable.Average(pricesOfModel.AsQueryable());
                Console.WriteLine($"Average price of {model} is : {avgOfModel}");
                Console.WriteLine($"Would you like to see price range of {model}?(Y/N):");
                while (true)
                {
                    string yn = Console.ReadLine();
                    if (yn.ToLower() == "y")
                    {
                        Console.WriteLine($"Price range is {PriceRange(pricesOfModel)}.");
                        return;
                    }
                    else if (yn.ToLower() == "n")
                    {
                        return;
                    }
                    else
                    {
                        Console.Write("You need to type Y or N: ");
                    }
                }
            }

            private void AllAveragePrice(List<Vehicle> vehicles)
            {
                List<int> pricesOfAll = new List<int>();
                foreach (var vehicle in vehicles)
                {
                    pricesOfAll.Add(Int32.Parse(vehicle.Price.Replace(" ", "")));
                }
                double avg = Queryable.Average(pricesOfAll.AsQueryable());
                Console.WriteLine($"Average price of all vehicles is:  {avg}  PLN");
                Console.WriteLine($"Would you like to see price range of all vehicles?(Y/N):");
                while (true)
                {
                    string yn = Console.ReadLine();
                    if (yn.ToLower() == "y")
                    {
                        Console.WriteLine($"Price range is {PriceRange(pricesOfAll)}.");
                    }
                    else if (yn.ToLower() == "n")
                    {
                        return;
                    }
                    else
                    {
                        Console.Write("You need to type Y or N: ");
                    }
                }
            }
            public virtual void VehiclesAveragePrice(List<Vehicle> vehicles)
            {
                Console.Write("Would you like to display average price of a model or all vehicles?(Model/All) : ");
                while (true)
                {
                    string which = Console.ReadLine();

                    if (which.ToLower() == "all")
                    {
                        AllAveragePrice(vehicles);
                        return;
                    }
                    else if (which.ToLower() == "model")
                    {
                        while (true)
                        {
                            Console.Write("Specify model or press enter to go back: ");
                            string model = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(model))
                            {
                                return;
                            }
                            else
                            {
                                ModelAveragePrice(model, vehicles);
                            }
                            
                        }
                    }
                    else
                    {
                        Console.Write("You need to specify if you want to see average price of model or all of the cars (Model/All): ");
                    }
                }
            }
        }

        // Offers class which holds list of vehicle offers //
        public class Offers
        {
            public List<Vehicle> Vehicles { get; } = new List<Vehicle>();

            // Scraper method responsible for getting data from a page //
            public virtual void Scraper(string searchKeyword)
            {
                // Send a get request to otomoto.pl
                String url = string.Concat("https://www.otomoto.pl/osobowe/" + searchKeyword);
                var httpClient = new HttpClient();
                var html = httpClient.GetStringAsync(url).Result;
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                // Get the container with offers
                var resultsElement = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='ooa-1hab6wx er8sc6m9']");
                HtmlNodeCollection singleResults = resultsElement.ChildNodes;

                Console.WriteLine("Wait! Scraping data...");
                int countCatch = 0, countSuccess = 0;

                foreach (var singleResult in singleResults)
                {
                    
                    try
                    {
                        // Encapsulate the scraping logic
                        var vehicle = ScrapeVehicle(singleResult);
                        Vehicles.Add(vehicle);
                        countSuccess++;
                    }
                    catch (Exception e)
                    {
                        countCatch++;
                    }
                }

                Console.WriteLine($"Scraped {countSuccess} offers out of {countSuccess + countCatch}");
            }

            // Encapsulate the scraping logic in a separate method
            protected virtual Vehicle ScrapeVehicle(HtmlNode singleResult)
            {
                return new Vehicle
                {
                    Name = singleResult.SelectSingleNode(".//h1[@class='ev7e6t89 ooa-1xvnx1e er34gjf0']").InnerText,


                    Details = singleResult.SelectSingleNode(".//p[@class='ev7e6t88 ooa-17thc3y er34gjf0']").InnerText,
                    Mileage = singleResult.SelectSingleNode(".//dd[@data-parameter='mileage']").InnerText,
                    FuelType = singleResult.SelectSingleNode(".//dd[@data-parameter='fuel_type']").InnerText,
                    Gearbox = singleResult.SelectSingleNode(".//dd[@data-parameter='gearbox']").InnerText,
                    Year = singleResult.SelectSingleNode(".//dd[@data-parameter='year']").InnerText,
                    Location = singleResult.SelectSingleNode(".//dd[@class='ooa-16w655c ev7e6t83']").InnerText,
                    Price = singleResult.SelectSingleNode(".//h3[@class='ev7e6t82 ooa-bz4efo er34gjf0']").InnerText,
                    Link = singleResult.SelectSingleNode(".//h1[@class='ev7e6t89 ooa-1xvnx1e er34gjf0']").ChildNodes["a"].GetAttributeValue("href", string.Empty)


                };
            }
        }

        // Input handler for menu's switch-case //
        public static int inputHandlerExit(string inputString)
        {
            int input;
            if (!int.TryParse(inputString, out input))
            {
                System.Environment.Exit(0);
            }
            return input;
        }

        // Input handler which checks if input is not a string and is in range of list of cars otherwise returns 0 which means to break case //
        public static int inputHandlerBreak(string inputString, List<Vehicle> offers)
        {
            int input;
            if (int.TryParse(inputString, out input) && input <= offers.Count)
            {
                return input;
            }
            return 0;
        }

        // Menu which handles the whole process while app is running //
        public static void Menu(List<Vehicle> vehicles)
        {
            MenuMethods method = new MenuMethods();
            

            while (true)
            {
                Console.WriteLine("*----------------------------------------------*");
                Console.WriteLine("|                     Menu                     |");
                Console.WriteLine("*----------------------------------------------*");
                Console.WriteLine("|     1. Display all found cars                |");
                Console.WriteLine("|     2. Display details of a car              |");
                Console.WriteLine("|     3. Display a average price of cars       |");
                Console.WriteLine("|     4. Display cars by voivodeship           |");
                Console.WriteLine("|     5. Display by price ASC/DESC             |");
                Console.WriteLine("|     6. Open car offer in browser             |");
                Console.WriteLine("|     7. Scrape different make                 |");
                Console.WriteLine("*----------------------------------------------*");
                Console.WriteLine("|     Press any other key to close program     |");
                Console.WriteLine("*----------------------------------------------*");
                Console.Write("");
                int inputSwitch = inputHandlerExit(Console.ReadLine());

                switch (inputSwitch)
                {
                    case 1:
                        method.DisplayAllVehicles(vehicles);
                        Console.Write("Press any key to go back: "); if (!string.IsNullOrEmpty(Console.ReadLine())) { Console.Clear();  break; } Console.Clear(); break;
                    case 2:
                        method.DisplayAllVehicles(vehicles);

                        while (true)
                        {
                            Console.Write("Specify number of a car to be displayed or type any other key to go back: ");
                            int input = inputHandlerBreak(Console.ReadLine(), vehicles);
                            Console.WriteLine(" ");

                            if (input < 1) { Console.Clear(); break; }
                            method.DisplayCarDetails(input, vehicles);
                        }
                        Console.Clear();
                        break;
                    case 3:
                        method.VehiclesAveragePrice(vehicles);
                        Console.Clear(); 
                        break;

                    case 4:

                        break;
                    case 5:

                        break;
                    case 6:

                        break;
                    case 7:

                        break;


                    default:
                        System.Environment.Exit(0); break;

                }
            }
        }


        static void Main()
        {
            Offers offers = new Offers();
            
            Console.WriteLine("Welcome to Otomoto web scraper!");
            Console.Write("What kind of car make are you looking for? : ");
            string searchKeyword = Console.ReadLine();

            offers.Scraper(searchKeyword);

            
            Menu(offers.Vehicles);
            
            
        }
    }
}
