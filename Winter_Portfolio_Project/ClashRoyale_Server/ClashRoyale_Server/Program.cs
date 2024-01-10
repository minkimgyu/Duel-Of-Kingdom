using WPP.ClashRoyale_Server;
using System;
using System.Threading;
using WPP.ClashRoyale_Server.Protocol.Server;
using WPP.ClashRoyale_Server.Logic.Match;
using WPP.ClashRoyale_Server.Database;

namespace WPP.ClashRoyale_Server
{
    internal class Program
    {
        private static Thread consoleThread;

        static void Main(string[] args)
        {
            ServerTCP.Instance().InitializeServer();
            DatabaseManager.Instance().InitializeMySQLServer();
            PacketHandler.Instance().InitializePacketHandler();
            MatchMaker.Instance().InitializeMatchThread();

            InitializeConsoleThread();
        }

        private static void InitializeConsoleThread()
        {
            consoleThread = new Thread(ConsoleLoop);
            consoleThread.Name = "consoleThread";
            consoleThread.IsBackground = false;
            consoleThread.Start();
        }

        private static void ConsoleLoop()
        {
            Console.WriteLine("Server initalization completed");

            while (true)
            {
            }
        }
    }
}
