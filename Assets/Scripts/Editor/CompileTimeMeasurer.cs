#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// cribbed from https://answers.unity.com/questions/1131497/how-to-measure-the-amount-of-time-it-takes-for-uni.html
// and https://forum.unity.com/threads/editor-is-taking-20-seconds-to-enter-playmode-in-an-empty-scene.516004/
[InitializeOnLoad]
class CompileTime : EditorWindow {
	static bool isTrackingTime;
	static bool isTrackingPlayTime;
	static bool hasPlaymodeTracked = true;
	static double startTime;
	static double playStartTime;

	static CompileTime() {
		EditorApplication.update += Update;
		startTime = PlayerPrefs.GetFloat("CompileStartTime", 0);
		if (startTime > 0) {
			isTrackingTime = true;
		}
	}


	static void Update() {
		if (EditorApplication.isCompiling && !isTrackingTime) {
			startTime = EditorApplication.timeSinceStartup;
			PlayerPrefs.SetFloat("CompileStartTime", (float)startTime);
			isTrackingTime = true;
		} else if (!EditorApplication.isCompiling && isTrackingTime) {
			var finishTime = EditorApplication.timeSinceStartup;
			isTrackingTime = false;
			var compileTime = finishTime - startTime;
			PlayerPrefs.DeleteKey("CompileStartTime");
			Debug.Log("Compile Time: \n" + compileTime.ToString("0.000") + "s");
		}

		if (EditorApplication.isPlayingOrWillChangePlaymode && !isTrackingPlayTime && !EditorApplication.isPlaying) {
			isTrackingPlayTime = true;
			playStartTime = EditorApplication.timeSinceStartup;
			PlayerPrefs.SetFloat("EnterPlaymodeStartTime", (float)playStartTime);
		} else if( EditorApplication.isPlaying && hasPlaymodeTracked) {
			hasPlaymodeTracked = false;
			var finishTime = EditorApplication.timeSinceStartup;
			var playmodeLoadTime = finishTime - PlayerPrefs.GetFloat("EnterPlaymodeStartTime");
			PlayerPrefs.DeleteKey("EnterPlaymodeStartTime");
			Debug.Log("Playmode load time: \n" + playmodeLoadTime.ToString("0.000") + "s");
		}
	}
}
#endif