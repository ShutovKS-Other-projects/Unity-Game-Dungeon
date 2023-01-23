/* SCRIPT INSPECTOR 3
 * version 3.0.30, May 2021
 * Copyright © 2012-2021, Flipbook Games
 * 
 * Unity's legendary editor for C#, UnityScript, Boo, Shaders, and text,
 * now transformed into an advanced C# IDE!!!
 * 
 * Follow me on http://twitter.com/FlipbookGames
 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join discussion in Unity forums http://forum.unity3d.com/threads/138329
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.
 */

namespace ScriptInspector
{

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

internal static class StringAndCharExtensions
{
	static public char ToLowerAsciiInvariant(this char c)
	{
		if ('A' <= c && c <= 'Z')
		{
			c = (char)(c | 0x20);
		}
		return c;
	}

	static public bool EndsWithCS(this string a)
	{
		var len = a.Length;
		if (a.Length < 3)
			return false;
		
		if (a[len - 3] != '.')
			return false;
		
		var c = a[len - 2];
		if (c != 'c' && c != 'C')
			return false;
		
		c = a[len - 1];
		if (c != 's' && c != 's')
			return false;

		return true;
	}

	static public bool EndsWithJS(this string a)
	{
		var len = a.Length;
		if (a.Length < 3)
			return false;
		
		if (a[len - 3] != '.')
			return false;
		
		var c = a[len - 2];
		if (c != 'j' && c != 'J')
			return false;
		
		c = a[len - 1];
		if (c != 's' && c != 's')
			return false;

		return true;
	}

	static public bool EndsWithBoo(this string a)
	{
		var len = a.Length;
		if (a.Length < 4)
			return false;
		
		if (a[len - 4] != '.')
			return false;
		
		var c = a[len - 3];
		if (c != 'b' && c != 'B')
			return false;
		
		c = a[len - 2];
		if (c != 'o' && c != 'O')
			return false;
		
		c = a[len - 1];
		if (c != 'o' && c != 'O')
			return false;

		return true;
	}

	static public bool EndsWithExe(this string a)
	{
		var len = a.Length;
		if (a.Length < 4)
			return false;
		
		if (a[len - 4] != '.')
			return false;
		
		var c = a[len - 3];
		if (c != 'e' && c != 'E')
			return false;
		
		c = a[len - 2];
		if (c != 'x' && c != 'X')
			return false;
		
		c = a[len - 1];
		if (c != 'e' && c != 'E')
			return false;

		return true;
	}

	static public bool EndsWithDll(this string a)
	{
		var len = a.Length;
		if (a.Length < 4)
			return false;
		
		if (a[len - 4] != '.')
			return false;
		
		var c = a[len - 3];
		if (c != 'd' && c != 'D')
			return false;
		
		c = a[len - 2];
		if (c != 'l' && c != 'L')
			return false;
		
		c = a[len - 1];
		if (c != 'l' && c != 'L')
			return false;

		return true;
	}

	static public bool FastStartsWith(this string a, string b)
	{
		var len = b.Length;
		if (a.Length < len)
			return false;

		var i = 0;
		while (i < len && a[i] == b[i])
		{
			i++;
		}

		return i == len;
	}

	static public bool StartsWithIgnoreCase(this string a, string b)
	{
		var len = b.Length;
		if (a.Length < len)
			return false;

		var i = 0;
		while (i < len && a[i].ToLowerAsciiInvariant() == b[i])
		{
			i++;
		}

		return i == len;
	}

	static public bool FastEndsWith(this string a, string b)
	{
		var i = a.Length - 1;
		var j = b.Length - 1;
		if (i < j)
			return false;

		while (j >= 0 && a[i] == b[j])
		{
			i--;
			j--;
		}

		return j < 0;
	}
	
	static public bool EndsWithIgnoreCase(this string a, string b)
	{
		var i = a.Length - 1;
		var j = b.Length - 1;
		if (i < j)
			return false;

		while (j >= 0 && a[i].ToLowerAsciiInvariant() == b[j])
		{
			i--;
			j--;
		}

		return j < 0;
	}
}

[CustomEditor(typeof(MonoScript))]
public class ScriptInspector : Editor
{
	[SerializeField, HideInInspector]
	protected FGTextEditor textEditor = new FGTextEditor();

