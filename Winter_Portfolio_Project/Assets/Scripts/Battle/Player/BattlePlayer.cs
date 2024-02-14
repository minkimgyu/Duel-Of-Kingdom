using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace WPP.Battle
{
    public class BattlePlayer : MonoBehaviour
    {
        [SerializeField] private BattlePlayer _enemy;
        [Space]
        [SerializeField] private CrownSystem _crownSystem;
        [SerializeField] private TowerSystem _towerSystem;

        public void Init()
        {
            _enemy._towerSystem.OnKingTowerDestroyed += _crownSystem.AddCrown;
            _enemy._towerSystem.OnLeftPrincessTowerDestroyed += _crownSystem.AddCrown;
            _enemy._towerSystem.OnRightPrincessTowerDestroyed += _crownSystem.AddCrown;
        }

        public CrownSystem CrownSystem => _crownSystem;
        public TowerSystem TowerSystem => _towerSystem;
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(BattlePlayer))]
    public class BattlePlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(!Application.isPlaying) return;

            var battlePlayer = (BattlePlayer)target;

            if (GUILayout.Button("Add Crown"))
            {
                battlePlayer.CrownSystem.AddCrown();
            }

            if (GUILayout.Button("Destroy King Tower"))
            {
                battlePlayer.TowerSystem.DestroyKingTower();
            }

            if (GUILayout.Button("Destroy Left Princess Tower"))
            {
                battlePlayer.TowerSystem.DestroyLeftPrincessTower();
            }

            if (GUILayout.Button("Destroy Right Princess Tower"))
            {
                battlePlayer.TowerSystem.DestroyRightPrincessTower();
            }
        }
    }
#endif
}

