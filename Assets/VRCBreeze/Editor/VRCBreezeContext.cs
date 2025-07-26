using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
using nadena.dev.ndmf;
using nadena.dev.ndmf.animator;

using UnityRandom = UnityEngine.Random;
using static UnityEngine.Object;

namespace VRCBreeze
{
    [DependsOnContext(typeof(AnimatorServicesContext))]
    public sealed class VRCBreezeContext : IExtensionContext
    {
        BuildContext context;
        AnimatorServicesContext asc;
        readonly Dictionary<(VRCBreezeCreator, Direction), VirtualClip> generatedClips = new();
        VirtualAnimatorController fx;
        bool writeDefaults;

        public void OnActivate(BuildContext context)
        {
            this.context = context;
            asc = context.Extension<AnimatorServicesContext>();
            fx = asc.ControllerContext.Controllers[VRCAvatarDescriptor.AnimLayerType.FX];
            writeDefaults = CalculateWriteDefaults();
        }

        public void OnDeactivate(BuildContext context)
        {
            if (context != this.context) return;
            fx = null;
            generatedClips.Clear();
            asc = null;
            this.context = null;
        }

        public void Install(VRCBreezeCreator creator)
        {
            if (creator == null) return;

            if (creator.boneObjects.Length == 0) return;

            // Setup Physbones
            SetupPhysbones(creator);

            // Create animations
            CreateBreezeAnimation(creator, Direction.Forward);
            CreateBreezeAnimation(creator, Direction.Backward);
            CreateBreezeAnimation(creator, Direction.Left);
            CreateBreezeAnimation(creator, Direction.Right);

            // Assign animations to controller
            AssignAnimationsToController(creator);

            // Set constraints
            SetConstraints(creator);

            // Remove creator component
            DestroyImmediate(creator);
        }

        private void SetupPhysbones(VRCBreezeCreator creator)
        {
            var allPhysBones = context.AvatarRootObject.GetComponentsInChildren<VRCPhysBone>(true);
            if (allPhysBones.Length == 0) return;
            foreach (var boneObject in creator.boneObjects)
            {
                if (boneObject.breezeBone == null) continue;
                var boneTransform = boneObject.breezeBone.transform;
                foreach (var pb in allPhysBones)
                {
                    var transform = pb.GetRootTransform();
                    if (transform == boneTransform || boneTransform.IsChildOf(transform))
                    {
                        bool isIgnored = false;
                        foreach (var ignoreTransform in pb.ignoreTransforms)
                        {
                            if (ignoreTransform == boneTransform || boneTransform.IsChildOf(ignoreTransform))
                            {
                                isIgnored = true;
                                break;
                            }
                        }
                        if (!isIgnored) pb.isAnimated = true;
                    }
                }
            }
        }

