using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;

namespace Moonshot.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                // Create HttpCient and make a request to api/values 
                var client = new HttpClient();
                var response = client.GetAsync(baseAddress + "api/Thing").Result;

                if (response != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Information from service ({0}): {1}", response.StatusCode, result);
                }
                else
                {
                    Console.WriteLine("ERROR: Impossible to connect to service");
                }

                Console.WriteLine();
                Console.WriteLine("Press ENTER to stop the server and close app...");
                Console.ReadLine();
            } 
        }
    }
}
