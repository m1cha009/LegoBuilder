using UnityEngine;
using UnityEngine.Serialization;

namespace BrickBuilder
{
	public class BrickStats : MonoBehaviour
	{
		[FormerlySerializedAs("brickHeight")]
		[Tooltip("Plates = .0032 | Regular = .0096 | 2xRegular = .0192 | 3xRegular = .0288 | 4xRegular = .0384")]
		public float _brickHeight = 0.0032f;

		private GameObject _visualObject;
		
		public BoxCollider BrickCollider { get; private set; }
		public Renderer BrickRenderer { get; private set; }
		public float GetBrickHeight() => _brickHeight;
		public GameObject VisualObject => _visualObject;

		private void Awake()
		{
			BrickCollider = GetComponentInChildren<BoxCollider>();
			BrickRenderer = GetComponentInChildren<Renderer>();
			_visualObject = transform.GetChild(0).gameObject;
		}

		public void SetLayer(int layerMask)
		{
			_visualObject.layer = layerMask;
		}
	}
}