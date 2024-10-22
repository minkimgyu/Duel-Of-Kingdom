using UnityEngine;
using WPP.ClientInfo;
using WPP.Network;
using WPP.RoomInfo;

namespace WPP.Battle
{
    public class BattlePlayer : MonoBehaviour
    {
        [SerializeField] private BattlePlayer _enemy;
        [Space]
        [SerializeField] private CrownSystem _crownSystem;
        [SerializeField] private TowerSystem _towerSystem;

        public void Init(bool isClient)
        {
            _enemy._towerSystem.OnKingTowerDestroyed += _crownSystem.AddCrown;
            _enemy._towerSystem.OnLeftPrincessTowerDestroyed += _crownSystem.AddCrown;
            _enemy._towerSystem.OnRightPrincessTowerDestroyed += _crownSystem.AddCrown;

            if (!isClient)
            {
                _enemy._towerSystem.OnKingTowerDestroyed += OnOpponentKingTowerDestroyed;
                _enemy._towerSystem.OnLeftPrincessTowerDestroyed += OnOpponentLeftTowerDestroyed;
                _enemy._towerSystem.OnRightPrincessTowerDestroyed += OnOpponentRightTowerDestroyed;
            }
        }

        private void OnOpponentRightTowerDestroyed()
        {
            ByteBuffer buffer = new ByteBuffer();
            int roomID = GameRoom.Instance().roomID;
            buffer.WriteInteger(roomID);
            ClientTCP.Instance().SendDataToServer(Client_PacketTagPackages.C_DESTROY_OPPONENT_RIGHT_TOWER, buffer.ToArray());
        }

        private void OnOpponentLeftTowerDestroyed()
        {
            ByteBuffer buffer = new ByteBuffer();
            int roomID = GameRoom.Instance().roomID;
            buffer.WriteInteger(roomID);
            ClientTCP.Instance().SendDataToServer(Client_PacketTagPackages.C_DESTROY_OPPONENT_LEFT_TOWER, buffer.ToArray());
        }

        private void OnOpponentKingTowerDestroyed()
        {
            ByteBuffer buffer = new ByteBuffer();
            int roomID = GameRoom.Instance().roomID;
            buffer.WriteInteger(roomID);
            ClientTCP.Instance().SendDataToServer(Client_PacketTagPackages.C_DESTROY_OPPONENT_KING_TOWER, buffer.ToArray());
        }

        public CrownSystem CrownSystem => _crownSystem;
        public TowerSystem TowerSystem => _towerSystem;
    }
}

