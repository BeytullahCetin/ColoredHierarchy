using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
            Object instance = EditorUtility.InstanceIDToObject(instanceID);

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

            // Remove the symbol (keyChar) from the name.
            StringBuilder newNameBuilder = new StringBuilder();
            for (int i = 1; i < gameObjectName.Length; i++)
            {
                if (i > 1) newNameBuilder.Append(" ");
                newNameBuilder.Append(gameObjectName[i]);
            }

            string keyChar = gameObjectName[0];
            string newName = newNameBuilder.ToString();
            ColorDesign design = hierarchySettings.GetColorDesign(keyChar);

            if (design == null)
                return;

            Color colorToDraw = new Color(design.backgroundColor.r, design.backgroundColor.g, design.backgroundColor.b, 255);

            if ((instance as GameObject).activeSelf == false)
            {
                colorToDraw.r = colorToDraw.r / 2;
                colorToDraw.g = colorToDraw.g / 2;
                colorToDraw.b = colorToDraw.b / 2;
                newName = "(DISABLED) " + newName;
            }

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

            EditorGUI.DrawRect(selectionRect, colorToDraw);
            EditorGUI.LabelField(selectionRect, newName, newStyle);
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