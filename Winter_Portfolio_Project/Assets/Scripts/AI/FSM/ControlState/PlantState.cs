using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;

namespace WPP.AI.GRID.STATE
{
    public class PlantState : State
    {
        GridController _gridController;

        public PlantState(GridController gridController)
        {
            _gridController = gridController;
        }
    }
}
