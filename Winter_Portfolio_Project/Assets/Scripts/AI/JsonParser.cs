using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using WPP.AI.STAT;
using WPP.AI.CAPTURE;
using WPP.GRID;

namespace WPP.JSON
{
    public class JsonParser : MonoBehaviour
    {
        // JsonData 폴더에 있는 파일을 불러온다.
        // 불러온 데이터를 BaseStat 형태로 저장하고 반환한다.

        // 나중에 경로는 Application.persistentDataPath로 바꿔주기
        string path = "/JsonData/MyData.txt";

        //private void Start()
        //{
        //    Save();
        //}

        List<BaseStat> ReturnBasicData()
        {
            List<BaseStat> stats = new List<BaseStat>();

            stats.Add(new UnitStat(0, 1, "barbarian", 670, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit }, 192, 1.4f, 0.7f, 10f));
            stats.Add(new UnitStat(1, 1, "shooter", 720, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit, CaptureTag.AirUnit }, 218, 1, 6, 10f));
            stats.Add(new UnitStat(2, 1, "giant", 4091, new CaptureTag[] { CaptureTag.Building }, 254, 1.5f, 1.2f, 10f));
            stats.Add(new UnitStat(3, 1, "wizard", 720, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit }, 281, 1.4f, 5.5f, 10f));
            stats.Add(new UnitStat(4, 1, "dragon", 1152, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit }, 160, 1.5f, 3.5f, 10f));

            stats.Add(new UnitStat(5, 1, "knight", 1766, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit }, 202, 1.2f, 1.2f, 10f));
            stats.Add(new UnitStat(6, 1, "bat", 81, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit, CaptureTag.AirUnit }, 81, 1.3f, 1.6f, 10f));
            stats.Add(new UnitStat(7, 1, "mega_minion", 837, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit, CaptureTag.AirUnit }, 311, 1.5f, 1.6f, 10f));

            OffsetFromCenter normalOffset = new OffsetFromCenter(1, 1, 1, 1);
            OffsetFromCenter kingTowerOffset = new OffsetFromCenter(1, 2, 2, 1);

            stats.Add(new LivingOutAttackBuildingStat(8, 1, "cannon", 896, normalOffset, new CaptureTag[] { CaptureTag.GroundUnit }, 212, 0.9f, 15f, 20f, 30));

            SerializableVector3[] spawnOffsets = new SerializableVector3[] { new SerializableVector3(0, 0, 0), new SerializableVector3(1, 0, 0) };
            stats.Add(new LivingOutSpawnBuildingStat(9, 1, "babarian_hut", 1166, normalOffset, 30, 0, 3, spawnOffsets));

            stats.Add(new AttackBuildingStat(10, 1, "princess_tower", 3052, normalOffset, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit, CaptureTag.AirUnit }, 109, 0.8f, 15f, 20f));
            stats.Add(new AttackBuildingStat(11, 1, "king_tower", 5052, kingTowerOffset, new CaptureTag[] { CaptureTag.Building, CaptureTag.GroundUnit, CaptureTag.AirUnit }, 218, 0.8f, 20f, 25f));

            return stats;
        }

        // Json 파일을 읽어와서 그걸 바탕으로 오브젝트를 스폰시킨다.
        public List<BaseStat> Load()
        {
            string jdata = File.ReadAllText(Application.dataPath + path);
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

            List<BaseStat> stats = JsonConvert.DeserializeObject<List<BaseStat>>(jdata, settings);

            return stats;
        }

        void Save()
        {
            Debug.Log(Application.dataPath + path);

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            string jdata = JsonConvert.SerializeObject(ReturnBasicData(), settings);
            File.WriteAllText(Application.dataPath + path, jdata);
        }
    }
}