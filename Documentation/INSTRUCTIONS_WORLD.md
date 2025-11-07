[VRCBreeze](../README.md) | [Avatar Instructions](../Documentation/INSTRUCTIONS.md) | **World Instructions** | [General Tips](../Documentation/GENERALTIPS.md) | [Guidelines](../Documentation/GUIDELINES.md) | [Download it here](https://github.com/Kadeko/VRCBreeze/releases/)

<img src="../Documentation/Screenshot_5.png" width="512" height="372">

# Instructions for World

> [!IMPORTANT]
> - This World version prefab is still very early version and requires a lot more updates.

## **Steps:**
1) Drag `VRCBreezeWorld.prefab` inside your World.

> [!IMPORTANT]
> - **Do not unpack it. Unpacking prefab will cause problems in future updates. Only do this, if you know what you are doing!**

2) You can move `Canvas_VRCBreezeMenu` and `Visual_WindDirection` around anywhere you want it to be.\
`WindDirection_Root` will automatically move to World Origin `(0, 0, 0)` so that Avatar's with VRCBreeze can use it.

> [!NOTE]
> - `Canvas_VRCBreezeMenu` and `Visual_WindDirection` can be removed, if you do not want Players control the wind. For custom scripting, check [UdonSharp API](#udonsharp-api) below.

# UdonSharp API

## VRCBreezeWorld

Used to take control of the World Breeze and Avatar's VRCBreeze.

### Public Methods
| Public Method | Description |
| --- | --- |
|`void UpdateWind()` | Method that updates Wind's Strength and Direction by reading `directionValue` and `strengthValue`. This will sync to other Players! |
|`void ToggleWorldBreeze()` | Method that toggles VRCBreeze World option. This is local! |
|`void UpdateTextLocally()` | Method that updates UI Text Values. This one requires UI with 2 Sliders & 2 Text components. This is local! |

### Public Fields
| Public Field | Description |
| --- | --- |
|`float directionValue` | Range between 0-360 for full direction. Changes Wind Direction. Is `UdonSynced`. |
|`float strengthValue` | Range between 0-1 for Wind Strength. Is `UdonSynced`. |

### Serialized Fields
| Serialized Field | Description |
| --- | --- |
|`GameObject windRoot` | Requires `WindDirection_Root` GameObject. Used in changing the Contact Sender rotation & position for Avatar's Wind Direction & Wind Strength |
| **Wind Smoothing:** |
|`bool enableSmoothing` | If enabled, adds smoothing for the wind change instead of being instant. |
|`float changingDuration` | How long does it take for the wind to change. Must have `enableSmoothing` enabled! |
|`AnimationCurve changingCurve` | Curve used in better smoothing instead of being Linear. Must have `enableSmoothing` enabled! |
| **UI:** |
|`Slider windDirectionSlider` | Slider used to set `directionValue` from it during  `UpdateWind()` method. |
|`Slider windStrengthSlider` | Slider used to set `strengthValue` from it during  `UpdateWind()` method. |
|`TMP_Text windDirectionValue` | Text used to tell current `directionValue` during  `UpdateWind()` method. |
|`TMP_Text windStrengthValue` | Text used to tell current `strengthValue` during  `UpdateWind()` method. |