using UnityEngine;
using UnityEngine.InputSystem;

namespace BrickBuilder
{
	public class BuildingPlaneOrbitCamera : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private Camera _camera;
		[SerializeField] private LayerMask _buildingLayerMask;

		[Header("Camera Settings")]
		[SerializeField] private float _rotationSpeed = 200.0f;
		[SerializeField] private float _zoomSpeed = 15.0f;
		[SerializeField] private float _panSpeed = 10.0f;
		[SerializeField] private float _minDistance = 5.0f;
		[SerializeField] private float _maxDistance = 20.0f;
		[SerializeField] private float _minVerticalAngle = 10.0f;
		[SerializeField] private float _maxVerticalAngle = 80.0f;
		[SerializeField] private Key _panModifierKey = Key.LeftShift;

		// Private variables
		private float _horizontalAngle = 0.0f;
		private float _verticalAngle = 45.0f;

		private void Start()
		{
			if (_camera == null)
				_camera = Camera.main;

			// Set initial rotation
			var angles = transform.eulerAngles;
			_horizontalAngle = angles.y;
			_verticalAngle = angles.x;
		}

		private void LateUpdate()
		{
			// Handle panning with WASD when shift is held (can work simultaneously with rotation)
			if (Keyboard.current[_panModifierKey].isPressed)
			{
				HandlePanning();
			}
			
			// Handle rotation when right mouse button is pressed
			if (Mouse.current.rightButton.isPressed)
			{
				HandleRotation();
			}

			// Handle zoom input
			HandleZoom();
		}

		private void HandleRotation()
		{
			var rotationPoint = FindHitPointAtScreenCenter();
			
			// Calculate the distance to rotation point
			var directionToCamera = transform.position - rotationPoint;
			var distanceToPoint = directionToCamera.magnitude;
			
			var mouseDelta = Mouse.current.delta.ReadValue();
			_horizontalAngle += mouseDelta.x * _rotationSpeed * Time.deltaTime;
			_verticalAngle -= mouseDelta.y * _rotationSpeed * Time.deltaTime;

			// Clamp vertical angle
			_verticalAngle = Mathf.Clamp(_verticalAngle, _minVerticalAngle, _maxVerticalAngle);

			// Calculate and apply new camera position and rotation
			var newRotation = Quaternion.Euler(_verticalAngle, _horizontalAngle, 0);
			var newPosition = rotationPoint + newRotation * new Vector3(0, 0, -distanceToPoint);
			
			transform.rotation = newRotation;
			transform.position = newPosition;
		}

		private void HandleZoom()
		{
			var scrollValue = Mouse.current.scroll.y.ReadValue();
			
			if (Mathf.Abs(scrollValue) > 0.01f)
			{
				// Calculate zoom direction
				var zoomDirection = transform.forward;
				var zoomAmount = scrollValue * _zoomSpeed * 0.01f;
				
				// Move the camera along the zoom direction
				var newPosition = transform.position + zoomDirection * zoomAmount;
				
				// Check distance limits
				var ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
				var currentDistance = 10f;
				
				if (Physics.Raycast(ray, out RaycastHit hit, 100f, _buildingLayerMask))
				{
					currentDistance = Vector3.Distance(transform.position, hit.point);
				}
				
				var newDistance = currentDistance - zoomAmount;
				if (newDistance >= _minDistance && newDistance <= _maxDistance)
				{
					transform.position = newPosition;
				}
			}
		}

		private void HandlePanning()
		{
			var movement = Vector3.zero;

			if (Keyboard.current.wKey.isPressed)
				movement += transform.forward;
			if (Keyboard.current.sKey.isPressed)
				movement -= transform.forward;
			if (Keyboard.current.aKey.isPressed)
				movement -= transform.right;
			if (Keyboard.current.dKey.isPressed)
				movement += transform.right;

			if (movement.magnitude > 0.1f)
			{
				// Remove vertical component
				movement.y = 0;
				movement.Normalize();
				
				// Apply movement
				transform.position += movement * _panSpeed * Time.deltaTime;
			}
		}

		private Vector3 FindHitPointAtScreenCenter()
		{
			var ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			
			if (Physics.Raycast(ray, out RaycastHit hit, 100f, _buildingLayerMask))
			{
				return hit.point;
			}
			
			// If no hit, use a point at a fixed distance
			return ray.GetPoint(10f);
		}
	}
}