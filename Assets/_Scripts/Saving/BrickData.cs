using System;
using UnityEngine;

namespace BrickBuilder
{
	[Serializable]
	public struct BrickData
	{
		public string PrefabName;
		public Vector3 Position;
		public Quaternion Rotation;
	}
}