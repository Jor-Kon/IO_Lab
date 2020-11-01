using ServerTCPLibrary;
using System.Net;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerSync sc = new ServerSync(IPAddress.Parse("127.0.0.1"), 1024);
            sc.Start();
        }
    }
}