        private void CreateBreezeAnimation(VRCBreezeCreator creator, Direction direction)
        {
            var clip = VirtualClip.Create("N/A");

            foreach (var boneObject in creator.boneObjects)
            {
                if (boneObject.breezeBone == null || boneObject.breezeBoneWeight == 0f) continue;

                string path = asc.ObjectPathRemapper.GetVirtualPathForObject(boneObject.breezeBone.transform);

                Vector3 axis = Vector3.up;
                float angle = creator.windStrength * boneObject.breezeBoneWeight;

                switch (direction)
                {
                    case Direction.Forward:
                        axis = boneObject.invertZ ? Vector3.forward : Vector3.back;
                        angle = Mathf.Abs(angle);
                        break;
                    case Direction.Backward:
                        axis = boneObject.invertZ ? Vector3.back : Vector3.forward;
                        angle = Mathf.Abs(angle);
                        break;
                    case Direction.Left:
                        axis = boneObject.invertX ? Vector3.right : Vector3.left;
                        angle = Mathf.Abs(angle);
                        break;
                    case Direction.Right:
                        axis = boneObject.invertX ? Vector3.left : Vector3.right;
                        angle = Mathf.Abs(angle);
                        break;
                }

                var boneTransform = boneObject.breezeBone.transform;
                Quaternion worldStartRot = boneTransform.rotation;
                Vector3 rotationAxis = Vector3.Cross(Vector3.up, axis).normalized;
                if (rotationAxis == Vector3.zero)
                    rotationAxis = Vector3.forward;

                Quaternion worldMiddleRot = Quaternion.AngleAxis(angle, rotationAxis) * worldStartRot;

                Quaternion localStartRot = worldStartRot;
                Quaternion localMiddleRot = worldMiddleRot;
                var parent = boneTransform.parent;
                if (parent != null)
                {
                    localStartRot = Quaternion.Inverse(parent.rotation) * localStartRot;
                    localMiddleRot = Quaternion.Inverse(parent.rotation) * localMiddleRot;
                }

                float keyframeTime = creator.moveBonesAtRandomTime ? UnityRandom.Range(0.35f, 0.65f) : 0.5f;

                var curveX = new AnimationCurve(new Keyframe(0f, localStartRot.x), new Keyframe(keyframeTime, localMiddleRot.x), new Keyframe(1f, localStartRot.x));
                var curveY = new AnimationCurve(new Keyframe(0f, localStartRot.y), new Keyframe(keyframeTime, localMiddleRot.y), new Keyframe(1f, localStartRot.y));
                var curveZ = new AnimationCurve(new Keyframe(0f, localStartRot.z), new Keyframe(keyframeTime, localMiddleRot.z), new Keyframe(1f, localStartRot.z));
                var curveW = new AnimationCurve(new Keyframe(0f, localStartRot.w), new Keyframe(keyframeTime, localMiddleRot.w), new Keyframe(1f, localStartRot.w));

                clip.SetFloatCurve(path, typeof(Transform), "localRotation.x", curveX);
                clip.SetFloatCurve(path, typeof(Transform), "localRotation.y", curveY);
                clip.SetFloatCurve(path, typeof(Transform), "localRotation.z", curveZ);
                clip.SetFloatCurve(path, typeof(Transform), "localRotation.w", curveW);
            }

            clip.WrapMode = WrapMode.Loop;

            switch (direction)
            {
                case Direction.Left:
                    clip.Name = "VRCBreeze_+X";
                    generatedClips[(creator, Direction.Left)] = clip;
                    break;
                case Direction.Right:
                    clip.Name = "VRCBreeze_-X";
                    generatedClips[(creator, Direction.Right)] = clip;
                    break;
                case Direction.Forward:
                    clip.Name = "VRCBreeze_+Z";
                    generatedClips[(creator, Direction.Forward)] = clip;
                    break;
                case Direction.Backward:
                    clip.Name = "VRCBreeze_-Z";
                    generatedClips[(creator, Direction.Backward)] = clip;
                    break;
            }
        }

