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
			
			
			// SaveMockup();
			
			LoadMockup();
		}

		public void Save(string saveName, bool obfuscate, SaveData bricksList)
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

		public SaveData Load(string saveName, bool isObfuscated)
		{
			var json = File.ReadAllText(Path.Combine(_savePath, $"{saveName}.json"));
			if (isObfuscated)
			{
				json = DeobfuscateJson(json);
			}
			
			return JsonUtility.FromJson<SaveData>(json);
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
		
		private void SaveMockup()
		{
			var brickData = new BrickData()
			{
				prefabPath = "asdasd",
				position = new Vector3(2, 3, 6),
				rotation = Quaternion.identity
			};
			
			var brickData2 = new BrickData()
			{
				prefabPath = "viva",
				position = new Vector3(1, 2, 3),
				rotation = Quaternion.identity
			};


			var saveData = new SaveData();
			saveData.BricksDataList.Add(brickData);
			saveData.BricksDataList.Add(brickData2);
			
			Save("we1", false, saveData);
		}
		
		private void LoadMockup()
		{
			var saveData = Load("we1", false);

			foreach (var brickData in saveData.BricksDataList)
			{
				Debug.Log($"PrefabPath: {brickData.prefabPath}");
				Debug.Log($"Position: {brickData.position}");
				Debug.Log($"Rotation: {brickData.rotation}");
			}
		}
	}
}