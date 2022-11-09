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

            //1.2 - Call AddListToEmail method for each events in customer city
            Console.Out.WriteLine("Answer Question 1 \n");
            AddListToEmail(customer, eventsInCustomerCity);

            //1.3 - It will depends where John Smith lives

            //1.4 - Yes, we can.
            //  * Apply some SOLID principles like single responsibility;
            //  * Create a method that receives a list of events and a customer to send email (I did that at the end)


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

            //2.2 - Sort the dictionary by value (distance) and then create a list of events (5) using the sorted dictionary
            var sortedDictCityDistance = from entry in dictCityDistance orderby entry.Value ascending select entry;
            var totalEventsToSend = 5;
            var eventsToSendAnEmail = new List<Event>();

            foreach (var item in sortedDictCityDistance)
            {
                if (eventsToSendAnEmail.Count == totalEventsToSend)
                    break;

                eventsToSendAnEmail.AddRange(events.Where(e => e.City == item.Key).Take(totalEventsToSend - eventsToSendAnEmail.Count));
            }

            Console.Out.WriteLine("\n\nAnswer Question 2 \n");
            AddListToEmail(customer, eventsToSendAnEmail);

            //2.3 - It will depends where John Smith lives

            //2.4 - Yes, we can:
            //  * Apply some SOLID principles like single responsibility;
            //  * Create a method to return a sorted list with cities
            //  * Create a method that receives a list of events and a customer to send email (I did that at the end)

            //3 - Since I'm already using a dictionary, what I can do is to use 
            //    Parallel.ForEach with ConcurrentDictionary and have a try catch
            var concuDictCityDistance = new ConcurrentDictionary<string, int>();
            Parallel.ForEach(events, item => {
                try
                {
                    if (!concuDictCityDistance.ContainsKey(item.City))
                    {
                        var distance = GetDistance(customer.City, item.City);
                        concuDictCityDistance[item.City] = distance;
                    }
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine($"{customer.Name}: {item.Name} in {item.City}"
                        + $" Exception: {e.Message}");
                }
            });

            var sortedConcuDictCityDistance = from entry in concuDictCityDistance orderby entry.Value ascending select entry;
            var eventsToSendAnEmailQuestion3 = new List<Event>();

            foreach (var item in sortedConcuDictCityDistance)
            {
                if (eventsToSendAnEmailQuestion3.Count == totalEventsToSend)
                    break;

                eventsToSendAnEmailQuestion3.AddRange(events.Where(e => e.City == item.Key).Take(totalEventsToSend - eventsToSendAnEmailQuestion3.Count));
            }

            Console.Out.WriteLine("\n\nAnswer Question 3 \n");
            AddListToEmail(customer, eventsToSendAnEmailQuestion3);

            //4 - We can use the code written in question 3 for the question 4
            Console.Out.WriteLine("\n\nAnswer Question 4 \n");
            AddListToEmail(customer, eventsToSendAnEmailQuestion3);

            //5 - We can use OrdeynBy and ThenBy
            var eventsOrderbyDistanceAndPrice = events.OrderBy(e => dictCityDistance[e.City]).ThenBy(x => GetPrice(x)).Take(5);

            Console.Out.WriteLine("\n\nAnswer Question 5 \n");
            AddListToEmail(customer, eventsOrderbyDistanceAndPrice);
        }

        // To be use in all questions
        static void AddListToEmail(Customer customer, IEnumerable<Event> events)
        {
            foreach (var item in events)
            {
                AddToEmail(customer, item);
            }
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
