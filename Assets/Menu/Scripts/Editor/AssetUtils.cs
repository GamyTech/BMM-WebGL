using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace GT.Assets
{
    public static class AssetUtils
    {
        public const string PAGE_CATEGORIES_RESOURCES_PATH = "Categories/PageCategories";
        public const string WIDGET_CATEGORIES_RESOURCES_PATH = "Categories/WidgetCategories";


        [MenuItem("Assets/Create/Categories/Pages")]
        public static PageCategoriesData CreatePageCategories()
        {
            PageCategoriesData c = CreateAsset<PageCategoriesData>("PageCategories");
            foreach (var item in Enum.GetValues(typeof(Enums.PageId)))
            {
                c.CategoriesDictionary.Add(item.ToString(), "Single Page/Common/");
            }
            return c;
        }

        [MenuItem("Assets/Create/Categories/Widgets")]
        public static WidgetCategoriesData CreateWidgetCategories()
        {
            WidgetCategoriesData c = CreateAsset<WidgetCategoriesData>("WidgetCategories");
            foreach (var item in Enum.GetValues(typeof(Enums.WidgetId)))
            {
                c.CategoriesDictionary.Add(item.ToString(), "Common/");
            }
            return c;
        }

        public static T CreateAsset<T>(string name, string overridePath = null) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            string uniqueFileName;
            if (overridePath != null)
                uniqueFileName = overridePath + "/" + name + ".asset";
            else
                uniqueFileName = AssetDatabase.GenerateUniqueAssetPath(AssetDatabase.GetAssetPath(Selection.activeObject) + "/" + name + ".asset");

            AssetDatabase.CreateAsset(asset, uniqueFileName);
            AssetDatabase.SaveAssets();
            return asset;
        }

        public static ReorderableList.AddDropdownCallbackDelegate AddPagesCallback(UnityAction<string[]> function)
        {
            PageCategoriesData categories = Resources.Load<PageCategoriesData>(PAGE_CATEGORIES_RESOURCES_PATH);
            // Adding callback
            ReorderableList.AddDropdownCallbackDelegate callback = (Rect buttonRect, ReorderableList l) =>
            {
                var menu = new GenericMenu();
                foreach (var item in categories.CategoriesDictionary)
                {
                    string pageId = item.Key;
                    string pagePath = item.Value;
                    menu.AddItem(new GUIContent(pagePath + pageId), false, () => function(new string[] { pageId }));
                }

                PagesGroup[] groups = Resources.LoadAll<PagesGroup>(PageController.GROUPS_RESOURCES_PATH);
                for (int i = 0; i < groups.Length; i++)
                {
                    string[] strs = groups[i].pageList.ToArray();
                    menu.AddItem(new GUIContent("Group/" + groups[i].groupId), false, () => function(strs));
                }
                menu.ShowAsContext();
            };
            return callback;
        }

        public static ReorderableList.AddDropdownCallbackDelegate AddWidgetsCallback(UnityAction<string[]> function)
        {
            WidgetCategoriesData categories = Resources.Load<WidgetCategoriesData>(WIDGET_CATEGORIES_RESOURCES_PATH);

            // Adding callback
            ReorderableList.AddDropdownCallbackDelegate callback = (Rect buttonRect, ReorderableList l) =>
            {
                var menu = new GenericMenu();
                foreach (var item in categories.CategoriesDictionary)
                {
                    string widgetId = item.Key;
                    string widgetPath = item.Value;
                    menu.AddItem(new GUIContent(widgetPath + widgetId), false, () => function(new string[] { widgetId }));
                }
                menu.ShowAsContext();
            };
            return callback;
        }

        public static T[] GetScriptableObjectsAtPath<T>(string path) where T : ScriptableObject
        {
            string[] assetFiles = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
            T[] assets = new T[assetFiles.Length];
            for (int i = 0; i < assetFiles.Length; i++)
            {
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetFiles[i]);
            }
            return assets;
        }
    }
}
