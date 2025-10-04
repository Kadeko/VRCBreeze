
using System;
using System.Collections.Generic;
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
        public List<GameObject> breezeBones;
        [Tooltip("How much is this bone influenced by wind. The weight multiplies by Wind Strength.\nWind Strength * Weight."), Range(0f, 2f)]
        public float breezeBoneWeight = 1f;

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

            var setupButton = new Button(() => SetupBones())
            {
                text = "Setup VRCBreeze",
                tooltip = "Warning, Destructive method! Once you click on this button, it will overwrite your VRCBreeze settings, such as: Breeze Bones and Breeze Bone Weight!"
            };
            rootVisualElement.Add(setupButton);

            rootVisualElement.Add(new SpaceElement(5));

            var retrieveButton = new Button(() => RetrieveBones())
            {
                text = "Copy Bones from VRCBreeze",
                tooltip = "Clears current settings from Quick Setup and Copies bones from assigned VRCBreeze prefab in your Avatar."
            };
            rootVisualElement.Add(retrieveButton);
        }

        private void SetupBones()
        {
            if (VRCBreezePrefab == null)
            {
                Debug.LogError("[VRCBreeze Quick Setup] Missing VRCBreeze Prefab! Please assign it from your Avatar!");
                return;
            }
            if (breezeBones.Count <= 0)
            {
                Debug.LogError("[VRCBreeze Quick Setup] Missing Breeze Bones! Please assign them from your Avatar!");
                return;
            }

            for (int i = breezeBones.Count - 1; i >= 0; i--)
            {
                if (breezeBones[i] == null)
                {
                    breezeBones.RemoveAt(i);
                }
            }

            GameObject[] bones = breezeBones.ToArray();
            VRCBreezePrefab.boneObjects = new BoneObjects[bones.Length];

            for (int i = 0; i < bones.Length; i++)
            {
                VRCBreezePrefab.boneObjects[i] = new BoneObjects
                {
                    breezeBone = bones[i],
                    breezeBoneWeight = breezeBoneWeight
                };
            }
        }

        private void RetrieveBones()
        {
            if (VRCBreezePrefab == null)
            {
                Debug.LogError("[VRCBreeze Quick Setup] Missing VRCBreeze Prefab! Please assign it from your Avatar!");
                return;
            }
            if (VRCBreezePrefab.boneObjects.Length <= 0)
            {
                Debug.LogError("[VRCBreeze Quick Setup] Missing Breeze Bones from the Avatar! You will not be able to use this option, if you are setting this up for the first time!");
                return;
            }

            breezeBones.Clear();

            for (int i = 0; i < VRCBreezePrefab.boneObjects.Length; i++)
            {
                if (VRCBreezePrefab.boneObjects[i].breezeBone == null) continue;
                breezeBones.Add(VRCBreezePrefab.boneObjects[i].breezeBone);
            }
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
