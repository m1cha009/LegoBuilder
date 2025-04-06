using UnityEngine;

namespace BrickBuilder
{
	public class Brick : MonoBehaviour
	{
		[SerializeField] private float _brickHeight;
		
		public float GetBrickHeight() => _brickHeight;
	}
}