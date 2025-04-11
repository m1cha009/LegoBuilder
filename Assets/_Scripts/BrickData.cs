using System;
using System.Collections.Generic;
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

	[Serializable]
	public class SaveData
	{
		public List<BrickData> BricksDataList = new();
	}
}