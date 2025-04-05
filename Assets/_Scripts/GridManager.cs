using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BrickBuilder
{
	public class GridManager : MonoBehaviour
	{
		[SerializeField] private Camera _mainCamera;
		[SerializeField] private Collider _gridCollider;
		[SerializeField] private float _gridCellSize;

		[SerializeField] private GameObject _ballPrefab;
		
		private Grid _grid;
		private GameObject _ball;

		private void Start()
		{
			var sizeX = _gridCollider.bounds.size.x;
			var sizeZ = _gridCollider.bounds.size.z;
			
			var sizeXInt = Mathf.RoundToInt(sizeX / _gridCellSize);
			var sizeZInt = Mathf.RoundToInt(sizeZ / _gridCellSize);
			
			_grid = new Grid(sizeXInt, sizeZInt, _gridCellSize);
			
			_ball = Instantiate(_ballPrefab, _gridCollider.transform);
		}

		private void Update()
		{
			var mousePos = Mouse.current.position.ReadValue();
			var ray = _mainCamera.ScreenPointToRay(mousePos);
		
			if (Physics.Raycast(ray, out RaycastHit hitInfo))
			{
				var localPoint = hitInfo.transform.InverseTransformPoint(hitInfo.point);

				var gridCellPosition = new Vector2Int(
					Mathf.RoundToInt(localPoint.x / _gridCellSize),
					Mathf.RoundToInt((localPoint.z * -1) / _gridCellSize));
				
				if (!_grid.IsInbounds(gridCellPosition))
				{
					return;
				}
				
				var visualGridPosition = _grid.GetVisualPosition(gridCellPosition);
				
				_ball.transform.localPosition = new Vector3(visualGridPosition.x, 0, visualGridPosition.y);

				// Debug.Log($"Grid position: X:{visualGridPosition.x}, Z:{visualGridPosition.y}");
			}
		}
	}
}
