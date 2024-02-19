using System;

namespace WPP.ClientInfo.Card
{
    [Serializable]
    public class GridSize
    {
        public int top;
        public int down;
        public int left;
        public int right;

        public GridSize TroopGrid()
        {
            GridSize gridSize = new GridSize();
            gridSize.top = 0;
            gridSize.down = 0;
            gridSize.left = 0;
            gridSize.right = 0;
            return gridSize;
        }

        public GridSize BuildingGrid()
        {
            GridSize gridSize = new GridSize();
            gridSize.top = 1;
            gridSize.down = -1;
            gridSize.left = -1;
            gridSize.right = 1;
            return gridSize;
        }
    }
}