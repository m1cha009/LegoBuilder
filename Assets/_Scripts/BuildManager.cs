using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BrickBuilder
{
	public class BuildManager : MonoBehaviour
	{
		private enum PreviewState
		{
			None = 0,
			Allowed = 1,
			Denied = 2,
		}
		
		[SerializeField] private Camera _mainCamera;
		[SerializeField] private BrickStats _buildingPlane;
		[SerializeField] private float _gridCellSize;
		[SerializeField] private Color _previewAllowedColor;
		[SerializeField] private Color _previewDeniedColor;
		[SerializeField] private List<BrickStats> _brickPrefabs;

		
		private const string OpacityPropertyName = "_Opacity";
		private const string ColorPropertyName = "_MyColor";
		private const string IgnoreLayerName = "Ignore Raycast";
		
		private BrickStats _brickPreview;
		private readonly Collider[] _previewHitColliders = new Collider[8];
		private Renderer _renderer;

		private PreviewState _currentPreviewState = PreviewState.None;
		private PreviewState _previousPreviewState = PreviewState.None;


		private readonly List<BrickStats> _bricks = new();

		private void Start()
		{
			foreach (var brickPrefab in _brickPrefabs)
			{
				var brick = Instantiate(brickPrefab, transform);
				brick.gameObject.layer = LayerMask.NameToLayer(IgnoreLayerName);
				brick.BrickRenderer.material.SetColor(ColorPropertyName, _previewAllowedColor);
				brick.gameObject.SetActive(false);
				_bricks.Add(brick);
			}
		}

		private void Update()
		{
			SelectBrick();

			if (_brickPreview == null)
			{
				return;
			}
			
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

				if (hitInfo.normal.y > 0)
				{
					_brickPreview.transform.localPosition = new Vector3(x, hitInfo.transform.position.y + _brickPreview.GetBrickHeight(), z);
				}
				else
				{
					_brickPreview.transform.localPosition = new Vector3(x, hitInfo.transform.position.y - _brickPreview.GetBrickHeight(), z);
				}
				
				if (Keyboard.current.rKey.wasPressedThisFrame)
				{
					_brickPreview.transform.Rotate(0, -90f, 0);
				}

				var hasCollisions = HasCollisions();
				_currentPreviewState = hasCollisions ? PreviewState.Denied : PreviewState.Allowed;
				
				if (hasCollisions)
				{
					if (_currentPreviewState == _previousPreviewState)
					{
						return;
					}
					
					_brickPreview.BrickRenderer.material.SetColor(ColorPropertyName, _previewDeniedColor);
				}
				else
				{
					if (_currentPreviewState != _previousPreviewState)
					{
						_brickPreview.BrickRenderer.material.SetColor(ColorPropertyName, _previewAllowedColor);
					}
				}

				_previousPreviewState = _currentPreviewState;

				if (Mouse.current.leftButton.wasPressedThisFrame)
				{
					var builtBrick = Instantiate(_brickPreview, _buildingPlane.transform);
					builtBrick.transform.localPosition = _brickPreview.transform.localPosition;
					builtBrick.transform.rotation = _brickPreview.transform.rotation;
					builtBrick.gameObject.layer = 0;
					builtBrick.BrickRenderer.material.SetColor(ColorPropertyName, GetNewRandomColor());
					builtBrick.BrickRenderer.material.SetFloat(OpacityPropertyName, 1);
				}
			}
		}

		private bool HasCollisions()
		{
			var center = _brickPreview.transform.TransformPoint(_brickPreview.BrickCollider.center);
			var halfExtents = _brickPreview.BrickCollider.size * 0.45f;
			var orientation = _brickPreview.transform.rotation;
			
			var numColliders = Physics.OverlapBoxNonAlloc(
				center, 
				halfExtents, 
				_previewHitColliders,
				orientation
			);
			
			DrawDebugBox(center, halfExtents, orientation, Color.red);
			
			for (var i = 0; i < numColliders; i++)
			{
				if (_previewHitColliders[i].gameObject != _brickPreview.gameObject)
				{
					// Debug.Log($"Hit: {_previewHitColliders[i].name}");
					return true;
				}
			}
			
			return false;
		}

		private void SelectBrick()
		{
			if (Keyboard.current.digit1Key.wasPressedThisFrame)
			{
				ConfigureBrickPreview(_bricks[0]);
			}
			else if (Keyboard.current.digit2Key.wasPressedThisFrame)
			{
				ConfigureBrickPreview(_bricks[1]);
			}
			else if (Keyboard.current.digit3Key.wasPressedThisFrame)
			{
				ConfigureBrickPreview(_bricks[2]);
			}
			else if (Keyboard.current.digit4Key.wasPressedThisFrame)
			{
				ConfigureBrickPreview(_bricks[3]);
			}
			else if (Keyboard.current.digit5Key.wasPressedThisFrame)
			{
				ConfigureBrickPreview(_bricks[4]);
			}

		}

		private void ConfigureBrickPreview(BrickStats newBrick)
		{
			if (_brickPreview != null)
			{
				_brickPreview.transform.parent = transform;
				_brickPreview.gameObject.SetActive(false);
			}
			
			_brickPreview = newBrick;
			_brickPreview.transform.parent = _buildingPlane.transform;
			_brickPreview.transform.localPosition = new Vector3(_brickPreview.transform.localPosition.x, _brickPreview.GetBrickHeight(), _brickPreview.transform.localPosition.z);
			
			_brickPreview.gameObject.SetActive(true);
		}

		private Color GetNewRandomColor()
		{
			return new Color
			{
				r = Random.Range(0.0f, 1.0f),
				g = Random.Range(0.0f, 1.0f),
				b = Random.Range(0.0f, 1.0f),
				a = 1.0f
			};
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
