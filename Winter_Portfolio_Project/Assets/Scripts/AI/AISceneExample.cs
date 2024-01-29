using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.GRID;
using WPP.DRAWING;

namespace WPP.AI
{
    public class AISceneExample : MonoBehaviour
    {
        [SerializeField] GridFiller _gridFiller;
        [SerializeField] SpawnAreaDrawer _spawnRect;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                _gridFiller.OnLandFormationAssigned(LandFormation.C);
                _spawnRect.Erase();

                _spawnRect.Draw();
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                _gridFiller.OnTowerConditionChanged(LandFormation.C, TowerCondition.LeftDestroy);
                _spawnRect.Erase();

                _spawnRect.Draw();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _gridFiller.OnTowerConditionChanged(LandFormation.C, TowerCondition.RightDestroy);
                _spawnRect.Erase();

                _spawnRect.Draw();
            }


            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                _gridFiller.OnLandFormationAssigned(LandFormation.R);
                _spawnRect.Erase();

                _spawnRect.Draw();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                _gridFiller.OnTowerConditionChanged(LandFormation.R, TowerCondition.LeftDestroy);
                _spawnRect.Erase();

                _spawnRect.Draw();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                _gridFiller.OnTowerConditionChanged(LandFormation.R, TowerCondition.RightDestroy);
                _spawnRect.Erase();

                _spawnRect.Draw();
            }
        }
    }
}
