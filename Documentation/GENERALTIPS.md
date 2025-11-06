[VRCBreeze](../README.md) | [Instructions](../Documentation/INSTRUCTIONS.md) | **General Tips** | [Guidelines](../Documentation/GUIDELINES.md) | [Download it here](https://github.com/Kadeko/VRCBreeze/releases/)

<p align="center"><img src="../Documentation/VRCB_Header.png" width="512" height="128"></p>

# General Tips:

- I recommend enabling "Move Bones At Random Time" so all your bones move at different speed!

- Bone moving in wrong direction? Try inverting X or Z axis!

- Having too many bones? You can try using "Quick Setup" tool that can be found at: "Tools/VRCBreeze/Quick Setup"!

- For Advanced Creators, generated animations can be edited! This way, you would not need to use the tool and instead, merge FX controller, Parameters and the Menu!
  - Tutorial for this will be added soon!

- If your bones are scaled into negative axis, the prefab may not work properly!

- If you have multiple armatures in your Avatar, you need to use the last child armature for the Breeze Bone!
  - Example: If you have a Hair mesh, which has its own Armature inside your Avatar's Head bone, you need to use the Hair's Armature for assigning Breeze Bones instead!
  - I recommend to merge your armatures with Modular Avatar or merge manually by using 3D software like Blender.