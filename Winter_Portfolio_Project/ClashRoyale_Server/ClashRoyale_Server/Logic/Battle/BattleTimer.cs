using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WPP.ClashRoyale_Server.Protocol;
using WPP.ClashRoyale_Server.Logic;
using WPP.ClashRoyale_Server.Logic.Room;

namespace WPP.ClashRoyale_Server.Logic.Battle
{
    internal class BattleTimer
    {
        private Timer _timer;
        private Timer _overTImer;

        public BattleTimer() { }
        public void InitializeTimers(BattleLogic battle)
        {
            _timer = new Timer(180000);
            _overTImer = new Timer(120000);

            _timer.Elapsed += (s, e) =>
            {
                _timer.Enabled = false;
                _overTImer.Enabled = true;
                battle.SetOverTimeMode();
            };


            _overTImer.Elapsed += (s, e) =>
            {
                _overTImer.Enabled = false;
                battle.EndBattle();
            };
        }

        public void StartTimer()
        {
            _timer.Enabled = true;
            _overTImer.Enabled = false;
        }

        public void StopTimer()
        {
            _timer.Enabled = false;
            _overTImer.Enabled = false;
        }
    }
}
