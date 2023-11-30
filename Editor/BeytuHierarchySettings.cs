using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BeytuHierarchy
{
	public class BeytuHierarchySettings : ScriptableObject
	{
		public bool isEnabled = true;
		public bool DrawBackground = true;
		public bool drawActivationToggle = true;

		public List<ColorDesign> colorDesigns = new List<ColorDesign>();

		// private void OnValidate()
		// {
		// 	Debug.Log("SO OnValidate Çalıştı");
		// 	StyleHierarchy.Initialize();
		// }

		public ColorDesign GetColorDesign(string keyChar)
		{
			foreach (ColorDesign colorDesign in colorDesigns)
			{
				if (colorDesign.keyChar.Equals(keyChar))
					return colorDesign;
			}
			return null;
		}

		[MenuItem("Hierarchy/Setup BeytuHierarchy")]
		public static void Setup()
		{
			BeytuHierarchySettings asset = ScriptableObject.CreateInstance<BeytuHierarchySettings>();
			var folder = Directory.CreateDirectory("Assets/BeytuHierarchy");
			string pathPrefix = "Assets/BeytuHierarchy";
			string path = "/BeytuHierarchySetting.asset";

			ResetAndLoadInitialSettings(asset);
			AssetDatabase.CreateAsset(asset, pathPrefix + path);
			StyleHierarchy.Initialize();
		}

		public static void ResetAndLoadInitialSettings(BeytuHierarchySettings asset)
		{
			asset.colorDesigns.Clear();
			asset.colorDesigns.Add(new ColorDesign("//", new Color32(0, 125, 255, 255), new Color32(255, 255, 255, 255), TextAnchor.MiddleCenter, FontStyle.Bold));
			asset.colorDesigns.Add(new ColorDesign("///", new Color32(0, 140, 70, 255), new Color32(255, 255, 255, 255), TextAnchor.MiddleCenter, FontStyle.Bold));
			asset.colorDesigns.Add(new ColorDesign("@", new Color32(255, 100, 0, 255), new Color32(255, 255, 255, 255), TextAnchor.MiddleCenter, FontStyle.Bold));
		}
	}

	[System.Serializable]
	public class ColorDesign
	{
		[Tooltip("Rename gameObject begin with this keychar")]
		public string keyChar;
		[Tooltip("Don't forget to change alpha to 255")]
		public Color backgroundColor;
		[Tooltip("Don't forget to change alpha to 255")]
		public Color textColor;
		public TextAnchor textAlignment;
		public FontStyle fontStyle;

		public ColorDesign(string keyChar, Color backgroundColor, Color textColor, TextAnchor textAlignment, FontStyle fontStyle)
		{
			this.keyChar = keyChar;
			this.backgroundColor = backgroundColor;
			this.textColor = textColor;
			this.textAlignment = textAlignment;
			this.fontStyle = fontStyle;
		}
	}
}