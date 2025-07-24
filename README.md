**VRCBreeze** | [Instructions](Documentation/INSTRUCTIONS.md) | [General Tips](../Documentation/GENERALTIPS.md) | [Guidelines](Documentation/GUIDELINES.md) | [Download it here](https://github.com/Kadeko/VRCBreeze/releases/)

<p align="center"><img src="Documentation/VRCB_Header.png" width="512" height="128"></p>
<p align="center">https://github.com/user-attachments/assets/471d30b5-6dd6-4d1b-8b4d-d84232aaf7a5</p>

VRCBreeze allows you to create any bone move in the wind.

## **Features:**
- Wind strength, Direction, Local & World space options, and Randomization!

- Prefab uses 4 Contact Receivers, 1 Contact Sender, 2 Rotation Constraints, 3 Synced Parameters (2 float & 1 boolean) in total of 17 Synced Bits.

- This prefab generates 4 animations for the wind direction:\
   Forward `(+Z)`, Backward `(-Z)`, Left `(+X)` & Right `(-X)`
   - Directions can be inverted on individual bones.
   - Animations are bending the root bones (with the help of Physbones) to create wind effect.

- These 4 generated animations are automatically assigned into a blend tree in `FX_Breeze.controller`.

- Assigned bones, that have Physbone component, will automatically set `IsAnimated` to `true`. This may not work sometimes, so please double check!

> [!NOTE]
> If your Physbones are outside from the bones, you definitely need to set `IsAnimated` to `true`.

- Modular Avatar merges FX Layer, Expression Menu & Parameters into your Avatar during publishing.

# **Known Issues:**

- By using `Armature Merge` in Modular Avatar, generated animations will break due to hierarchy change (name & parent).
  - Current solution is to `uncheck Avoid Name Collisions` inside `Armature Merge` component & during play mode, click `Apply VRCBreeze to Avatar` button.

- VRCFury breaks animations that have missing objects inside generated animations.
  - Current solution is disabling VRCFury, if that happens with animations.
 
- Physbones, that are outside of the bones, will not have `IsAnimated` automatically set to `true`.
   - Current solution is enabling it manually.
