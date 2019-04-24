#if UNITY_EDITOR

using System.Reflection;
using UnityEditor;

static class Shortcuts {
	[MenuItem("Tools/Clear Console %&v")]
	static void ClearConsole() {
		var assembly = Assembly.GetAssembly(typeof(SceneView));
		var type = assembly.GetType("UnityEditor.LogEntries");
		var method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
	}
}
#endif
