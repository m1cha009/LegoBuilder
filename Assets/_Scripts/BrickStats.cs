﻿using UnityEngine;
using UnityEngine.Serialization;

namespace BrickBuilder
{
	public class BrickStats : MonoBehaviour
	{
		[FormerlySerializedAs("brickHeight")]
		[Tooltip("Plates = .0032 | Regular = .0096 | 2xRegular = .0192 | 3xRegular = .0288 | 4xRegular = .0384")]
		public float _brickHeight = 0.0032f;

		public BoxCollider BrickCollider { get; private set; }
		public Renderer BrickRenderer { get; private set; }
		public float GetBrickHeight() => _brickHeight;

		private void Awake()
		{
			BrickCollider = GetComponent<BoxCollider>();
			BrickRenderer = GetComponent<Renderer>();
		}
	}
}