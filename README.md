**VRCBreeze** | [Instructions](Documentation/INSTRUCTIONS.md) | [General Tips](Documentation/GENERALTIPS.md) | [Guidelines](Documentation/GUIDELINES.md) | [Download it here](https://github.com/Kadeko/VRCBreeze/releases/)

<p align="center"><img src="Documentation/VRCB_Header.png" width="512" height="128"></p>
<p align="center">https://github.com/user-attachments/assets/471d30b5-6dd6-4d1b-8b4d-d84232aaf7a5</p>

VRCBreeze is Non-Destructive prefab that allows you to create any bone move in the wind. It can be your hair, clothes, anything!

## **Features:**
- Wind strength, Direction, Local & World space options, and Randomization!

- Prefab uses:
   - 4 Contact Receivers,
   - 1 Contact Sender,
   - 2 Rotation Constraints,
   - 3 Synced Parameters: 2 float & 1 boolean, in total of 17 Synced Bits.

- This prefab generates 4 animations for the wind direction:\
   Forward `(+Z)`, Backward `(-Z)`, Left `(+X)` & Right `(-X)`
   - Directions can be inverted on every individual bone.
   - Animations are bending the root bones to create wind effect. Perfect with Physbones!

- These 4 generated animations are automatically assigned into a blend tree in `FX_Breeze.controller`.

- Assigned bones, that uses Physbones, will automatically set `IsAnimated` to `true`.

- Modular Avatar merges Expression Menu & Parameters into your Avatar during publishing.

- Most important feature: It is Non-Destructive, meaning it will never change your Avatar in Unity until publishing!

## **Known Issues:**

- None! Previous issues have been fixed in Version 1.3.0!