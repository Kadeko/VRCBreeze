#if UNITY_EDITOR
using VRC.SDKBase;
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRC.SDK3.Dynamics.Constraint.Components;
using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;

namespace VRCBreeze
{
    [System.Serializable]
    public class BoneObjects
    {
        [Tooltip("Bones, that will be controlled by wind.")]
        public GameObject breezeBone;
        [Tooltip("How much is this bone influenced by wind."), Range(0f, 1f)]
        public float breezeBoneWeight = 1f;
        [Tooltip("Inverts rotation in X (left/right) axis.")]
        public bool invertX = false;
        [Tooltip("Inverts rotation in Z (forward/backward) axis.")]
        public bool invertZ = false;
    }

    [AddComponentMenu("VRC Breeze Creator")]
    public class VRCBreezeCreator : MonoBehaviour
    {
        #region Variables
        [SerializeField, Header("Wind Settings:"), Tooltip("Assign your desired Wind Anchor here. We recommend that to be 'Hips' or 'Head'.")]
        private GameObject windAnchor;

        [SerializeField, Tooltip("How strong is the wind."), Min(0f)]
        private float windStrength = 10f;

        [SerializeField, Tooltip("If enabled, Bone keyframes are slightly randomized around the middle of the AnimationClip.\nIf disabled, Bone keyframes are placed in the middle of the AnimationClip.")]
        private bool moveBonesAtRandomTime = false;
        
        [Space, SerializeField, Tooltip("Drag any root bones here!")]
        private BoneObjects[] boneObjects;

        [SerializeField, Header("Important Components:"), Tooltip("Do not remove this, unless you know what you are doing!\nYou can find this component at: 'Rotation_Source' GameObject.")]
        private VRCRotationConstraint rotationConstraint;

        [SerializeField, Tooltip("Requires 'FX_Breeze' Animator Controller.\nYou can find this at: 'Assets/VRCBreeze/Animations/FX_Breeze.controller'")]
        private AnimatorController sourceAnimatorController;

        [Space, SerializeField, Header("Debug:"), Tooltip("If you want to keep your animations, change this number. Otherwise leave it at 0.")]
        private int id = 0;

        [SerializeField]
        private bool enableGizmos = false;

        private AnimationClip[] generatedClips = new AnimationClip[4];

        private const string PREFIX = "[<color=cyan>VRCBreezeCreator</color>]";
        #endregion

        public void Initialize()
        {
            if (boneObjects.Length == 0)
            {
                Debug.LogError($"{PREFIX} Missing Bones! Assign your bones in Bone Objects list!");
                return;
            }

            SetupPhysbones();

            CreateBreezeAnimation(Direction.Forward);
            CreateBreezeAnimation(Direction.Backward);
            CreateBreezeAnimation(Direction.Left);
            CreateBreezeAnimation(Direction.Right);
            Debug.Log($"{PREFIX} Successfully generated animations at: 'Assets/VRCBreeze/Animations/Generated/'!");

            AssignAnimationsToController();
            SetConstraints();
        }

