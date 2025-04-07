using UnityEngine;
using UnityEngine.InputSystem;

namespace BrickBuilder
{
	public class BuildManager : MonoBehaviour
	{
		[SerializeField] private Camera _mainCamera;
		[SerializeField] private BrickStats _buildingPlane;
		[SerializeField] private float _gridCellSize;
		[SerializeField] private BrickStats _brickPrefab;
		
		private const string OpacityPropertyName = "_Opacity";
		private const string IgnoreLayerName = "Ignore Raycast";
		
		private BrickStats _brickPreview;
		private BoxCollider _brickPreviewCollider;
		private readonly Collider[] _previewHitColliders = new Collider[10];
		private Renderer _renderer;

		private void Start()
		{
			_brickPreview = Instantiate(_brickPrefab, _buildingPlane.transform);
			_brickPreview.transform.localPosition = new Vector3(_brickPreview.transform.localPosition.x, _buildingPlane.GetBrickHeight(), _brickPreview.transform.localPosition.z);
			_brickPreviewCollider = _brickPreview.GetComponent<BoxCollider>();
			_brickPreview.gameObject.layer = LayerMask.NameToLayer(IgnoreLayerName);
		}

		private void Update()
		{
			var mousePos = Mouse.current.position.ReadValue();
			var ray = _mainCamera.ScreenPointToRay(mousePos);
		
			if (Physics.Raycast(ray, out RaycastHit hitInfo))
			{
				hitInfo.collider.gameObject.TryGetComponent(out BrickStats hitBrick);
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
					var ren = builtBrick.GetComponent<Renderer>();
					ren.material.SetFloat(OpacityPropertyName, 1);
				}
				
				CheckForCollisions();
			}
		}

		private void CheckForCollisions()
		{
			var center = _brickPreview.transform.TransformPoint(_brickPreviewCollider.center);
			var halfExtents = _brickPreviewCollider.size * 0.45f;
			var orientation = _brickPreview.transform.localRotation;
			
			var numColliders = Physics.OverlapBoxNonAlloc(
				center, 
				halfExtents, 
				_previewHitColliders
			);
			
			
			for (var i = 0; i < numColliders; i++)
			{
				if (_previewHitColliders[i].gameObject != _brickPreview.gameObject)
				{
					Debug.Log($"Hit: {_previewHitColliders[i].name}");
				}
			}
			
			DrawDebugBox(center, halfExtents, orientation, Color.red);
		}
		
		void DrawDebugBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, Color color, float duration = 0)
		{
			// Get the corner points of the box
			Vector3[] points = new Vector3[8];
    
			// Bottom four corners
			points[0] = center + orientation * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
			points[1] = center + orientation * new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
			points[2] = center + orientation * new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
			points[3] = center + orientation * new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
    
			// Top four corners
			points[4] = center + orientation * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
			points[5] = center + orientation * new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
			points[6] = center + orientation * new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
			points[7] = center + orientation * new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);
    
			// Draw bottom face
			Debug.DrawLine(points[0], points[1], color, duration);
			Debug.DrawLine(points[1], points[2], color, duration);
			Debug.DrawLine(points[2], points[3], color, duration);
			Debug.DrawLine(points[3], points[0], color, duration);
    
			// Draw top face
			Debug.DrawLine(points[4], points[5], color, duration);
			Debug.DrawLine(points[5], points[6], color, duration);
			Debug.DrawLine(points[6], points[7], color, duration);
			Debug.DrawLine(points[7], points[4], color, duration);
    
			// Draw vertical edges connecting top and bottom faces
			Debug.DrawLine(points[0], points[4], color, duration);
			Debug.DrawLine(points[1], points[5], color, duration);
			Debug.DrawLine(points[2], points[6], color, duration);
			Debug.DrawLine(points[3], points[7], color, duration);
		}
	}
}
