using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplicationExample
{
    interface IClient
    {
        Task SendRequestAsync();
    }
}
