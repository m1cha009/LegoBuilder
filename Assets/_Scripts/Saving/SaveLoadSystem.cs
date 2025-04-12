using System;
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
		}

		public void Save(string saveName, bool obfuscate, BrickSaveData bricksList)
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

		public BrickSaveData Load(string saveName, bool isObfuscated)
		{
			var json = File.ReadAllText(Path.Combine(_savePath, $"{saveName}.json"));
			if (isObfuscated)
			{
				json = DeobfuscateJson(json);
			}
			
			return JsonUtility.FromJson<BrickSaveData>(json);
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