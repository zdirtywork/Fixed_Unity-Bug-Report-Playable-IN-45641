# [Fixed] Unity-Bug-Report-Playable-IN-45641

**Fixed in 2021.3.30f1, 2022.3.9f1, 2023.1.10f1, 2023.2.0b7, 2023.3.0a1.**

## About this issue

When the `AnimationScriptPlayable` (asp) is output to an `AnimationLayerMixerPlayable` (_layerMixer) and the `singleLayerOptimization` parameter is set to false when creating the _layerMixer, modifying the alpha in the asp job (refer to the code in the "ModifyBoneTest.cs") does not take effect.

## How to reproduce

1. Open the "Sample" scene.
2. Select the "Player" GameObject in the Hierarchy.
3. Expand the "ModifyBoneTest" component in the Inspector, make sure the value of the "Single Layer Optimization" property is false.
4. Enter the Play mode.
5. Drag the "Bone Alpha" slider to change the value of the "Bone Alpha" property in the Inspector.
6. Observe the pose of the player's head in the Scene view.

Expected result: The pose of the player's head should change according to the value of the "Bone Alpha" property.

Actual result: The pose of the player's head does not change.

You can modify the "Single Layer Optimization", "Mode", "Bone Value" and "Bone Alpha" properties to see the effect.

PS: Only modifying the "Single Layer Optimization" in Editor mode will take effect, modifying it during Play mode will not take effect.
