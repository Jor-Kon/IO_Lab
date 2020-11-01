using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ServerTCPLibrary
{
    /// <summary>
    /// Klasa abstrakcyjna serwera
    /// </summary>
    public abstract class Server
    {
        IPAddress ipAddress;
        int port;
        TcpListener tcpListener;
        bool isRunning;
        int bufferSize = 1024;


        protected TcpListener TcpListener { get => tcpListener; set => tcpListener = value; }
        public int Port { get => port; set => port = value; }
        public IPAddress IpAddress { get => ipAddress; set => ipAddress = value; }
        public int BufferSize { get => bufferSize; set => bufferSize = value; }
        public bool IsRunning { get => isRunning; set => isRunning = value; }

        /// <summary>
        /// Metoda rozpoczynająca działanie listenera
        /// </summary>
        protected void startListening()
        {
            TcpListener = new TcpListener(IpAddress, Port);
            TcpListener.Start();
        }

        protected abstract void acceptClient();

        protected abstract void Transmission(NetworkStream stream);

        public abstract void Start();

        /// <summary>
        /// Konstruktor obiekty klasy Server
        /// </summary>
        /// <param name="IP">Adres IP serwera</param>
        /// <param name="port">Numer portu serwera</param>
        public Server(IPAddress IP, int port)
        {
            IpAddress = IP;
            Port = port;
            IsRunning = false;
        }
    }
}
