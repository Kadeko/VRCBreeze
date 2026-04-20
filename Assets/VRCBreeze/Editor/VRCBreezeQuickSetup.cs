
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace VRCBreeze
{
    public class VRCBreezeQuickSetup : EditorWindow
    {
        [Tooltip("Drag VRCBreeze.prefab from your Avatar! If you do not have it, get it from 'Assets/VRCBreeze/Prefabs/VRCBreeze.prefab' and drag it into your Avatar.")]
        public VRCBreezeCreator VRCBreezePrefab;

        [Tooltip("Assign your desired Wind Anchor here. We recommend that to be 'Hips' or 'Head'.")]
        public HumanoidBoneOption windAnchor = HumanoidBoneOption.Unchanged;

        [Tooltip("Bones, that will be controlled by wind.")]
        public List<GameObject> newBreezeBones;

        [Tooltip("How much is this bone influenced by wind. The weight multiplies by Wind Strength.\nWind Strength * Weight."), Range(0f, 2f)]
        public float newBreezeBoneWeights = 1f;

        public SerializedObject serializedObject;

        [MenuItem("Tools/VRCBreeze/Quick Setup")]
        public static void ShowWindow()
        {
            var window = GetWindow<VRCBreezeQuickSetup>();
            window.titleContent = new GUIContent("VRCBreeze " + VRCBreezeLocalizationBridge.Get("_Quick_Setup"));
        }

        private void CreateGUI()
        {
            serializedObject = new SerializedObject(this);
            var iterator = serializedObject.GetIterator();

            rootVisualElement.Add(new SpaceElement(5));

            iterator.NextVisible(true);

            bool first = true;
            while (iterator.NextVisible(false))
            {
                if (first)
                {
                    first = false;
                    continue;
                }
                var prop = iterator.Copy();
                if (prop.name == "newBreezeBoneWeights")
                {
                    var container = new VisualElement();
                    container.style.flexDirection = FlexDirection.Row;

                    var label = new Label(VRCBreezeLocalizationBridge.Get(prop.name));
                    label.style.minWidth = 150;
                    container.Add(label);

                    var slider = new Slider(0f, 2f);
                    slider.style.flexGrow = 1;
                    slider.BindProperty(prop);
                    container.Add(slider);

                    var floatField = new FloatField();
                    floatField.style.width = 60;
                    floatField.BindProperty(prop);
                    container.Add(floatField);

                    rootVisualElement.Add(container);
                }
                else
                {
                    var field = new PropertyField(prop);
                    field.Bind(serializedObject);
                    field.label = VRCBreezeLocalizationBridge.Get(prop.name);
                    rootVisualElement.Add(field);
                }
                rootVisualElement.Add(new SpaceElement(5));
            }

            rootVisualElement.Add(new SpaceElement(10));

            var retrieveButton = new Button(() => RetrieveBones())
            {
                text = VRCBreezeLocalizationBridge.Get("_Copy_Bones_From_VRCBreeze"),
                tooltip = "Clears current settings from Quick Setup and Copies bones from assigned VRCBreeze prefab in your Avatar."
            };
            rootVisualElement.Add(retrieveButton);

            rootVisualElement.Add(new SpaceElement(5));

            var clearButton = new Button(() => ClearBonesFromPrefab())
            {
                text = VRCBreezeLocalizationBridge.Get("_Clear_Bones_From_VRCBreeze"),
                tooltip = "Removes current Breeze Bones from VRCBreeze prefab in your Avatar. Useful for resetting the list."
            };
            rootVisualElement.Add(clearButton);

            rootVisualElement.Add(new SpaceElement(10));

            rootVisualElement.Add(new Label($"<b>{VRCBreezeLocalizationBridge.Get("_Final_Step")}</b>"));
            var setupButton = new Button(() => SetupBones())
            {
                text = VRCBreezeLocalizationBridge.Get("_Add_New_Bones_To_VRCBreeze"),
                tooltip = "Adds new bones into your VRCBreeze Prefab settings, such as: Breeze Bones and Breeze Bone Weight! Ignores existing bones that are already in your prefab."
            };
            rootVisualElement.Add(setupButton);
        }

        public void SetupBones()
        {
            if (VRCBreezePrefab == null)
            {
                Debug.LogError("[VRCBreeze Quick Setup] Missing VRCBreeze Prefab! To fix that, drag VRCBreeze.prefab into your Avatar and then drag the prefab into this 'VRCBreeze Prefab' slot.");
                return;
            }

            if (windAnchor != HumanoidBoneOption.Unchanged)
            {
                var animator = VRCBreezePrefab.GetComponentInParent<Animator>();
                if (animator != null && animator.isHuman)
                {
                    var bone = animator.GetBoneTransform((HumanBodyBones)windAnchor);
                    if (bone != null)
                        VRCBreezePrefab.windAnchor = bone.gameObject;
                }
            }

            newBreezeBones = newBreezeBones.Where(b => b != null).ToList();
            if (newBreezeBones == null || newBreezeBones.Count == 0)
            {
                Debug.LogWarning("[VRCBreeze Quick Setup] Missing Breeze Bones! Please assign them from your Avatar!");
                return;
            }

            if (VRCBreezePrefab.bones == null)
                VRCBreezePrefab.bones = new BoneObjects[0];

            HashSet<GameObject> existingBones = new HashSet<GameObject>(VRCBreezePrefab.bones.Where(x => x != null && x.breezeBone != null).Select(x => x.breezeBone));

            List<BoneObjects> merged = new List<BoneObjects>();

            foreach (var bo in VRCBreezePrefab.bones)
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

            VRCBreezePrefab.bones = merged.ToArray();

            EditorUtility.SetDirty(VRCBreezePrefab);
        }

        public void RetrieveBones()
        {
            if (VRCBreezePrefab == null)
            {
                Debug.LogError("[VRCBreeze Quick Setup] Missing VRCBreeze Prefab! Drag the VRCBreeze prefab from your Avatar. You will not be able to use this option, if you are setting this up for the first time!");
                return;
            }
            if (VRCBreezePrefab.bones == null || VRCBreezePrefab.bones.Length == 0)
                return;

            if (newBreezeBones == null)
                newBreezeBones = new List<GameObject>();

            if (newBreezeBones.Count > 0)
                newBreezeBones.Clear();

            for (int i = 0; i < VRCBreezePrefab.bones.Length; i++)
            {
                if (VRCBreezePrefab.bones[i].breezeBone == null || !VRCBreezePrefab.bones[i].breezeBone.CompareTag("Untagged"))
                    continue;
                newBreezeBones.Add(VRCBreezePrefab.bones[i].breezeBone);
            }
        }

        public void ClearBonesFromPrefab()
        {
            if (VRCBreezePrefab == null)
            {
                Debug.LogError("[VRCBreeze Quick Setup] Missing VRCBreeze Prefab! Drag the VRCBreeze prefab from your Avatar. You will not be able to use this option, if you are setting this up for the first time!");
                return;
            }
            if (VRCBreezePrefab.bones.Length == 0)
                return;

            VRCBreezePrefab.bones = new BoneObjects[0];

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

    public enum HumanoidBoneOption
    {
        Unchanged = -1,
        Hips = HumanBodyBones.Hips,
        Chest = HumanBodyBones.Chest,
        Head = HumanBodyBones.Head
    }
}
