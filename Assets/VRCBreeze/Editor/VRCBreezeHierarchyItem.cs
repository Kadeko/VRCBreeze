using UnityEditor;
using UnityEngine;

namespace VRCBreeze
{
    public static class VRCBreezeHierarchyItem
    {
        [MenuItem("GameObject/VRCBreeze", false, 9999)]
        private static void CreateVRCBreeze(MenuCommand menuCommand)
        {
            GameObject VRCBreeze = null;
            string[] guids = AssetDatabase.FindAssets("VRCBreeze t:Prefab");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                VRCBreeze = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }

            if (VRCBreeze != null)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(VRCBreeze);

                GameObject parent = menuCommand.context as GameObject;
                if (parent != null)
                {
                    GameObjectUtility.SetParentAndAlign(instance, parent);
                }

                Undo.RegisterCreatedObjectUndo(instance, "Create " + instance.name);
                Selection.activeObject = instance;
            }
            else
            {
                Debug.LogError("Prefab not found in project! Please install VRCBreeze properly.");
            }
        }
    }
}
