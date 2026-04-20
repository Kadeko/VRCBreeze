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
        [Tooltip("Bone, that will be controlled by wind.")]
        public GameObject breezeBone;
        [Tooltip("How much is this bone influenced by wind. The weight multiplies by Wind Strength and Wind Pattern Value.\nWind Strength * Wind Pattern Value * Weight."), Range(0f, 2f)]
        public float breezeBoneWeight = 1f;
        [Tooltip("Inverts rotation in X axis. (left / right)")]
        public bool invertX = false;
        [Tooltip("Inverts rotation in Z axis. (forward / backward)")]
        public bool invertZ = false;
    }

    [AddComponentMenu("VRCBreeze/VRC Breeze Creator")]
    [HelpURL("https://github.com/Kadeko/VRCBreeze/")]
    public class VRCBreezeCreator : MonoBehaviour, IEditorOnly, IVirtualizeAnimatorController
    {
        #region Variables
        [Tooltip("Assign your desired Wind Anchor here. We recommend that to be 'Hips' or 'Head'.")]
        public GameObject windAnchor;

        [Tooltip("How strong is the wind."), Min(0f)]
        public float windStrength = 10f;

        [Tooltip("The key value (Y) must be between 0 and 1. The key time (X) can be longer. The script will remove the last key and replace it with the first key to complete the Animation loop.")]
        public AnimationCurve windPattern;

        [Tooltip("If enabled, Animation keyframes are slightly shuffled.")]
        public bool moveBonesAtRandomTime = false;

        [Tooltip("If 'Move Bones At Random Time' is enabled, Animation keyframes are slightly shuffled by this value."), Min(0f)]
        public float randomRange = 0.1f;

        [Tooltip("Drag any bones here to make it move in the wind!\nDo not add any child bones inside! Physbones will move the rest of the bones itself.")]
        public BoneObjects[] bones;

        [Tooltip("Do not remove this, unless you know what you are doing!\nYou can find this component in your avatar: 'Avatar/VRCBreeze/WorldConstraint/Rotation_Source' GameObject.")]
        public VRCRotationConstraint rotationConstraint;

        [Tooltip("Requires 'FX_Breeze' Animator Controller.\nYou can find this at: 'Assets/VRCBreeze/Animations/FX_Breeze.controller'")]
        public RuntimeAnimatorController sourceAnimatorController;

        // [Tooltip("Automatically tries to check, if your FX is using Write Defaults ON/OFF during Avatar installment. If your controller has mixed Write Defaults, we recommend to disable this option.")]
        // public bool enableAutomaticWriteDefaults = true;

        [Tooltip("Show gizmos on selected Breeze Bones. Avatar (or VRCBreeze Object) must be selected and Unity Gizmos enabled! Useful when setting up Wind Strength and Breeze Bone Weight.")]
        public bool enableGizmos = false;

        [HideInInspector]
        public int language;

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
            if (bones == null || bones.Length == 0) return;

            float height = 0.05f;
            int circleSegments = 8;

            Gizmos.color = Color.cyan;

            for (int i = 0; i < bones.Length; i++)
            {
                if (bones[i].breezeBone == null) continue;

                float radius = Mathf.Tan(windStrength * bones[i].breezeBoneWeight * Mathf.Deg2Rad) * height;

                Vector3 origin = bones[i].breezeBone.transform.position;
                Vector3 direction = bones[i].breezeBone.transform.up * height;

                Vector3 baseCenter = origin + direction;
                Quaternion rotation = Quaternion.LookRotation(bones[i].breezeBone.transform.up);
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
