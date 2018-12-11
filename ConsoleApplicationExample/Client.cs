using System;
using System.Threading.Tasks;

namespace ConsoleApplicationExample
{
    public class Client : IClient
    {
        private readonly string _url;

        public Client(string url)
        {
            _url = url;
        }

        public Task SendRequestAsync()
        {
            Console.WriteLine($"Connecting to {_url}");
            return Task.CompletedTask;
        }
    }
}