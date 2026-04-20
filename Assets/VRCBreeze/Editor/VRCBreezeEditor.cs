using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace VRCBreeze
{
    [CustomEditor(typeof(VRCBreezeCreator))]
    public sealed class VRCBreezeEditor : Editor
    {
        Texture2D header_mainTexture;
        Rect headerSection;
        float headerSize = 100f;

        List<Dictionary<string, string>> texts = new List<Dictionary<string, string>>();

        private static GUIStyle DefaultButtonStyle => new GUIStyle(GUI.skin.button)
        {
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.MiddleCenter,
            richText = true,
            wordWrap = false,
        };

        private static GUIStyle DefaultHeaderStyle => new GUIStyle(GUI.skin.label)
        {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            richText = true,
            wordWrap = false,
        };

        private void OnEnable()
        {
            header_mainTexture = Resources.Load<Texture2D>("VRCB_Header");
            texts = Localize();
        }

        public override void OnInspectorGUI()
        {
            VRCBreezeCreator creator = (VRCBreezeCreator)target;
            if (creator == null) return;

            var localized_texts = new Dictionary<string, string>();
            localized_texts = texts[creator.language];
            VRCBreezeLocalizationBridge.localized_texts = localized_texts;

            if (header_mainTexture != null)
            {
                headerSection.height = headerSize;
                headerSection.width = headerSize * 3.94f;
                headerSection.x = Screen.width / 2 - headerSection.width / 2;
                headerSection.y = 0;

                GUI.DrawTexture(headerSection, header_mainTexture);

                EditorGUILayout.Space(headerSection.height);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(localized_texts["_Created_By"], DefaultHeaderStyle);
            if (GUILayout.Button("Kadeko", DefaultButtonStyle))
                Application.OpenURL("https://x.com/kadeko_vrc");
            if (GUILayout.Button("InviaWaffles", DefaultButtonStyle))
                Application.OpenURL("https://x.com/InviaWaffles");
            if (GUILayout.Button("Vistanz", DefaultButtonStyle))
            {
                Application.OpenURL("https://www.github.com/JLChnToZ");
                Application.OpenURL("https://xtl.booth.pm/");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(localized_texts["_Translators"], DefaultHeaderStyle);
            if (GUILayout.Button("季葉chowchow", DefaultButtonStyle))
                Application.OpenURL("https://x.com/chowchow0704");
            if (GUILayout.Button("Sora", DefaultButtonStyle))
                Application.OpenURL("https://x.com/vrc_sora_");
            if (GUILayout.Button("ASCEND", DefaultButtonStyle))
                Application.OpenURL("https://ascend.booth.pm/");
            if (GUILayout.Button("O_ru", DefaultButtonStyle))
                Application.OpenURL("https://x.com/oru_milkyway");
            if (GUILayout.Button("Lyxie", DefaultButtonStyle))
                Application.OpenURL("https://linktr.ee/itsLyxie");
            EditorGUILayout.EndHorizontal();

            creator.language = EditorGUILayout.Popup(localized_texts["_Select_Langauge"], (int)creator.language, new[] { 0, 1, 2, 3, 4 }.Select(i => texts[i]["_Languages"]).ToArray());

            EditorGUILayout.Space(15f);

            if (GUILayout.Button(localized_texts["_Help_Documentation"], DefaultButtonStyle))
                Application.OpenURL("https://github.com/Kadeko/VRCBreeze/");

            if (GUILayout.Button(localized_texts["_Quick_Setup"], DefaultButtonStyle))
            {
                VRCBreezeQuickSetup quickSetup = (VRCBreezeQuickSetup)EditorWindow.GetWindow(typeof(VRCBreezeQuickSetup), false, "VRCBreeze " + localized_texts["_Quick_Setup"]);
                if (quickSetup != null)
                {
                    quickSetup.serializedObject.Update();
                    quickSetup.VRCBreezePrefab = creator;
                    // quickSetup.RetrieveBones();
                    quickSetup.serializedObject.ApplyModifiedProperties();
                    quickSetup.rootVisualElement.Bind(quickSetup.serializedObject);
                }
            }
            
            EditorGUILayout.Space(15f);

            EditorGUILayout.LabelField(localized_texts["_Wind_Settings"], DefaultHeaderStyle);

            var windAnchor = serializedObject.FindProperty("windAnchor");
            EditorGUILayout.PropertyField(windAnchor, new GUIContent(localized_texts["_Wind_Anchor"]));

            var windStrength = serializedObject.FindProperty("windStrength");
            EditorGUILayout.PropertyField(windStrength, new GUIContent(localized_texts["_Wind_Strength"]));

            var windPattern = serializedObject.FindProperty("windPattern");
            EditorGUILayout.PropertyField(windPattern, new GUIContent(localized_texts["_Wind_Pattern"]));

            var moveBonesAtRandomTime = serializedObject.FindProperty("moveBonesAtRandomTime");
            EditorGUILayout.PropertyField(moveBonesAtRandomTime, new GUIContent(localized_texts["_Moving_Bones_At_Random_Time"]));

            var randomRange = serializedObject.FindProperty("randomRange");
            EditorGUILayout.PropertyField(randomRange, new GUIContent(localized_texts["_Random_Range"]));

            EditorGUILayout.Space(15f);
            EditorGUILayout.LabelField(localized_texts["_Avatar_Bones"], DefaultHeaderStyle);

            var bones = serializedObject.FindProperty("bones");
            EditorGUILayout.PropertyField(bones, new GUIContent(localized_texts["_Bones"]), true);

            EditorGUILayout.Space(15f);
            EditorGUILayout.LabelField(localized_texts["_Important_Components"], DefaultHeaderStyle);
            
            var rotationConstraint = serializedObject.FindProperty("rotationConstraint");
            EditorGUILayout.PropertyField(rotationConstraint, new GUIContent(localized_texts["_Rotation_Constraint"]));

            var sourceAnimatorController = serializedObject.FindProperty("sourceAnimatorController");
            EditorGUILayout.PropertyField(sourceAnimatorController, new GUIContent(localized_texts["_Animator_Controller"]));

            EditorGUILayout.Space(15f);
            EditorGUILayout.LabelField(localized_texts["_Debug"], DefaultHeaderStyle);

            // var enableAutomaticWriteDefaults = serializedObject.FindProperty("enableAutomaticWriteDefaults");
            // EditorGUILayout.PropertyField(enableAutomaticWriteDefaults, new GUIContent(localized_texts["_Enable_Automatic_Write_Defaults"]));

            var enableGizmos = serializedObject.FindProperty("enableGizmos");
            EditorGUILayout.PropertyField(enableGizmos, new GUIContent(localized_texts["_Enable_Gizmos"]));

            serializedObject.ApplyModifiedProperties();
            //DrawDefaultInspector();
        }

        public List<Dictionary<string, string>> Localize()
        {
            List<Dictionary<string, string>> texts = new List<Dictionary<string, string>>();
            StreamReader sr = new StreamReader(AssetDatabase.GUIDToAssetPath("26d24caea8e51d146b7e7c6a45a10688"));
            bool n = false;
            int _languageTotal = 5;

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');
                if (!n)
                {
                    for (int i = 0; i < _languageTotal; i++)
                    {
                        texts.Add(new Dictionary<string, string>());
                    }
                    n = true;
                }

                for (int j = 0; j < _languageTotal; j++)
                {
                    if (values[0] == "") break;
                    texts[j].Add(values[0], values[j + 1]);
                }
            }
            return texts;
        }
    }
}