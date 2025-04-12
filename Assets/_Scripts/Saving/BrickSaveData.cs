using System;
using System.Collections.Generic;

namespace BrickBuilder
{
	[Serializable]
	public class BrickSaveData
	{
		public List<BrickData> BricksDataList = new();
	}
}