        private void AssignAnimationsToController(VRCBreezeCreator creator)
        {
            if (!asc.ControllerContext.Controllers.TryGetValue(creator, out var sourceAnimatorController))
            {
                return;
            }

            VirtualLayer fxLayer = null;
            foreach (var layer in sourceAnimatorController.Layers)
            {
                if (string.Equals(layer.Name, "breeze", StringComparison.OrdinalIgnoreCase))
                {
                    fxLayer = layer;
                    break;
                }
            }

            if (fxLayer == null)
            {
                return;
            }

            VirtualState hairState = null;
            foreach (var state in fxLayer.StateMachine.AllStates())
            {
                if (string.Equals(state.Name, "breeze blend tree", StringComparison.OrdinalIgnoreCase))
                {
                    hairState = state;
                    break;
                }
            }
            if (hairState == null)
            {
                return;
            }

            var blendTree = hairState.Motion as VirtualBlendTree;
            if (blendTree == null)
            {
                return;
            }

            // Breeze Tree

            VirtualBlendTree breezeTree = null;
            foreach (var child in blendTree.Children)
            {
                if (child.Motion is VirtualBlendTree bt && string.Equals(bt.Name, "breeze tree", StringComparison.OrdinalIgnoreCase))
                {
                    breezeTree = bt;
                    break;
                }
            }
            if (breezeTree == null)
            {
                return;
            }

            // Breeze World

            VirtualBlendTree breezeWorld = null;
            foreach (var child in breezeTree.Children)
            {
                if (child.Motion is VirtualBlendTree bt && string.Equals(bt.Name, "world", StringComparison.OrdinalIgnoreCase))
                {
                    breezeWorld = bt;
                    break;
                }
            }
            if (breezeWorld == null)
            {
                return;
            }

            var worldChildren = breezeWorld.Children;
            if (worldChildren.Count < 4)
            {
                return;
            }

            worldChildren[(int)Direction.Left].Motion = generatedClips[(creator, Direction.Left)];
            worldChildren[(int)Direction.Right].Motion = generatedClips[(creator, Direction.Right)];
            worldChildren[(int)Direction.Forward].Motion = generatedClips[(creator, Direction.Forward)];
            worldChildren[(int)Direction.Backward].Motion = generatedClips[(creator, Direction.Backward)];

            breezeWorld.Children = worldChildren;

            // Breeze Local

            VirtualBlendTree breezeLocal = null;
            foreach (var child in breezeTree.Children)
            {
                if (child.Motion is VirtualBlendTree bt && string.Equals(bt.Name, "local", StringComparison.OrdinalIgnoreCase))
                {
                    breezeLocal = bt;
                    break;
                }
            }
            if (breezeLocal == null)
            {
                return;
            }

            var localChildren = breezeLocal.Children;
            if (localChildren.Count < 5)
            {
                return;
            }

            localChildren[0].Motion = generatedClips[(creator, Direction.Forward)];
            localChildren[1].Motion = generatedClips[(creator, Direction.Left)];
            localChildren[2].Motion = generatedClips[(creator, Direction.Backward)];
            localChildren[3].Motion = generatedClips[(creator, Direction.Right)];
            localChildren[4].Motion = generatedClips[(creator, Direction.Forward)];

            breezeLocal.Children = localChildren;

            // Install the modified controller to main FX layer
            fx.Parameters = fx.Parameters.SetItems(sourceAnimatorController.Parameters);
            foreach (var layer in sourceAnimatorController.Layers)
            {
                foreach (var state in layer.StateMachine.AllStates())
                    state.WriteDefaultValues = writeDefaults;
                fx.AddLayer(LayerPriority.Default, layer);
            }
        }

        private void SetConstraints(VRCBreezeCreator creator)
        {
            if (creator.rotationConstraint == null)
            {
                return;
            }
            if (creator.windAnchor == null)
            {
                return;
            }
            creator.rotationConstraint.Sources.Clear();

            creator.rotationConstraint.Sources.Add(new()
            {
                SourceTransform = creator.windAnchor.transform,
                Weight = 1f
            });

            creator.rotationConstraint.ActivateConstraint();
        }

        public bool CalculateWriteDefaults()
        {
            int wdOffCount = 0, wdOnCount = 0;
            foreach (var layer in fx.Layers)
            {
                if (layer.BlendingMode == AnimatorLayerBlendingMode.Additive)
                    continue;
                var stateMachine = layer.StateMachine;
                if (stateMachine.StateMachines.Count == 0 &&
                    stateMachine.States.Count == 1 &&
                    stateMachine.AnyStateTransitions.Count == 0)
                {
                    var defaultState = stateMachine.DefaultState;
                    if (defaultState != null &&
                        defaultState.Transitions.Count == 0 &&
                        defaultState.Motion is VirtualBlendTree)
                        continue;
                }
                foreach (var state in stateMachine.AllStates())
                    if (state.WriteDefaultValues)
                        wdOnCount++;
                    else
                        wdOffCount++;
            }
            return wdOnCount > wdOffCount;
        }
    }
}