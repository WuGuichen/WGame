using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "TWY/Settings/Game", order = 0)]
public class GameSetting : ScriptableObject
{
	[Header("Game Settings")] public string GameVersion;
	public int BuildNumber;

	[Header("Player Settings"), Space(2f)] public PlayerSettings PlayerConfig;

	[Header("Camera Settings"), Space(2f)] public CameraConfigs CameraConfig;

	[Serializable]
	public class PlayerSettings
	{
		public GameObject PlayerPrefab;
		public float MovementSpeed;
		public float RotationSpeed;
		public float GravitySpeed;
	}

	[Serializable]
	public class CameraConfigs
	{
		public float CameraLookSpeed;
		public float CameraPivotSpeed;
		public float CameraSmoothTime;
		public float MinimumPivotAngle;
		public float MaximumPivotAngle;
	}
}
