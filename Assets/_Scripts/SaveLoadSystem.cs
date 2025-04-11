using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BrickBuilder
{
	public class SaveLoadSystem : MonoBehaviour
	{
		private string _savePath;

		private void Awake()
		{
			_savePath = $"{Application.persistentDataPath}/saves/";
			
			if (!Directory.Exists(_savePath))
			{
				Directory.CreateDirectory(_savePath);
			}
			
			
			SaveMockup();
		}

		public void Save(string saveName, bool obfuscate, BrickData bricksList)
		{
			var json = JsonUtility.ToJson(bricksList, true);
			if (obfuscate)
			{
				json = ObfuscateJson(json);
			}
				
			var fullPath = Path.Combine(_savePath, $"{saveName}.json");
				
			File.WriteAllText(fullPath, json);
			Debug.Log($"Structure saved to {fullPath}");
		}

		public void Load()
		{
			
		}

		private void SaveMockup()
		{
			var bricksList = new List<BrickData>();

			var brickData = new BrickData()
			{
				prefabPath = "asdasd",
				position = new Vector3(2, 3, 6),
				rotation = Quaternion.identity
			};
			
			bricksList.Add(brickData);
			
			Save("we4", false, brickData);
		}
		
		private string ObfuscateJson(string jsonString)
		{
			var jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
			return Convert.ToBase64String(jsonBytes);
		}
		
		private string DeobfuscateJson(string obfuscatedString)
		{
			var obfuscatedBytes = Convert.FromBase64String(obfuscatedString);
			return System.Text.Encoding.UTF8.GetString(obfuscatedBytes);
		}
	}
}