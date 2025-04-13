using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

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
		[SerializeField] private AllBricksList _allBricksList;

		private const string IgnoreLayerName = "Ignore Raycast";
		
		private BrickStats _brickPreview;
		private readonly Collider[] _previewHitColliders = new Collider[8];
		private Renderer _renderer;

		private PreviewState _currentPreviewState = PreviewState.None;
		private PreviewState _previousPreviewState = PreviewState.None;

		private readonly Dictionary<string, BrickStats> _bricksPrefabDict = new();
		private readonly List<BrickStats> _builtBricksList = new();

		private void Start()
		{
			SetupBrickPrefabs();
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
					
					_brickPreview.SetColor(_previewDeniedColor, true);
				}
				else
				{
					if (_currentPreviewState != _previousPreviewState)
					{
						_brickPreview.SetColor(_previewAllowedColor, true);
					}
				}

				_previousPreviewState = _currentPreviewState;

				if (Mouse.current.leftButton.wasPressedThisFrame)
				{
					var brickName = _brickPreview.name.Replace("(Clone)", "");
					
					CreateBrick(brickName, _brickPreview.transform.localPosition, _brickPreview.transform.rotation);
				}
			}
		}

		public List<BrickStats> GetBuiltStructure()
		{
			return _builtBricksList;
		}

		public void ClearStructure()
		{
			var bricks = GetBuiltStructure();

			foreach (var brick in bricks)
			{
				Destroy(brick.gameObject);
			}
			
			_builtBricksList.Clear();
		}

		public void CreateBrick(string prefabName, Vector3 position, Quaternion rotation)
		{
			if (!_bricksPrefabDict.TryGetValue(prefabName, out var prefab))
			{
				Debug.LogError($"Prefab Instance ID: {prefabName} not found!");
				return;
			}
			
			var brickInstance = Instantiate(prefab, _buildingPlane.transform);
			brickInstance.transform.localPosition = position;
			brickInstance.transform.rotation = rotation;
			brickInstance.gameObject.layer = 0;
			brickInstance.SetColor(prefab.BrickColor, false, 1f);
			
			_builtBricksList.Add(brickInstance);
		}

		private void SetupBrickPrefabs()
		{
			foreach (var brickPrefab in _allBricksList.AllBricks)
			{
				var color = GetNewRandomColor();
				brickPrefab.SetColor(color, true);
				
				_bricksPrefabDict.Add(brickPrefab.name, brickPrefab);
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
			
			// DrawDebugBox(center, halfExtents, orientation, Color.red);
			
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
				ConfigureBrickPreview(_bricksPrefabDict.Values.ToList()[0]);
			}
			else if (Keyboard.current.digit2Key.wasPressedThisFrame)
			{
				ConfigureBrickPreview(_bricksPrefabDict.Values.ToList()[1]);
			}
			else if (Keyboard.current.digit3Key.wasPressedThisFrame)
			{
				ConfigureBrickPreview(_bricksPrefabDict.Values.ToList()[2]);
			}
			else if (Keyboard.current.digit4Key.wasPressedThisFrame)
			{
				ConfigureBrickPreview(_bricksPrefabDict.Values.ToList()[3]);
			}
			else if (Keyboard.current.digit5Key.wasPressedThisFrame)
			{
				ConfigureBrickPreview(_bricksPrefabDict.Values.ToList()[4]);
			}

		}

		private void ConfigureBrickPreview(BrickStats newBrick)
		{
			var position = Vector3.zero;
			var rotation = Quaternion.identity;
			
			if (_brickPreview != null)
			{
				position = _brickPreview.transform.localPosition;
				rotation = _brickPreview.transform.rotation;
				
				Destroy(_brickPreview.gameObject);
				_brickPreview = null;
			}
			
			_brickPreview = Instantiate(newBrick, _buildingPlane.transform);
			_brickPreview.gameObject.layer = LayerMask.NameToLayer(IgnoreLayerName);
			_brickPreview.SetColor(_previewDeniedColor, true);
			_brickPreview.transform.localPosition = position;
			_brickPreview.transform.rotation = rotation;
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