        #region Animation Creation
        private void CreateBreezeAnimation(Direction direction)
        {
            AnimationClip clip = new AnimationClip();
            clip.legacy = false;

            for (int i = 0; i < boneObjects.Length; i++)
            {
                if (boneObjects[i].breezeBone == null || boneObjects[i].breezeBoneWeight == 0f) continue;

                string path = GetRelativePath(boneObjects[i].breezeBone.gameObject);
                int armatureIndex = path.IndexOf("Armature");
                if (armatureIndex >= 0)
                    path = path.Substring(armatureIndex);

                Quaternion worldStartRot = boneObjects[i].breezeBone.transform.rotation;

                Vector3 axis = Vector3.up;
                float angle = windStrength * boneObjects[i].breezeBoneWeight;

                switch (direction)
                {
                    case Direction.Forward:
                        axis = boneObjects[i].invertZ ? Vector3.forward : Vector3.back;
                        angle = Mathf.Abs(angle);
                        break;
                    case Direction.Backward:
                        axis = boneObjects[i].invertZ ? Vector3.back : Vector3.forward;
                        angle = Mathf.Abs(angle);
                        break;
                    case Direction.Left:
                        axis = boneObjects[i].invertX ? Vector3.right : Vector3.left;
                        angle = Mathf.Abs(angle);
                        break;
                    case Direction.Right:
                        axis = boneObjects[i].invertX ? Vector3.left : Vector3.right;
                        angle = Mathf.Abs(angle);
                        break;
                }

                Vector3 rotationAxis = Vector3.Cross(Vector3.up, axis).normalized;
                if (rotationAxis == Vector3.zero) 
                    rotationAxis = Vector3.forward;

                Quaternion worldMiddleRot = Quaternion.AngleAxis(angle, rotationAxis) * worldStartRot;

                Quaternion localStartRot = boneObjects[i].breezeBone.transform.parent != null ? Quaternion.Inverse(boneObjects[i].breezeBone.transform.parent.rotation) * worldStartRot : worldStartRot;
                Quaternion localMiddleRot = boneObjects[i].breezeBone.transform.parent != null ? Quaternion.Inverse(boneObjects[i].breezeBone.transform.parent.rotation) * worldMiddleRot: worldMiddleRot;

                float keyframeTime = moveBonesAtRandomTime ? Random.Range(0.35f, 0.65f) : 0.5f;

                AnimationCurve curveX = new AnimationCurve(new Keyframe(0f, localStartRot.x), new Keyframe(keyframeTime, localMiddleRot.x), new Keyframe(1f, localStartRot.x));
                AnimationCurve curveY = new AnimationCurve(new Keyframe(0f, localStartRot.y), new Keyframe(keyframeTime, localMiddleRot.y), new Keyframe(1f, localStartRot.y));
                AnimationCurve curveZ = new AnimationCurve(new Keyframe(0f, localStartRot.z), new Keyframe(keyframeTime, localMiddleRot.z), new Keyframe(1f, localStartRot.z));
                AnimationCurve curveW = new AnimationCurve(new Keyframe(0f, localStartRot.w), new Keyframe(keyframeTime, localMiddleRot.w), new Keyframe(1f, localStartRot.w));

                clip.SetCurve(path, typeof(Transform), "localRotation.x", curveX);
                clip.SetCurve(path, typeof(Transform), "localRotation.y", curveY);
                clip.SetCurve(path, typeof(Transform), "localRotation.z", curveZ);
                clip.SetCurve(path, typeof(Transform), "localRotation.w", curveW);
            }

            AnimationClipSettings settings = new AnimationClipSettings { loopTime = true };
            SetAnimationClipSettings(clip, settings);

            string clipName = "N/A";
            switch (direction)
            {
                case Direction.Left:
                    clipName = "VRCBreeze_+X";
                    generatedClips[(int)Direction.Left] = clip;
                    break;
                case Direction.Right:
                    clipName = "VRCBreeze_-X";
                    generatedClips[(int)Direction.Right] = clip;
                    break;
                case Direction.Forward:
                    clipName = "VRCBreeze_+Z";
                    generatedClips[(int)Direction.Forward] = clip;
                    break;
                case Direction.Backward:
                    clipName = "VRCBreeze_-Z";
                    generatedClips[(int)Direction.Backward] = clip;
                    break;
            }
            string pathAsset = $"Assets/VRCBreeze/Animations/Generated/{this.transform.parent.name}_{id}_{clipName}.anim";
            AssetDatabase.CreateAsset(clip, pathAsset);
            AssetDatabase.SaveAssets();
        }

        void SetAnimationClipSettings(AnimationClip clip, AnimationClipSettings settings)
        {
            var serializedClip = new SerializedObject(clip);
            var settingsProperty = serializedClip.FindProperty("m_AnimationClipSettings");
            settingsProperty.FindPropertyRelative("m_LoopTime").boolValue = settings.loopTime;
            serializedClip.ApplyModifiedProperties();
        }

        private string GetRelativePath(GameObject obj)
        {
            return AnimationUtility.CalculateTransformPath(obj.transform, transform);
        }
        #endregion

