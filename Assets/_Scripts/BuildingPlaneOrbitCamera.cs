using UnityEngine;
using UnityEngine.InputSystem;

namespace BrickBuilder
{
    public class BuildingPlaneOrbitCamera : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform _buildingPlane;
        [SerializeField] private Vector3 _orbitOffset = Vector3.zero; // Optional offset from building plane center

        [Header("Camera Settings")]
        [SerializeField] private float _initialDistance = 10.0f;
        [SerializeField] private float _minDistance = 5.0f;
        [SerializeField] private float _maxDistance = 20.0f;
        [SerializeField] private float _rotationSpeed = 200.0f;
        [SerializeField] private float _zoomSpeed = 15.0f;

        [Header("Rotation Limits")]
        [SerializeField] private float _minVerticalAngle = 10.0f;
        [SerializeField] private float _maxVerticalAngle = 80.0f;

        [Header("Input Settings")]
        [SerializeField] private bool _requireRightMouseButton = true;
        [SerializeField] private Key _alternateRotateKey = Key.LeftAlt;

        // Private variables
        private float _currentDistance;
        private float _horizontalAngle = 0.0f;
        private float _verticalAngle = 45.0f;
        private Vector3 _targetPosition;

        private void Start()
        {
            if (_buildingPlane == null)
            {
                Debug.LogError("BuildingPlaneOrbitCamera: Building plane reference is missing!");
                enabled = false;
                return;
            }

            // Initialize camera position
            _currentDistance = _initialDistance;
            _targetPosition = _buildingPlane.position + _orbitOffset;

            // Set initial rotation from current transform if needed
            var angles = transform.eulerAngles;
            _horizontalAngle = angles.y;
            _verticalAngle = angles.x;

            // Apply initial position
            UpdateCameraTransform();
        }

        private void LateUpdate()
        {
            if (_buildingPlane == null)
                return;

            // Update target position in case the building plane moves
            _targetPosition = _buildingPlane.position + _orbitOffset;

            // Handle rotation input
            if (CanRotate())
            {
                HandleRotation();
            }

            // Handle zoom input (always active)
            HandleZoom();

            // Apply the calculated transform
            UpdateCameraTransform();
        }

        private bool CanRotate()
        {
            return (_requireRightMouseButton && Mouse.current.rightButton.isPressed) || 
                   Keyboard.current[_alternateRotateKey].isPressed;
        }

        private void HandleRotation()
        {
            // Read mouse delta
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            // Apply rotation speed and delta time
            float deltaX = mouseDelta.x * _rotationSpeed * Time.deltaTime;
            float deltaY = mouseDelta.y * _rotationSpeed * Time.deltaTime;

            // Update angles
            _horizontalAngle += deltaX;
            _verticalAngle -= deltaY; // Invert Y for intuitive controls

            // Clamp vertical angle to prevent flipping
            _verticalAngle = Mathf.Clamp(_verticalAngle, _minVerticalAngle, _maxVerticalAngle);

            // Normalize horizontal angle
            if (_horizontalAngle > 360f)
                _horizontalAngle -= 360f;
            if (_horizontalAngle < 0f)
                _horizontalAngle += 360f;
        }

        private void HandleZoom()
        {
            // Get scroll wheel input
            float scrollValue = Mouse.current.scroll.y.ReadValue();
            
            // Apply zoom based on scroll input
            if (Mathf.Abs(scrollValue) > 0.01f)
            {
                _currentDistance -= scrollValue * _zoomSpeed * 0.01f;
                _currentDistance = Mathf.Clamp(_currentDistance, _minDistance, _maxDistance);
            }
        }

        private void UpdateCameraTransform()
        {
            // Calculate rotation
            Quaternion rotation = Quaternion.Euler(_verticalAngle, _horizontalAngle, 0);
            
            // Calculate position based on rotation and distance
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -_currentDistance);
            Vector3 position = rotation * negDistance + _targetPosition;
            
            // Apply position and rotation to camera
            transform.rotation = rotation;
            transform.position = position;
        }
    }
}