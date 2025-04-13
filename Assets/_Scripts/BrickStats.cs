using UnityEngine;
using UnityEngine.Serialization;

namespace BrickBuilder
{
	public class BrickStats : MonoBehaviour
	{
		[FormerlySerializedAs("brickHeight")]
		[Tooltip("Plates = .0032 | Regular = .0096 | 2xRegular = .0192 | 3xRegular = .0288 | 4xRegular = .0384")]
		public float _brickHeight = 0.0032f;
		
		private const string ColorPropertyName = "_MyColor";
		private const string OpacityPropertyName = "_Opacity";
		
		private Renderer _brickRenderer;
		private bool _initialized;

		public BoxCollider BrickCollider { get; private set; }
		public Color BrickColor { get; private set; }
		public float GetBrickHeight() => _brickHeight;

		private void Awake()
		{
			BrickCollider = GetComponent<BoxCollider>();
			_brickRenderer = GetComponent<Renderer>();

			_initialized = true;
		}

		public void SetColor(Color color, bool isDefault, float opacity = 0)
		{
			if (_initialized)
			{
				if (isDefault)
				{
					_brickRenderer.material.SetColor(ColorPropertyName, color);
				}
				else
				{
					_brickRenderer.material.SetColor(ColorPropertyName, color);
					_brickRenderer.material.SetFloat(OpacityPropertyName, opacity);
				}
			}

			BrickColor = color;
		}
	}
}