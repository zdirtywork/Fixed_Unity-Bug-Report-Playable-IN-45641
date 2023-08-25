# [My Fault] Unity-Bug-Report-Playable-IN-45641

**The issue doesn't come from Animator.angularVelocity but from calling the wrong function in my code.**

> Resolution Notes: The issue doesn't come from Animator.angularVelocity but from calling the wrong function in the user code :
> ```csharp
> var rotationAngle = _animator.angularVelocity * Mathf.Rad2Deg * deltaTime;
> transform.Rotate(rotationAngle, Space.World);
> ```
> 
> This version of transform.Rotate() takes euler angles as parameters and not an axis and an angle. The user can fix the issue by using the appropriate function that takes an axis and an angle instead :
> ```csharp
> var deltaTime = Time.deltaTime / Time.timeScale; // Time.unscaledDeltaTime is not always accurate
> var rotationAngle = _animator.angularVelocity * Mathf.Rad2Deg * deltaTime;
> transform.Rotate(rotationAngle.normalized, rotationAngle.magnitude, Space.World);
> ```

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
