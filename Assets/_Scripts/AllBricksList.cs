using System.Collections.Generic;
using UnityEngine;

namespace BrickBuilder
{
	[CreateAssetMenu(fileName = "AllBricksList", menuName = "SO/AllBricksList", order = 0)]
	public class AllBricksList : ScriptableObject
	{
		[SerializeField] private List<BrickStats> _allBricks;

		public List<BrickStats> AllBricks => _allBricks;
	}
}