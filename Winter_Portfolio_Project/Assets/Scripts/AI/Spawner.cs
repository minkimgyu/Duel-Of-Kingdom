using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.GRID;
using System;

namespace WPP.AI.SPAWNER
{
    public class Spawner : MonoBehaviour
    {
        List<Entity> entityPrefabs;
        Func<Vector3> OnSpawnPointRequested;

        private void Start()
        {
            GameObject grid = GameObject.FindWithTag("Grid");
            if (grid == null) return;

            GridSelecter selecter = grid.GetComponent<GridSelecter>();
            if (selecter == null) return;

            OnSpawnPointRequested = selecter.ReturnSpawnPoint;
        }

        //public void Spawn(int id, )
        //{
        //    Vector3 pos = OnSpawnPointRequested();


        //}
    }
}
