# VRCBreeze
VRCBreeze allows you to create any bone move in the wind. Supports wind strength, direction and randomization!

## **How it works:**
1) This prefab generates 4 animations for the wind direction: Forward (+Z), Backward (-Z), Left (+X) & Right (-X).
   - Animations are bending the bones (with the help of Physbones) to create wind effect.

3) These 4 animations are automatically assigned into blend tree in 'FX_Breeze.controller'.

4) Assigned bones, that have Physbone component, will automatically set "IsAnimated" to true. This may not work sometimes, so please double check!

5) Modular Avatar merges FX Layer, Expression Menu & Parameters into your Avatar during publishing.

# Instructions:

### **Requires:** Modular Avatar

> [!NOTE]
> Modular Avatar is used in merging FX Layers, Expression Menu & Expression Parameters.\
> If you are advanced creator, you do not need this.

### **Instructions:**
1) Drag VRCBreeze.prefab inside your Avatar. Do not unpack it.

2) Assign "Head" bone.

3) Assign any bone you would like to move around by wind in "Bone Objects". Do not assign child bones!

4) Adjust every individual weight to your liking. (Setting it to 0 will do nothing, so don't leave it as 0)

5) Adjust Wind Strength. I recommend enabling Gizmos to see how much it will bend. Weight is also affecting it.

6) Once you have setup all your bones, click "Apply VRCBreeze to Avatar".\
   Generated animations can be found at Assets/VRCBreeze/Animations/Generated/ folder.

8) Click "Finish" or delete "VRC Hair Breeze Creator" component before uploading the Avatar.


> [!TIP]
> Generated Animations can be edited! It's always better to do it yourself! Sometimes the bones may rotate in a different way due to bone rotations, so you may have to fix that manually.

# **Problems & Solutions:**

**Problem 1:** The hair doesn't move, when strength is enabled (above 0).

**Solution:** Make sure the Physbones have "IsAnimated" set to true.\
You may have to increase Wind Strength in Unity. Make sure all Bone Weights are above 0.\
Also check, if these bones still exist in the generated animations. If the objects are missing, you may have to rename Hierarchy manually, or create hair movement by yourself.


# **Known Issues:**

- By using "Armature Merge" in Modular Avatar, generated animations will break due to hierarchy change (name & parent).
  - Current solution is to uncheck "Avoid Name Collisions" inside "Armature Merge" component & during play mode, click "Apply VRCBreeze to Avatar" button.

- VRCFury breaks animations that have missing objects inside generated animations.
  - Current solution is not use VRCFury, if that happens with animations.
