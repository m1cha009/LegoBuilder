using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BrickBuilder
{
	public class SaveLoadUI : MonoBehaviour
	{
		[SerializeField] private SaveLoadSystem _saveLoadSystem;
		[SerializeField] private BuildManager _buildManager;
		
		[SerializeField] private TMP_InputField _nameInputField;
		[SerializeField] private Button _saveButton;
		[SerializeField] private Button _loadButton;
		[SerializeField] private Button _refreshButton;
		[SerializeField] private TMP_Dropdown _loadFilesDropdown;
		[SerializeField] private Button _clearButton;

		private void Awake()
		{
			_saveButton.onClick.AddListener(OnSaveButtonClicked);
			_loadButton.onClick.AddListener(OnLoadButtonClicked);
			_refreshButton.onClick.AddListener(OnRefreshButtonClicked);
			_clearButton.onClick.AddListener(OnClearButtonClicked);
		}

		private void Start()
		{
			RefreshSaveFiles();
		}

		private void OnDestroy()
		{
			_saveButton.onClick.RemoveListener(OnSaveButtonClicked);
			_loadButton.onClick.RemoveListener(OnLoadButtonClicked);
			_refreshButton.onClick.RemoveListener(OnRefreshButtonClicked);
			_clearButton.onClick.RemoveListener(OnClearButtonClicked);
		}

		private void OnLoadButtonClicked()
		{
			if (_loadFilesDropdown.options.Count == 0)
			{
				return;
			}

			var saveName = _loadFilesDropdown.options[_loadFilesDropdown.value].text;
			_saveLoadSystem.LoadStructure(saveName, false);
		}

		private void OnSaveButtonClicked()
		{
			if (string.IsNullOrEmpty(_nameInputField.text))
			{
				return;
			}
			
			_saveLoadSystem.SaveStructure(_nameInputField.text, false);
		}
		
		private void OnRefreshButtonClicked()
		{
			RefreshSaveFiles();
		}

		private void RefreshSaveFiles()
		{
			var files = _saveLoadSystem.GetSaveFiles();
			
			_loadFilesDropdown.ClearOptions();
			_loadFilesDropdown.AddOptions(files.ToList());
		}
		
		private void OnClearButtonClicked()
		{
			_buildManager.ClearStructure();
		}
	}
}