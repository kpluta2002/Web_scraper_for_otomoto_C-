using HtmlAgilityPack;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Linq;

namespace Web_Scraper
{
    class Program
    {
        public class Vehicle
        {
            public string Name { get; set; }
            public string Details { get; set; }
            public string Mileage { get; set; }
            public string FuelType { get; set; }
            public string Gearbox { get; set; }
            public string Year { get; set; }
            public string Location { get; set; }


            public virtual void DisplayDetails()
            {
                Console.WriteLine($"Vehicle: {Name}");
                Console.WriteLine($"Details: {Details}");
                Console.WriteLine($"Mileage: {Mileage}");
                Console.WriteLine($"Fuel Type: {FuelType}");
                Console.WriteLine($"Gearbox: {Gearbox}");
                Console.WriteLine($"Year: {Year}");
                Console.WriteLine($"Location: {Location}");
            }
        }

        public class CarDetails : Vehicle
        {
            // Additional properties or methods specific to DerivedClass can be added here

            // Override the DisplayDetails method to provide a specific implementation
            public override void DisplayDetails()
            {
                base.DisplayDetails();
                Console.WriteLine("Additional details specific to DerivedClass.");
            }
        }

        public class Offers
        {
            public List<Vehicle> Vehicles { get; } = new List<Vehicle>();

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
                    Location = singleResult.SelectSingleNode(".//dd[@class='ooa-16w655c ev7e6t83']").InnerText
                };
            }
        }

        static void DisplayCars(Offers cars)
        {
            foreach (var car in cars)
            {
                Console.WriteLine(car.Name);
            }
        }
        static void Main()
        {
            Offers offers = new Offers();

            Console.Write("Podaj marke samochodu do wyszukania: ");
            string searchKeyword = Console.ReadLine();

            offers.Scraper(searchKeyword);

            while (true)
            {
                Console.WriteLine(" ---------------------Menu---------------------");
                Console.WriteLine("|     1. Display all found cars                |");
                Console.WriteLine("|     2. Display details of a car              |");
                Console.WriteLine("|     3. Display a average price of a car      |");
                Console.WriteLine("|     4. Display cars by voivodeship           |");
                Console.WriteLine("|     5. Display by price ASC/DESC             |");
                Console.WriteLine("|     6. Display            |");



                switch (Console.ReadLine())
                {
                    case 1:

                }
            }
            
        }
    }
}