        #region Animator
        private void AssignAnimationsToController()
        {
            if (sourceAnimatorController == null)
            {
                Debug.LogError($"{PREFIX} Missing Animator Controller! You can find this at: 'Assets/VRCBreeze/Animations/FX_Breeze.controller'");
                return;
            }

            AnimatorControllerLayer fxLayer = null;
            foreach (var layer in sourceAnimatorController.layers)
            {
                if (layer.name.ToLower() == "breeze")
                {
                    fxLayer = layer;
                    break;
                }
            }
            if (fxLayer == null)
            {
                Debug.LogError($"{PREFIX} Missing 'Breeze' Layer! Make sure you have not changed anything, including names, inside 'FX_Layer'! Use 'FX_Layer (BACKUP)' to solve the problem.");
                return;
            }

            ChildAnimatorState hairState = default;
            foreach (var state in fxLayer.stateMachine.states)
            {
                if (state.state.name.ToLower() == "breeze blend tree")
                {
                    hairState = state;
                    break;
                }
            }
            if (hairState.state == null)
            {
                Debug.LogError($"{PREFIX} Missing 'Breeze Blend Tree' State! Make sure you have not changed anything, including names, inside 'FX_Layer'! Use 'FX_Layer (BACKUP)' to solve the problem.");
                return;
            }

            var blendTree = hairState.state.motion as BlendTree;
            if (blendTree == null)
            {
                Debug.LogError($"{PREFIX} Missing Blend Tree! Make sure you have not changed anything, including names, inside 'FX_Layer'! Use 'FX_Layer (BACKUP)' to solve the problem.");
                return;
            }

            // Breeze Tree

            BlendTree breezeTree = null;
            foreach (var child in blendTree.children)
            {
                if (child.motion is BlendTree bt && bt.name.ToLower() == "breeze tree")
                {
                    breezeTree = bt;
                    break;
                }
            }
            if (breezeTree == null)
            {
                Debug.LogError($"{PREFIX} Missing 'Breeze Tree' BlendTree! Make sure you have not changed anything, including names, inside 'FX_Layer'! Use 'FX_Layer (BACKUP)' to solve the problem.");
                return;
            }

            // Breeze World

            BlendTree breezeWorld = null;
            foreach (var child in breezeTree.children)
            {
                if (child.motion is BlendTree bt && bt.name.ToLower() == "world")
                {
                    breezeWorld = bt;
                    break;
                }
            }
            if (breezeWorld == null)
            {
                Debug.LogError($"{PREFIX} Missing 'World' BlendTree! Make sure you have not changed anything, including names, inside 'FX_Layer'! Use 'FX_Layer (BACKUP)' to solve the problem.");
                return;
            }

            var worldChildren = breezeWorld.children;
            if (worldChildren.Length < 4)
            {
                Debug.LogError($"{PREFIX} 'World' BlendTree has not enough empty motions! Make sure you have not changed anything, including names, inside 'FX_Layer'! Use 'FX_Layer (BACKUP)' to solve the problem.");
                return;
            }

            worldChildren[(int)Direction.Left].motion = generatedClips[(int)Direction.Left];
            worldChildren[(int)Direction.Right].motion = generatedClips[(int)Direction.Right];
            worldChildren[(int)Direction.Forward].motion = generatedClips[(int)Direction.Forward];
            worldChildren[(int)Direction.Backward].motion = generatedClips[(int)Direction.Backward];

            breezeWorld.children = worldChildren;

            // Breeze Local

            BlendTree breezeLocal = null;
            foreach (var child in breezeTree.children)
            {
                if (child.motion is BlendTree bt && bt.name.ToLower() == "local")
                {
                    breezeLocal = bt;
                    break;
                }
            }
            if (breezeLocal == null)
            {
                Debug.LogError($"{PREFIX} Missing 'Local' BlendTree! Make sure you have not changed anything, including names, inside 'FX_Layer'! Use 'FX_Layer (BACKUP)' to solve the problem.");
                return;
            }

            var localChildren = breezeLocal.children;
            if (localChildren.Length < 5)
            {
                Debug.LogError($"{PREFIX} 'Local' BlendTree has not enough empty motions! Make sure you have not changed anything, including names, inside 'FX_Layer'! Use 'FX_Layer (BACKUP)' to solve the problem.");
                return;
            }

            localChildren[0].motion = generatedClips[(int)Direction.Forward];
            localChildren[1].motion = generatedClips[(int)Direction.Left];
            localChildren[2].motion = generatedClips[(int)Direction.Backward];
            localChildren[3].motion = generatedClips[(int)Direction.Right];
            localChildren[4].motion = generatedClips[(int)Direction.Forward];

            breezeLocal.children = localChildren;

            EditorUtility.SetDirty(sourceAnimatorController);
            AssetDatabase.SaveAssets();

            Debug.Log($"{PREFIX} Successfully assigned animations into FX Layer!");
        }
        #endregion

        #region Components
        private void SetConstraints()
        {
            if (rotationConstraint == null)
            {
                Debug.LogError($"{PREFIX} Missing Rotation Constraint component! You can find this component at: 'Rotation_Source' GameObject.");
                return;
            }
            if (windAnchor == null)
            {
                Debug.LogError($"{PREFIX} Missing Wind Anchor! We recommend that to be 'Hips' or 'Head'.");
                return;
            }
            rotationConstraint.Sources.Clear();

            rotationConstraint.Sources.Add(new()
            {
                SourceTransform = windAnchor.transform,
                Weight = 1f
            });

            rotationConstraint.ActivateConstraint();
            Debug.Log($"{PREFIX} Successfully assigned & activated '{rotationConstraint.gameObject.name}' constraint!");
        }

