using ClashRoyale_Server.Logic.GameRoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WPP.ClashRoyale_Server.Protocol;
using WPP.ClashRoyale_Server.Logic;

namespace WPP.ClashRoyale_Server.Logic.Battle
{
    internal class BattleTimer
    {
        private Timer _timer;
        private Timer _extraElixiertimer;
        private Timer _suddenDeathTimer;

        public BattleTimer() { }
        public void InitializeTimers(BattleLogic battle)
        {
            _timer = new Timer(120000);
            _extraElixiertimer = new Timer(60000);
            _suddenDeathTimer = new Timer(120000);

            _timer.Elapsed += (s, e) =>
            {
                _timer.Enabled = false;
                _extraElixiertimer.Enabled = true;
            };

            _extraElixiertimer.Elapsed += (s, e) =>
            {
                _extraElixiertimer.Enabled = false;
                _suddenDeathTimer.Enabled = true;
            };

            _suddenDeathTimer.Elapsed += (s, e) =>
            {
                _suddenDeathTimer.Enabled = false;
                battle.EndBattle();
            };
        }

        public void StartTimer()
        {
            _timer.Enabled = true;
            _extraElixiertimer.Enabled = false;
            _suddenDeathTimer.Enabled = false;
        }

        public void StopTimer()
        {
            _timer.Enabled = false;
            _extraElixiertimer.Enabled = false;
            _suddenDeathTimer.Enabled = false;
        }
    }
}
