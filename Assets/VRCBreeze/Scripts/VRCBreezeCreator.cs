using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.Constraint.Components;
using nadena.dev.ndmf;
using nadena.dev.ndmf.runtime;
using nadena.dev.ndmf.animator;

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
    [HelpURL("https://github.com/Kadeko/VRCBreeze/")]
    public class VRCBreezeCreator : MonoBehaviour, IEditorOnly, IVirtualizeAnimatorController
    {
        #region Variables
        [Header("Wind Settings:"), Tooltip("Assign your desired Wind Anchor here. We recommend that to be 'Hips' or 'Head'.")]
        public GameObject windAnchor;

        [Tooltip("How strong is the wind."), Min(0f)]
        public float windStrength = 10f;

        [Tooltip("If enabled, Bone keyframes are slightly randomized around the middle of the AnimationClip.\nIf disabled, Bone keyframes are placed in the middle of the AnimationClip.")]
        public bool moveBonesAtRandomTime = false;

        [Space, Tooltip("Drag any root bones here!")]
        public BoneObjects[] boneObjects;

        [Header("Important Components:"), Tooltip("Do not remove this, unless you know what you are doing!\nYou can find this component at: 'Rotation_Source' GameObject.")]
        public VRCRotationConstraint rotationConstraint;

        [Tooltip("Requires 'FX_Breeze' Animator Controller.\nYou can find this at: 'Assets/VRCBreeze/Animations/FX_Breeze.controller'")]
        public RuntimeAnimatorController sourceAnimatorController;

        [SerializeField]
        private bool enableGizmos = false;

        private bool isAbsolute;
        #endregion

        #region VirtualizeAnimatorController
        RuntimeAnimatorController IVirtualizeAnimatorController.AnimatorController
        {
            get => sourceAnimatorController;
            set => sourceAnimatorController = value;
        }

        object IVirtualizeAnimatorController.TargetControllerKey => VRCAvatarDescriptor.AnimLayerType.FX;

        string IVirtualizeAnimatorController.GetMotionBasePath(object ndmfBuildContext, bool clearPath)
        {
            var wasAbsolute = isAbsolute;
            isAbsolute |= clearPath;
#if UNITY_EDITOR
            if (!wasAbsolute && ndmfBuildContext is BuildContext context)
                return RuntimeUtil.RelativePath(context.AvatarRootTransform, transform) ?? "";
#endif
            return "";
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
}
