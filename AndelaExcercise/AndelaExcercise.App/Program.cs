using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AndelaExcercise.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var events = new List<Event>{
                new Event{ Name = "Phantom of the Opera", City = "New York"},
                new Event{ Name = "Metallica", City = "Los Angeles"},
                new Event{ Name = "Metallica", City = "New York"},
                new Event{ Name = "Metallica", City = "Boston"},
                new Event{ Name = "LadyGaGa", City = "New York"},
                new Event{ Name = "LadyGaGa", City = "Boston"},
                new Event{ Name = "LadyGaGa", City = "Chicago"},
                new Event{ Name = "LadyGaGa", City = "San Francisco"},
                new Event{ Name = "LadyGaGa", City = "Washington"}
            };
            
            var customer = new Customer { Name = "Mr. Fake", City = "New York" };

            //1.1 - Use Linq to get all events from customer city
            var eventsInCustomerCity = events.Where(e => e.City == customer.City).ToList();
            
            //1.2 - Call AddToEmail methodo for each events in customer city
            foreach (var item in eventsInCustomerCity)
            {
                AddToEmail(customer, item);
            }

            //1.3 - It will depends where John Smith lives

            //1.4 - Yes, we can create unit tests, aply some SOLID principles like single responsability,
            //create a separeted method tha will receive a customer and list of events and do the logic to add email 


            //2.1 - Create a Dictionary<string, int> with city (key) and distance (value)
            var dictCityDistance = new Dictionary<string, int>();
            foreach (var item in events)
            {
                if (!dictCityDistance.ContainsKey(item.City))
                {
                    var distance = GetDistance(customer.City, item.City);
                    dictCityDistance[item.City] = distance;
                }
            }

            //2.2 - Sorte the discrionary by value (distance) and then create a list of events (5) using the sorted disctionary 
            var sortedDictCityDistance = from entry in dictCityDistance orderby entry.Value ascending select entry;
            var totalEventsToSend = 5;
            var eventsToSendAnEmail = new List<Event>();

            foreach (var item in sortedDictCityDistance)
            {
                if (eventsToSendAnEmail.Count == totalEventsToSend)
                    break;

                eventsToSendAnEmail.AddRange(events.Where(e => e.City == item.Key).Take(totalEventsToSend - eventsToSendAnEmail.Count));
            }

            foreach (var item in eventsToSendAnEmail)
            {
                AddToEmail(customer, item);
            }

            //2.3 - It will depends where John Smith lives

            //2.4 - I could create separeted methods for example:
            //    * a method to return a sorted list with cities
            //    * a method that receives a list of events and a customer to send email

            //3 - Since I'm already using a dictionary, what I can do is to use 
            //    Parallel.ForEach with ConcurrentDictionary
            var concuDictCityDistance = new ConcurrentDictionary<string, int>();
            Parallel.ForEach(events, item => {
                if (!concuDictCityDistance.ContainsKey(item.City))
                {
                    var distance = GetDistance(customer.City, item.City);
                    concuDictCityDistance[item.City] = distance;
                }
            });
        }

        // You do not need to know how these methods work
        static void AddToEmail(Customer c, Event e, int? price = null)
        {
            var distance = GetDistance(c.City, e.City);
            Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
            + (distance > 0 ? $" ({distance} miles away)" : "")
            + (price.HasValue ? $" for ${price}" : ""));
        }
        
        static int GetPrice(Event e)
        {
            return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
        }
        
        static int GetDistance(string fromCity, string toCity)
        {
            return AlphebiticalDistance(fromCity, toCity);
        }
        
        private static int AlphebiticalDistance(string s, string t)
        {
            var result = 0;
            var i = 0;
            for (i = 0; i < Math.Min(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
            }
            for (; i < Math.Max(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
            }
            return result;
        }

    }
    
    public class Event
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
    
    public class Customer
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
}
