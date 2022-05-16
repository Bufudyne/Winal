
using UnityEngine;
using System.Collections;

public class LowResolution : MonoBehaviour
{
	
	public bool isFirstScene = true;


	private void Start ()
	{
		#region Default Settings

		// Set the game default setting when it is the first run on the device
		if (PlayerPrefs.GetInt("The First Run") != 1) // 1 = true; others = 0
		{
			// Set the default quality level
			PlayerPrefs.SetInt("Quality Level", 0);

			// Store the device original resolution
			PlayerPrefs.SetInt("OriginalX", Screen.width);

			PlayerPrefs.SetInt("OriginalY", Screen.height);
			
			//Resolution Settings
			PlayerPrefs.SetInt("Resolution Quality", 2);

			// The is not the first run anymore
			PlayerPrefs.SetInt("The First Run", 1);
		}
		
		#endregion

		#region Apply Settings

		if (!isFirstScene) return;
		if (PlayerPrefs.GetInt("Resolution Quality") == 2)
		{
			Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 0.5f),
				(int)(PlayerPrefs.GetInt("OriginalY") * 0.5f), true);
		}
		if (PlayerPrefs.GetInt("Resolution Quality") == 1)
		{
			Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 0.7f),
				(int)(PlayerPrefs.GetInt("OriginalY") * 0.7f), true);
		}
		if (PlayerPrefs.GetInt("Resolution Quality") == 0)
		{
			Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 1),
				(int)(PlayerPrefs.GetInt("OriginalY") * 1), true);
		}
			
		QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality Level"), false);

		#endregion
	}
}
