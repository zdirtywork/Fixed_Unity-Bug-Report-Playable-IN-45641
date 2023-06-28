# Unity-Bug-Report-Playable-IN-45641
When the `AnimationScriptPlayable` (asp) is output to an `AnimationLayerMixerPlayable` (_layerMixer) and the `singleLayerOptimization` parameter is set to false when creating the _layerMixer, modifying the alpha in the asp job (refer to the code in the "ModifyBoneTest.cs") does not take effect.
