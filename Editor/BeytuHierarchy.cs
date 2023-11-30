using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BeytuHierarchy
{
    [InitializeOnLoad]
    public class StyleHierarchy
    {
        static string[] dataArray;//Find ColorPalette GUID
        static string path;//Get ColorPalette(ScriptableObject) path
        static BeytuHierarchySettings hierarchySettings;

        private static bool isInitialized = false;

        static StyleHierarchy()
        {
            Initialize();
        }

        public static void Initialize()
        {
            dataArray = AssetDatabase.FindAssets("t:BeytuHierarchySettings");
            if (dataArray.Length == 0)
            {
                Debug.Log("<color=red>BeytuHierarchy asset not found! New Asset Created!</color>");
                BeytuHierarchySettings.Setup();
                return;
            }
            //We have only one color palette, so we use dataArray[0] to get the path of the file
            path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            hierarchySettings = AssetDatabase.LoadAssetAtPath<BeytuHierarchySettings>(path);
            if (isInitialized == true)
                EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindow;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindow;
            isInitialized = true;
        }

        private static void OnHierarchyWindow(int instanceID, Rect selectionRect)
        {
            if (hierarchySettings.isEnabled == false) return;

            //To make sure there is no error on the first time the tool imported in project
            if (dataArray.Length == 0) return;
            UnityEngine.Object instance = EditorUtility.InstanceIDToObject(instanceID);

            if (instance == null)
                return;

            DrawBackground(instance, selectionRect, hierarchySettings.DrawBackground);
            DrawEnableDisableToggle(instance, selectionRect, hierarchySettings.drawActivationToggle);
        }

        private static void DrawBackground(Object instance, Rect selectionRect, bool canDraw)
        {
            if (canDraw == false)
                return;

            string[] gameObjectName = instance.name.Split(" ");
            string keyChar = gameObjectName[0];

            ColorDesign design = hierarchySettings.GetColorDesign(keyChar);

            if (design == null)
                return;

            //Remove the symbol(keyChar) from the name.
            string newName = gameObjectName[1];

            //Draw a rectangle as a background, and set the color.
            EditorGUI.DrawRect(selectionRect, design.backgroundColor);

            //Create a new GUIStyle to match the desing in colorDesigns list.
            GUIStyle newStyle = new GUIStyle
            {
                alignment = design.textAlignment,
                fontStyle = design.fontStyle,
                normal = new GUIStyleState()
                {
                    textColor = design.textColor,
                }
            };

            //Draw a label to show the name in upper letters and newStyle.
            if ((instance as GameObject).activeSelf == true)
                EditorGUI.LabelField(selectionRect, newName, newStyle);
            else
                EditorGUI.LabelField(selectionRect, "(DISABLED) " + newName, newStyle);
        }

        private static void DrawEnableDisableToggle(Object instance, Rect selectionRect, bool canDraw)
        {
            if (canDraw == false)
                return;

            GameObject go = instance as GameObject;

            var r = new Rect(selectionRect.xMax - 16, selectionRect.yMin, 16, 16);

            var wasActive = go.activeSelf;
            var isActive = GUI.Toggle(r, wasActive, "");
            if (wasActive != isActive)
            {
                go.SetActive(isActive);
                if (EditorApplication.isPlaying == false)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(go.scene);
                    EditorUtility.SetDirty(go);
                }
            }
        }
    }
}