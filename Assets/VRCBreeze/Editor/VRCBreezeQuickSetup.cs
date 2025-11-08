
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace VRCBreeze {
    public class VRCBreezeQuickSetup : EditorWindow
    {
        [Header("Important Components:"), Tooltip("Drag VRCBreeze.prefab from your Avatar! If you do not have it, get it from 'Assets/VRCBreeze/Prefabs/VRCBreeze.prefab' and drag it into your Avatar.")]
        public VRCBreezeCreator VRCBreezePrefab;

        [Header("Bone Settings:"), Tooltip("Bone, that will be controlled by wind.")]
        public List<GameObject> newBreezeBones;
        [Tooltip("How much is this bone influenced by wind. The weight multiplies by Wind Strength.\nWind Strength * Weight."), Range(0f, 2f)]
        public float newBreezeBoneWeights = 1f;

        [MenuItem("Tools/VRCBreeze/Quick Setup")]
        public static void ShowWindow()
        {
            var window = GetWindow<VRCBreezeQuickSetup>();
            window.titleContent = new GUIContent("VRCBreeze Quick Setup");
        }

        private void CreateGUI()
        {
            var serializedObject = new SerializedObject(this);
            var iterator = serializedObject.GetIterator();

            iterator.NextVisible(true);

            bool first = true;
            while (iterator.NextVisible(false))
            {
                if (first)
                {
                    first = false;
                    continue;
                }
                var field = new PropertyField(iterator.Copy());
                field.Bind(serializedObject);
                rootVisualElement.Add(field);
                rootVisualElement.Add(new SpaceElement(5));
            }

            rootVisualElement.Add(new SpaceElement(10));

            var retrieveButton = new Button(() => RetrieveBones())
            {
                text = "Copy Bones from VRCBreeze",
                tooltip = "Clears current settings from Quick Setup and Copies bones from assigned VRCBreeze prefab in your Avatar."
            };
            rootVisualElement.Add(retrieveButton);

            var clearButton = new Button(() => ClearBonesFromPrefab())
            {
                text = "Clear Bones from VRCBreeze",
                tooltip = "Removes current Breeze Bones from VRCBreeze prefab in your Avatar. Useful for resetting the list."
            };
            rootVisualElement.Add(clearButton);

            rootVisualElement.Add(new SpaceElement(10));

            var setupButton = new Button(() => SetupBones())
            {
                text = "Add New Bones to VRCBreeze",
                tooltip = "Adds new bones into your VRCBreeze Prefab settings, such as: Breeze Bones and Breeze Bone Weight! Ignores existing bones that are already in your prefab."
            };
            rootVisualElement.Add(setupButton);
        }

        private void SetupBones()
        {
            if (VRCBreezePrefab == null)
            {
                Debug.LogError("[VRCBreeze Quick Setup] Missing VRCBreeze Prefab! To fix that, drag VRCBreeze.prefab into your Avatar and then drag the prefab into this slot.");
                return;
            }

            newBreezeBones = newBreezeBones.Where(b => b != null).ToList();
            if (newBreezeBones.Count == 0)
            {
                Debug.LogError("[VRCBreeze Quick Setup] Missing Breeze Bones! Please assign them from your Avatar!");
                return;
            }

            if (VRCBreezePrefab.boneObjects == null)
            {
                VRCBreezePrefab.boneObjects = new BoneObjects[0];
            }

            HashSet<GameObject> existingBones = new HashSet<GameObject>(
                VRCBreezePrefab.boneObjects
                    .Where(x => x != null && x.breezeBone != null)
                    .Select(x => x.breezeBone)
            );

            List<BoneObjects> merged = new List<BoneObjects>();

            foreach (var bo in VRCBreezePrefab.boneObjects)
            {
                if (bo != null && bo.breezeBone != null && existingBones.Contains(bo.breezeBone))
                {
                    merged.Add(bo);
                }
            }

            foreach (var bone in newBreezeBones)
            {
                if (existingBones.Add(bone))
                {
                    merged.Add(new BoneObjects
                    {
                        breezeBone = bone,
                        breezeBoneWeight = newBreezeBoneWeights
                    });
                }
            }

            VRCBreezePrefab.boneObjects = merged.ToArray();

            EditorUtility.SetDirty(VRCBreezePrefab);
        }

        private void RetrieveBones()
        {
            if (VRCBreezePrefab == null)
            {
                Debug.LogError("[VRCBreeze Quick Setup] Missing VRCBreeze Prefab! Drag the VRCBreeze prefab from your Avatar. You will not be able to use this option, if you are setting this up for the first time!");
                return;
            }
            if (VRCBreezePrefab.boneObjects.Length <= 0)
            {
                Debug.LogError("[VRCBreeze Quick Setup] Missing Breeze Bones from the Avatar! You will not be able to use this option, if you are setting this up for the first time!");
                return;
            }

            newBreezeBones.Clear();

            for (int i = 0; i < VRCBreezePrefab.boneObjects.Length; i++)
            {
                if (VRCBreezePrefab.boneObjects[i].breezeBone == null) continue;
                newBreezeBones.Add(VRCBreezePrefab.boneObjects[i].breezeBone);
            }
        }

        private void ClearBonesFromPrefab()
        {
            if (VRCBreezePrefab == null)
            {
                Debug.LogError("[VRCBreeze Quick Setup] Missing VRCBreeze Prefab! Drag the VRCBreeze prefab from your Avatar. You will not be able to use this option, if you are setting this up for the first time!");
                return;
            }
            if (VRCBreezePrefab.boneObjects.Length <= 0)
            {
                Debug.Log("[VRCBreeze Quick Setup] You already have no Breeze Bones in the list!");
                return;
            }

            VRCBreezePrefab.boneObjects = new BoneObjects[0];

            EditorUtility.SetDirty(VRCBreezePrefab);
        }

        private class SpaceElement : VisualElement
        {
            public SpaceElement(float pixels)
            {
                style.height = pixels;
            }
        }
    }
}