        private void SetupPhysbones()
        {
            int successfulObjCount = 0;
            for (int i = 0; i < boneObjects.Length; i++)
            {
                if (boneObjects[i].breezeBone == null) continue;
                VRCPhysBone[] physBone = boneObjects[i].breezeBone.GetComponentsInChildren<VRCPhysBone>(true);
                if (physBone != null && physBone.Length > 0)
                {
                    foreach (var bone in physBone)
                    {
                        if (bone == null) continue;
                        bone.isAnimated = true;
                    }
                }
                successfulObjCount++;
            }
            Debug.Log($"{PREFIX} Successfully set 'IsAnimated' to 'true' for Physbones! [{successfulObjCount}/{boneObjects.Length}]");
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmosSelected()
        {
            if (!enableGizmos) return;
            if (boneObjects.Length == 0) return;

            float height = 0.05f;
            int circleSegments = 8;

            Gizmos.color = Color.cyan;

            for (int i = 0; i < boneObjects.Length; i++)
            {
                if (boneObjects[i].breezeBone == null) continue;

                float radius = Mathf.Tan(windStrength * boneObjects[i].breezeBoneWeight * Mathf.Deg2Rad) * height;

                Vector3 origin = boneObjects[i].breezeBone.transform.position;
                Vector3 direction = boneObjects[i].breezeBone.transform.up * height;

                Gizmos.DrawLine(origin, origin + direction);

                Vector3 baseCenter = origin + direction;
                Quaternion rotation = Quaternion.LookRotation(boneObjects[i].breezeBone.transform.up);
                Vector3 prevPoint = Vector3.zero;

                for (int c = 0; c <= circleSegments; c++)
                {
                    float theta = (c / (float)circleSegments) * Mathf.PI * 2;
                    Vector3 localPos = new Vector3(Mathf.Cos(theta) * radius, Mathf.Sin(theta) * radius, 0);
                    Vector3 worldPos = rotation * localPos + baseCenter;

                    if (c > 0)
                        Gizmos.DrawLine(prevPoint, worldPos);

                    Gizmos.DrawLine(origin, worldPos);

                    prevPoint = worldPos;
                }
            }
        }
        #endregion
    }

    public enum Direction
    {
        Left,       // +X
        Right,      // -X
        Forward,    // +Z
        Backward,   // -Z
    }

    #region Editor
    [CustomEditor(typeof(VRCBreezeCreator))]
    public class VRCBreezeEditor : Editor
    {
        private Texture2D header_mainTexture;
        private Rect headerSection;
        private float headerSize = 100f;

        private static GUIStyle DefaultButtonStyle => new GUIStyle(GUI.skin.button)
        {
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.MiddleCenter,
            richText = true,
            wordWrap = true,
        };
        private static GUIStyle DefaultButtonStyleUnwrap => new GUIStyle(GUI.skin.button)
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
                headerSection.x = 0f;/*(Screen.width / 2) - headerSection.width / 2;*/
                headerSection.y = 0f;

                GUI.DrawTexture(headerSection, header_mainTexture);

                EditorGUILayout.Space(headerSection.height);
            }

            EditorGUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Created by:", GUILayout.Height(25f)/*, GUILayout.Width(Screen.width / 6f - 20f)*/);
            if (GUILayout.Button("Kadeko", DefaultButtonStyleUnwrap, GUILayout.Height(25f)/*, GUILayout.Width(Screen.width / 6f - 20f)*/))
                Application.OpenURL("https://x.com/kadeko_vrc");
            if (GUILayout.Button("InviaWaffles", DefaultButtonStyleUnwrap, GUILayout.Height(25f)/*, GUILayout.Width(Screen.width / 6f - 20f)*/))
                Application.OpenURL("https://x.com/InviaWaffles");
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Help & Documentation", DefaultButtonStyleUnwrap, GUILayout.Height(25f)/*, GUILayout.Width(Screen.width / 6f - 20f)*/))
                Application.OpenURL("https://github.com/Kadeko/VRCBreeze/");

            EditorGUILayout.Space(15f);

            DrawDefaultInspector();

            EditorGUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Apply VRCBreeze to Avatar", "Applies this feature into Avatar. You can remove this component after."), DefaultButtonStyle, GUILayout.Height(25f)/*, GUILayout.Width(Screen.width / 2f - 20f)*/))
                creator.Initialize();
            if (GUILayout.Button(new GUIContent("Finish", "Removes this component. You can revert it by right clicking the component -> Removed Components -> Revert."), DefaultButtonStyle, GUILayout.Height(25f)/*, GUILayout.Width(Screen.width / 2f - 20f)*/))
                DestroyImmediate(creator);
            EditorGUILayout.EndHorizontal();
        }
    }
    #endregion
}
#endif