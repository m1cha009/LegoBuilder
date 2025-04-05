using UnityEngine;

namespace BrickBuilder
{
	public class Grid
	{
		private int[,] _grid;
		private float _cellSize;

		public Grid(int sizeX, int sizeY, float cellSize)
		{
			_grid = new int[sizeX, sizeY];
			_cellSize = cellSize;
			
			for (var i = 0; i < sizeX; i++)
			{
				for (var j = 0; j < sizeY; j++)
				{
					_grid[i, j] = 0;
				}
			}
		}

		public int GetSizeX() => _grid.GetLength(0);
		public int GetSizeY() => _grid.GetLength(1);

		public Vector2 GetVisualPosition(Vector2Int gridPosition)
		{
			return new Vector2(gridPosition.x * _cellSize, gridPosition.y * _cellSize * -1);
		}

		public bool IsInbounds(Vector2Int gridPosition)
		{
			return gridPosition.x >= 0 && gridPosition.x < GetSizeX() && gridPosition.y >= 0 && gridPosition.y < GetSizeY();
		}
	}
}