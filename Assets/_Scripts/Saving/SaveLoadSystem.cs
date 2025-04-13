using System;
using System.IO;
using UnityEngine;

namespace BrickBuilder
{
	public class SaveLoadSystem : MonoBehaviour
	{
		[SerializeField] private BuildManager _buildManager;
		
		private string _savePath;

		private void Awake()
		{
			_savePath = $"{Application.persistentDataPath}/saves/";
			
			if (!Directory.Exists(_savePath))
			{
				Directory.CreateDirectory(_savePath);
			}
		}

		public void SaveStructure(string saveName, bool obfuscate)
		{
			var saveData = new BrickSaveData();

			var builtStructure = _buildManager.GetBuiltStructure();

			foreach (var brick in builtStructure)
			{
				var brickData = new BrickData()
				{
					PrefabName = brick.name.Replace("(Clone)", ""),
					Position = brick.transform.localPosition,
					Rotation = brick.transform.rotation,
				};
				
				saveData.BricksDataList.Add(brickData);
			}
			
			var json = JsonUtility.ToJson(saveData, true);
			if (obfuscate)
			{
				json = ObfuscateJson(json);
			}
				
			var fullPath = Path.Combine(_savePath, $"{saveName}.json");
				
			File.WriteAllText(fullPath, json);
			Debug.Log($"Structure saved to {fullPath}");
		}

		public void LoadStructure(string saveName, bool isObfuscated)
		{
			var loadPath = Path.Combine(_savePath, $"{saveName}.json");

			if (!File.Exists(loadPath))
			{
				Debug.LogError($"File {loadPath} not found");
				return;
			}
			
			var json = File.ReadAllText(loadPath);
			if (isObfuscated)
			{
				json = DeobfuscateJson(json);
			}
			
			var brickSaveData = JsonUtility.FromJson<BrickSaveData>(json);
			
			_buildManager.ClearStructure();

			foreach (var brickData in brickSaveData.BricksDataList)
			{
				_buildManager.CreateBrick(brickData.PrefabName, brickData.Position, brickData.Rotation);
			}
			
			Debug.Log($"Structure loaded from {loadPath}");
		}

		public string[] GetSaveFiles()
		{
			var files = Directory.GetFiles(_savePath, "*.json");

			for (var i = 0; i < files.Length; i++)
			{
				files[i] = Path.GetFileNameWithoutExtension(files[i]);
			}

			return files;
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