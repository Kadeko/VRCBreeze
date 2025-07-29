using UnityEngine;
using UnityEditor;

namespace VRCBreeze
{
    #region Editor
    [CustomEditor(typeof(VRCBreezeCreator))]
    public sealed class VRCBreezeEditor : Editor
    {
        private Texture2D header_mainTexture;
        private Rect headerSection;
        private float headerSize = 100f;

        private static GUIStyle DefaultButtonStyle => new GUIStyle(GUI.skin.button)
        {
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.MiddleCenter,
            richText = true,
            wordWrap = false,
        };

        private void OnEnable()
        {
            header_mainTexture = Resources.Load<Texture2D>("VRCB_Header");
        }

        public override void OnInspectorGUI()
        {
            VRCBreezeCreator creator = (VRCBreezeCreator)target;
            if (creator == null) return;

            if (header_mainTexture != null)
            {
                headerSection.height = headerSize;
                headerSection.width = headerSize * 3.94f;
                headerSection.x = 0f;
                headerSection.y = 0f;

                GUI.DrawTexture(headerSection, header_mainTexture);

                EditorGUILayout.Space(headerSection.height);
            }

            EditorGUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Created by:", GUILayout.Height(25f));
            if (GUILayout.Button("Kadeko", DefaultButtonStyle, GUILayout.Height(25f)))
                Application.OpenURL("https://x.com/kadeko_vrc");
            if (GUILayout.Button("InviaWaffles", DefaultButtonStyle, GUILayout.Height(25f)))
                Application.OpenURL("https://x.com/InviaWaffles");
            if (GUILayout.Button("Vistanz", DefaultButtonStyle, GUILayout.Height(25f)))
            {
                Application.OpenURL("https://www.github.com/JLChnToZ");
                Application.OpenURL("https://xtl.booth.pm/");
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Help & Documentation", DefaultButtonStyle, GUILayout.Height(25f)))
                Application.OpenURL("https://github.com/Kadeko/VRCBreeze/");

            EditorGUILayout.Space(15f);

            DrawDefaultInspector();
        }
    }
    #endregion
}