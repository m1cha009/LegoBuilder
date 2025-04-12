using System;
using UnityEngine;

namespace BrickBuilder
{
	[Serializable]
	public struct BrickData
	{
		public string prefabPath;
		public Vector3 position;
		public Quaternion rotation;
		
	}
}