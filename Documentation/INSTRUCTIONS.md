[VRCBreeze](README.md) | **Instructions** | [Guidelines](Documentation/GUIDELINES.md)

# Instructions:

**Requires:** [Modular Avatar](https://modular-avatar.nadena.dev/)

> [!NOTE]
> Modular Avatar is used in merging FX Layers, Expression Menu & Expression Parameters.\
> If you are advanced creator and know how to merge everything, you do not need this then.

## **Steps:**
1) Drag `VRCBreeze.prefab` inside your Avatar. Do not unpack it.

2) Assign `Head` bone.

3) Assign any bone you would like to move around by wind in `Bone Objects`. **Do not assign child bones!**

4) Adjust every individual weight to your liking. You can also invert X and Z directions!

> [!WARNING]
> Do not leave Bone Weights to 0! This will do nothing in the animations.

6) Adjust `Wind Strength`. I recommend enabling Gizmos to see how much it will bend. Weight is also affecting Wind Strength\
`Wind Strength * Weight`.

7) Once you have setup all your bones, click `Apply VRCBreeze to Avatar`.\
   Generated animations can be found at `Assets/VRCBreeze/Animations/Generated/` folder.

8) Click `Finish` or delete `VRC Hair Breeze Creator` component before uploading the Avatar.


> [!TIP]
> Generated Animations can be edited! It's always better to do it yourself! Sometimes the bones may rotate in a different way due to bone rotations, so you may have to fix that manually.

> [!TIP]
> Bones that are going upwards (For example: Animal Ears) should use Inverted X and Z axis option!

# **Problems & Solutions:**

**Problem 1:** The hair doesn't move, when strength is enabled (above 0).

**Solution:** Make sure the Physbones have `IsAnimated` set to `true`.\
You may have to increase `Wind Strength` in Unity. Make sure all `Bone Weights` are `above 0`.\
Also check, if these bones still exist in the generated animations. If the objects are missing, you may have to rename Hierarchy manually, or create hair movement by yourself.