	[System.NonSerialized]
	protected bool enableEditor = true;

	public static string GetVersionString()
	{
		return "3.0.30, May 2021";
	}
	
	public void OnDisable()
	{
		textEditor.onRepaint = null;
		textEditor.OnDisable();
	}
	
	public override void OnInspectorGUI()
	{
		if (enableEditor)
		{
			textEditor.onRepaint = Repaint;
			textEditor.OnEnable(target);
			enableEditor = false;
		}

		if (Event.current.type == EventType.ValidateCommand)
		{
			if (Event.current.commandName == "ScriptInspector.AddTab")
			{
				Event.current.Use();
				return;
			}
		}
		else if (Event.current.type == EventType.ExecuteCommand)
		{
			if (Event.current.commandName == "ScriptInspector.AddTab")
			{
				FGCodeWindow.OpenNewWindow(null, null, false);
				Event.current.Use();
				return;
			}
		}
		
		DoGUI();
	}

	protected virtual void DoGUI()
	{
		if (textEditor == null)
			return;
		var currentInspector = GetCurrentInspector();
		textEditor.OnInspectorGUI(true, new RectOffset(0, -6, -4, 0), currentInspector);
	}
	
	private static System.Type spotlightWindowType;
	private static System.Type inspectorWindowType;
	private static FieldInfo currentInspectorWindowField;
	private static PropertyInfo currentSpotlightWindowProperty;
	private static System.Type guiViewType;
	private static System.Reflection.PropertyInfo guiViewCurrentProperty;
	private static System.Type hostViewType;
	private static System.Reflection.PropertyInfo hostViewActualViewProperty;

	public static bool IsFocused()
	{
		var windowType = EditorWindow.focusedWindow.GetType();
		return
			windowType.ToString() == "UnityEditor.InspectorWindow" ||
			spotlightWindowType != null && windowType == spotlightWindowType;
	}
 
	static ScriptInspector()
	{
		var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
		
		var spotlightAssembly = assemblies.FirstOrDefault(a => a.FullName.FastStartsWith("Spotlight,"));
		if (spotlightAssembly == null)
		{
			spotlightAssembly = assemblies.FirstOrDefault(a => a.FullName.FastStartsWith("Assembly-CSharp-Editor,"));
		}
		
		if (spotlightAssembly != null)
		{
			spotlightWindowType = spotlightAssembly.GetType("TakionStudios.Spotlight.Helper");
			if (spotlightWindowType != null)
			{
				currentSpotlightWindowProperty = spotlightWindowType.GetProperty("CurrentWindow",
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			}
		}
		
		inspectorWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
		if (inspectorWindowType != null)
		{
			currentInspectorWindowField = inspectorWindowType.GetField("s_CurrentInspectorWindow",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
		}

		guiViewType = typeof(EditorWindow).Assembly.GetType("UnityEditor.GUIView");
		if (guiViewType != null)
		{
			guiViewCurrentProperty = guiViewType.GetProperty("current");
		}

		hostViewType = typeof(EditorWindow).Assembly.GetType("UnityEditor.HostView");
		if (hostViewType != null)
		{
			hostViewActualViewProperty = hostViewType.GetProperty("actualView",
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		}
	}
	
	public static EditorWindow GetCurrentInspector()
	{
		if (currentSpotlightWindowProperty != null)
		{
			var currentInspector = currentSpotlightWindowProperty.GetValue(null, null) as EditorWindow;
			if (currentInspector != null)
				return currentInspector;
		}
		
		if (currentInspectorWindowField != null)
		{
			return currentInspectorWindowField.GetValue(null) as EditorWindow;
		}
		
		if (guiViewCurrentProperty != null)
		{
			object currentView = guiViewCurrentProperty.GetValue(null, null);
			if (currentView != null && currentView.GetType().IsSubclassOf(hostViewType) && hostViewActualViewProperty != null)
			{
				var actualView = hostViewActualViewProperty.GetValue(currentView, null) as EditorWindow;
				return actualView;
			}
		}
		
		return null;
	}
}

}
