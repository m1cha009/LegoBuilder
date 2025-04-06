using UnityEngine;
using UnityEngine.InputSystem;

namespace BrickBuilder
{
	public class BuildManager : MonoBehaviour
	{
		[SerializeField] private Camera _mainCamera;
		[SerializeField] private Brick _buildingPlane;
		[SerializeField] private float _gridCellSize;

		[SerializeField] private Brick _brickPrefab;
		
		private Brick _brickPreview;

		private void Start()
		{
			_brickPreview = Instantiate(_brickPrefab, _buildingPlane.transform);
			_brickPreview.transform.localPosition = new Vector3(_brickPreview.transform.localPosition.x, _buildingPlane.GetBrickHeight(), _brickPreview.transform.localPosition.z);
			_brickPreview.GetComponent<Collider>().enabled = false;
		}

		private void Update()
		{
			var mousePos = Mouse.current.position.ReadValue();
			var ray = _mainCamera.ScreenPointToRay(mousePos);
		
			if (Physics.Raycast(ray, out RaycastHit hitInfo))
			{
				hitInfo.collider.gameObject.TryGetComponent(out Brick hitBrick);
				if (hitBrick == null)
				{
					return;
				}
				
				if (Mathf.Abs(hitInfo.normal.y) < 0.9f) // only top/bottom is checked
				{
					return;
				}
				
				var x = Mathf.RoundToInt(hitInfo.point.x / _gridCellSize) * _gridCellSize;
				var z = Mathf.RoundToInt(hitInfo.point.z / _gridCellSize) * _gridCellSize;

				_brickPreview.transform.localPosition = new Vector3(x, hitInfo.transform.position.y + hitBrick.GetBrickHeight(), z);
				
				if (Keyboard.current.rKey.wasPressedThisFrame)
				{
					_brickPreview.transform.Rotate(0, -90f, 0);
				}
				else if (Mouse.current.leftButton.wasPressedThisFrame)
				{
					var builtBrick = Instantiate(_brickPrefab, _buildingPlane.transform);
					builtBrick.transform.localPosition = _brickPreview.transform.localPosition;
					builtBrick.transform.rotation = _brickPreview.transform.rotation;
				}
			}


		}
	}
}
