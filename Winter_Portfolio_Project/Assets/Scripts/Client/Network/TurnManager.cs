using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using WPP.Network;
using WPP.ClientInfo;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using WPP.Protocol;
using WPP.RoomInfo;
using UnityEngine.Rendering;
using System.Windows.Input;

namespace WPP
{
    public class TurnManager : MonoBehaviour
    {
        private static TurnManager _instance = null;
        public static TurnManager Instance { get { return _instance; } }
        private List<byte[]> _myInputCommands;
        private List<byte[]> _opponentInputCommands;

        private List<byte[]> _opponentCommandsToExecute;
        private List<byte[]> _commandsToExecute;
        private object _commandLock;
        private int _turn;

        private void Awake()
        {
            Application.targetFrameRate = 30;
            _instance = this;
            _myInputCommands = new List<byte[]>();
            _opponentInputCommands = new List<byte[]>();
            _opponentCommandsToExecute = new List<byte[]>();
            _commandsToExecute = new List<byte[]>();
            _commandLock = new object();
            _turn = 0;
        }

        public void AddCommand(Peer_PacketTagPackages tag, byte[] command)
        {
            lock (_commandLock)
            {
                ByteBuffer buffer = ClientTCP.Instance().CreateBufferToSend(tag, command);
                _myInputCommands.Add(buffer.ToArray());
                Debug.Log("add commands");
            }
        }

        public void AddOpponentCommand(Peer_PacketTagPackages tag, byte[] command)
        {
            lock (_commandLock)
            {
                ByteBuffer buffer = ClientTCP.Instance().CreateBufferToSend(tag, command);
                _opponentInputCommands.Add(buffer.ToArray());
                Debug.Log("add opponent commands");
            }
        }

        public void SendCommandsToOpponent()
        {
            lock (_commandLock)
            {
                _commandsToExecute = new List<byte[]>(_myInputCommands);
                _myInputCommands.Clear();

                _opponentCommandsToExecute = new List<byte[]>(_opponentInputCommands);
                _opponentInputCommands.Clear();

                if (_commandsToExecute.Count > 0)
                {
                    ByteBuffer commandBuffer = new ByteBuffer();
                    commandBuffer.WriteInteger(_commandsToExecute.Count);

                    foreach (byte[] command in _commandsToExecute)
                    {
                        commandBuffer.WriteBytes(command);
                    }
                    // 내 List에 있는 command들을 상대방에게 보내기
                    ClientTCP.Instance().SendDataToPeer(Peer_PacketTagPackages.P_SEND_COMMANDS, commandBuffer.ToArray());
                    Debug.Log("send commands");
                }
                else
                {
                    // 비어 있는 커맨드 송신
                    ClientTCP.Instance().SendDataToPeer(Peer_PacketTagPackages.P_SEND_COMMANDS, null);
                }
            }
        }

        public void ExecuteCommands()
        {
            lock (_commandLock)
            {
                _commandsToExecute.AddRange(_opponentCommandsToExecute);
                foreach (byte[] command in _commandsToExecute)
                {
                    lock (ClientTCP.Instance().PacketQueueLockObject)
                    {
                        PacketHandler.Instance().packetQueue.Enqueue(command);
                        Debug.Log("enqueue commands");
                    }
                }

                _commandsToExecute.Clear();
                _opponentCommandsToExecute.Clear();
            }
        }

        private void Update()
        {
            // 10 프레임 때 내 보유 커맨드들을 상대방에게 전송
            if(_turn == 10)
            {
                SendCommandsToOpponent();
            }
            // 20 프레임 때 보유 커맨드(자신의 커맨드와 상대방의 커맨드 모두를 지칭)들을 실행
            // 10 프레임 간의 차이를 두는 이유는 전송과 실행 시간에 대한 여유를 주기 위함
            else if (_turn == 20 && _commandsToExecute.Count > 0)
            {
                ExecuteCommands();
            }
            else if (_turn == 30)
            {
                _turn = 0;
                _commandsToExecute.Clear();
                _opponentCommandsToExecute.Clear();
            }
            ++_turn;
        }
    }
}

