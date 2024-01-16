using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WPP.ClashRoyale_Server.Protocol.Server;
using WPP.ClashRoyale_Server.Protocol;
using WPP.ClashRoyale_Server.Logic.Room;
using System.Reflection;

namespace WPP.ClashRoyale_Server.Logic.Match
{
    class MatchMaker
    {
        private static MatchMaker _instance;
        private Thread _matchThread;

        public static MatchMaker Instance()
        {
            if (_instance == null)
            {
                _instance = new MatchMaker();
            }
            return _instance;
        }

        public void InitializeMatchThread()
        {
            _matchThread = new Thread(MatchLoop);
            _matchThread.Name = "matchThread";
            _matchThread.IsBackground = false;
            _matchThread.Start();
        }
       
        public void MatchLoop()
        {
            int clientIndextToMatch = 0;
            while (true)
            {
                if(clientIndextToMatch > ServerTCP.Instance().waitingClients.Count)
                {
                    Console.WriteLine("there are no clinets that can be match");
                    continue;
                }

                if (ServerTCP.Instance().waitingClients.Count > 1)
                {
                    // reference sites
                    // https://www.reddit.com/r/ClashRoyale/comments/rvghl1/explaining_the_matchmaking/
                    // https://supercell.com/en/games/clashroyale/blog/news/matchmaking-changes/

                    ClientObject clnt1 = ServerTCP.Instance().waitingClients[clientIndextToMatch];
                    int opponentIndex = FindOptimalOpponentIndex(clnt1, clientIndextToMatch);
                    if (opponentIndex == -1)
                    {
                        // 적절한 상대를 찾지 못하였을 경우
                        ++clientIndextToMatch;
                        continue;
                    }

                    ClientObject clnt2 = ServerTCP.Instance().waitingClients[opponentIndex];
                    Match(clnt1, clientIndextToMatch, clnt2, opponentIndex);
                }
            }
        } 

        public void Match(ClientObject clnt1, int clnt1Index, ClientObject clnt2, int clnt2Index)
        {
            ServerTCP.Instance().waitingClients.RemoveAt(clnt1Index);
            clnt1.state = ClientState.IN_GAME;

            ServerTCP.Instance().waitingClients.RemoveAt(clnt2Index - 1);
            clnt2.state = ClientState.IN_GAME;

            GameRoomManager.Instance().EnterGameRoom(clnt1, clnt2);

            Console.WriteLine("clientID: {0}, trophy: {1} and clientID: {2}, trophy: {3} has matched", clnt1.tcp.id, clnt1.accountInfo.trophy, clnt2.tcp.id, clnt2.accountInfo.trophy);
        }

        public int FindOptimalOpponentIndex(ClientObject client, int clientIndextToMatch)
        {
            int trophy = client.accountInfo.trophy;
            int kingTowerLevel = client.towers.kingTower.level;

            int minTrophy = trophy - 100;
            int maxTrophy = trophy + 100;

            int minKingTowerLevel = kingTowerLevel - 1;
            int maxKingTowerLevel = kingTowerLevel + 1;

            int index = -1;

            while(true)
            {
                ++index;

                // 조건에 맞는 client를 찾지 못했을 경우
                if (index > ServerTCP.Instance().waitingClients.Count - 1)
                {
                    if (minKingTowerLevel <= kingTowerLevel - 3 &&
                       maxKingTowerLevel >= kingTowerLevel + 3)
                    {
                        Console.WriteLine("Can't find optimal opponent");
                        return -1;
                    }

                    // 조건 변경
                    --minKingTowerLevel;
                    --maxKingTowerLevel;
                    index = 0;
                }

                ClientObject clientObj = ServerTCP.Instance().waitingClients[index];

                if (index == clientIndextToMatch)
                    continue;

                if (clientObj.accountInfo.trophy < minTrophy)
                    continue;

                if (clientObj.accountInfo.trophy > maxTrophy)
                    continue;

                if (clientObj.towers.kingTower.level < minKingTowerLevel)
                    continue;

                if (clientObj.towers.kingTower.level > maxKingTowerLevel)
                    continue;

                // 모든 조건 성립
                return index;
            }
        }
    }
}
