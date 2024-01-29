using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using WPP.AI.STAT;
using WPP.AI.CAPTURE;

namespace WPP.JSON
{
    public class JsonParser : MonoBehaviour
    {
        // JsonData 폴더에 있는 파일을 불러온다.
        // 불러온 데이터를 BaseStat 형태로 저장하고 반환한다.

        // 나중에 경로는 Application.persistentDataPath로 바꿔주기
        string path = "/JsonData/MyData.txt";

        private void Start()
        {
            Save();
        }

        List<BaseStat> ReturnBasicData()
        {
            List<BaseStat> stats = new List<BaseStat>();

            stats.Add(new UnitStat(0, 1, "barbarian", 670, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit }, 192, 1.4f, 0.7f));
            stats.Add(new UnitStat(1, 1, "Shooter", 720, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit, CaptureTag.AirUnit }, 218, 1, 6));
            stats.Add(new UnitStat(2, 1, "Giant", 4091, new CaptureTag[] { CaptureTag.Building }, 254, 1.5f, 1.2f));
            stats.Add(new UnitStat(3, 1, "Wizard", 720, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit }, 281, 1.4f, 5.5f));
            stats.Add(new UnitStat(4, 1, "Dragon", 1152, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit }, 160, 1.5f, 3.5f));

            stats.Add(new UnitStat(5, 1, "Knight", 1766, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit }, 202, 1.2f, 1.2f));
            stats.Add(new UnitStat(6, 1, "Bee", 81, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit, CaptureTag.AirUnit }, 81, 1.3f, 1.2f));
            stats.Add(new UnitStat(7, 1, "Minion", 837, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit, CaptureTag.AirUnit }, 311, 1.5f, 1.6f));

            stats.Add(new LivingOutAttackBuildingStat(8, 1, "CannonTower", 896, new CaptureTag[] { CaptureTag.GroundUnit }, 212, 0.9f, 5.5f, 30));
            stats.Add(new LivingOutSpawnBuildingStat(9, 1, "BarbarianHut", 1166, 30, 0, 3));

            stats.Add(new AttackBuildingStat(10, 1, "PrincessTower", 3052, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit, CaptureTag.AirUnit }, 109, 0.8f, 10f));
            stats.Add(new AttackBuildingStat(11, 1, "KingTower", 5052, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit, CaptureTag.AirUnit }, 218, 0.8f, 10f));

            return stats;
        }

        // Json 파일을 읽어와서 그걸 바탕으로 오브젝트를 스폰시킨다.
        void Load()
        {
            string jdata = JsonConvert.SerializeObject(ReturnBasicData());
            File.WriteAllText(Application.dataPath + path, jdata);
        }

        void Save()
        {
            Debug.Log(Application.dataPath + path);

            string jdata = JsonConvert.SerializeObject(ReturnBasicData());
            File.WriteAllText(Application.dataPath + path, jdata);
        }
    }
}