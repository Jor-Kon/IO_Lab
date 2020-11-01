using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerTCPLibrary
{
    /// <summary>
    /// Klasa implementująca serwer TPC połączony z bazą danych przechowującą dane użytkowników (login, hasło), który pełni funkcję prostego kalkulatora
    /// </summary>
    public class ServerSync : Server
    {
        public delegate void TransmissionDelegate(NetworkStream stream);
        
        /// <summary>
        /// Konstruktor obiektu klasy ServerSync
        /// </summary>
        /// <param name="IP">Adres IP serwera</param>
        /// <param name="port">Numer portu serwera</param>
        public ServerSync(IPAddress IP, int port) : base (IP, port){}

        /// <summary>
        /// Metoda oczekująca na połączenie klienta oraz rozpoczynająca transmisję danych
        /// </summary>
        protected override void acceptClient()
        {
            while (IsRunning == true)
            {
                byte[] buffer = new byte[BufferSize];
                TcpClient TcpClient = TcpListener.AcceptTcpClient();
                NetworkStream Stream = TcpClient.GetStream();
                TransmissionDelegate transmissionDelegate = new TransmissionDelegate(Transmission);
                transmissionDelegate.BeginInvoke(Stream, TransmissionCallback, TcpClient);                
            }
        }

        /// <summary>
        /// Metoda odpowiadająca za transmisję danych
        /// </summary>
        /// <param name="Stream"></param>
        protected override void Transmission(NetworkStream Stream)
        {
            string menu = "1.Calculator\n\r2.Exit\n\r";

            byte[] buffer = new byte[BufferSize];
            Account acc = null;
            bool isLoggedIn = false;
            string opt = "";
            try
            {
                while(true)
                {
                    sendMsgToClient("1.Log in\n\r2.Sign up\r\n3.Exit\n\r", Stream);
                    opt = readMsgFromClient(Stream);

                    if (opt == "1")
                    {
                        isLoggedIn = logIn(Stream);
                    }
                    else if (opt == "2")
                    {
                        try
                        {
                            addUser(Stream);
                        }
                        catch (System.Data.SQLite.SQLiteException)
                        {
                            sendMsgToClient("Login is unavailable\n\r", Stream);
                        }
                    }
                    else if (opt == "3")
                    {
                        break;
                    }
                    else
                    {
                        sendMsgToClient("Choose option 1-3", Stream);
                    }

                    while (isLoggedIn)
                    {
                        sendMsgToClient("1.Calculator\n\r2.Log out\n\r", Stream);
                        opt = readMsgFromClient(Stream);
                        if (opt == "1")
                        {
                            while (true)
                            {
                                sendMsgToClient("1.Addition\n\r2.Division\n\r3.Multipication\n\r4.Division\n\r5.Exit\n\r", Stream);
                                int operation = Convert.ToInt32(readMsgFromClient(Stream));
                                if (operation == 5) break;
                                sendMsgToClient("Enter two numbers\n\r", Stream);

                                double a;
                                double b;
                                string first = readMsgFromClient(Stream);
                                string second = readMsgFromClient(Stream);
                                if (first.All(char.IsDigit) && second.All(char.IsDigit))
                                {
                                    a = Convert.ToDouble(first);
                                    b = Convert.ToDouble(second);
                                }
                                else
                                {
                                    sendMsgToClient("Entered non-numeric string!\n\r", Stream);
                                    continue;
                                }
                                switch (operation)
                                {
                                    case 1:
                                        {
                                            sendMsgToClient($"{a} + {b} = {a + b}\n\r", Stream);
                                            break;
                                        }
                                    case 2:
                                        {
                                            sendMsgToClient($"{a} - {b} = {a - b}\n\r", Stream);
                                            break;
                                        }
                                    case 3:
                                        {
                                            sendMsgToClient($"{a} * {b} = {a * b}\n\r", Stream);
                                            break;
                                        }
                                    case 4:
                                        {
                                            sendMsgToClient($"{a} / {b} = {a / b}\n\r", Stream);
                                            break;
                                        }
                                    default:
                                        {
                                            sendMsgToClient("Choose option 1-5", Stream);
                                            break;
                                        }
                                }

                            }
                        }
                        else if (opt == "2") isLoggedIn = false;
                        else
                            sendMsgToClient("Choose option 1-2", Stream);
                    }

                } 
                
            }
            catch
            {

            }
        }

        private void TransmissionCallback(IAsyncResult client)
        {
            TcpClient tcpClient = (TcpClient)client.AsyncState;
            tcpClient.Close();
        }

        /// <summary>
        /// Metoda wysyłająca wiadomość do klienta
        /// </summary>
        /// <param name="msg">Wysyłana wiadomość</param>
        /// <param name="Stream">Strumień danych nawiązany z danym klientem</param>
        protected void sendMsgToClient(string msg, NetworkStream Stream)
        {
            byte[] buffer = new byte[BufferSize];
            buffer = Encoding.ASCII.GetBytes(msg);
            Stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Metoda oczekująca na otrzymanie wiadomości od klienta
        /// </summary>
        /// <param name="Stream">Strumień danych nawiązany z danym klientem</param>
        /// <returns></returns>
        protected string readMsgFromClient(NetworkStream Stream)
        {
            byte[] buffer = new byte[BufferSize];
            int msgSize = Stream.Read(buffer, 0, buffer.Length);
            if (buffer[0] == 13 && buffer[1] == 10)
            {
                msgSize = Stream.Read(buffer, 0, buffer.Length);
            }
            return Encoding.ASCII.GetString(buffer, 0, msgSize);
        }

        /// <summary>
        /// Metoda sprawdzająca czy dane logowania znajdują się w bazie danych
        /// </summary>
        /// <param name="Stream">Strumień danych nawiązany z danym klientem</param>
        /// <returns></returns>
        protected bool logIn(NetworkStream Stream)
        {
            byte[] buffer = new byte[BufferSize];
            string login = null;
            string password = null;
            Account acc = null;
            while (acc == null)
            {
                try
                {
                    sendMsgToClient("Login: ", Stream);
                    login = readMsgFromClient(Stream);

                    sendMsgToClient("Password: ", Stream);
                    password = readMsgFromClient(Stream);

                    try
                    {
                        acc = DatabaseAccess.checkDB(login, password);

                    }
                    catch (InvalidOperationException)
                    {
                        sendMsgToClient("Invalid login or password!\n\r", Stream);
                    }
                }
                catch (Exception) { if(DatabaseAccess.ReaderWriterLock.IsReadLockHeld == true)DatabaseAccess.ReaderWriterLock.ExitReadLock(); }
            }
            sendMsgToClient("Logged in\n\r", Stream);
            Console.WriteLine($"{acc.Login} logged in");
            return true;
        }

        /// <summary>
        /// Metoda dodająca do bazy danych nowe dane logowania
        /// </summary>
        /// <param name="Stream">Strumień danych nawiązany z danym klientem</param>
        protected void addUser(NetworkStream Stream)
        {
            byte[] buffer = new byte[BufferSize];
            string login = null;
            string password = null;
            {
                sendMsgToClient("Login: ", Stream);
                login = readMsgFromClient(Stream);
                sendMsgToClient("Password: ", Stream);
                password = readMsgFromClient(Stream);
                DatabaseAccess.addUser(login, password);
            }
        }

        /// <summary>
        /// Metoda rozpoczynająca działanie serwera
        /// </summary>
        public override void Start()
        {
            IsRunning = true;
            startListening();
            acceptClient();
        }
    }
}
