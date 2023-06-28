using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

// About this issue:
// Modifying bones with `TransformStreamHandle` does not work properly under certain topology structures.
// 
// When the `AnimationScriptPlayable` (asp) is output to an `AnimationLayerMixerPlayable` (_layerMixer) and
// the `singleLayerOptimization` parameter is set to false when creating the _layerMixer,
// modifying the alpha in the asp job (refer to the code in the "ModifyBoneTest.cs") does not take effect.
// 
// How to reproduce:
// 1. Open the "Sample" scene.
// 2. Select the "Player" GameObject in the Hierarchy.
// 3. Expand the "ModifyBoneTest" component in the Inspector, make sure the value of the "Single Layer Optimization" property is false.
// 4. Enter the Play mode.
// 5. Drag the "Bone Alpha" slider to change the value of the "Bone Alpha" property in the Inspector.
// 6. Observe the pose of the player's head in the Scene view.
// 
// Expected result: The pose of the player's head should change according to the value of the "Bone Alpha" property.
// Actual result: The pose of the player's head does not change.
// 
// You can modify the "Single Layer Optimization", "Mode", "Bone Value" and "Bone Alpha" properties to see the effect.
// PS: Only modifying the "Single Layer Optimization" in Editor mode will take effect, modifying it during Play mode will not take effect.


public enum ModifyBoneMode : byte
{
    None,
    Scale,
    Rotation,
    Position,
}

public struct ModifyBoneJob : IAnimationJob
{
    public TransformStreamHandle boneHandle;
    public NativeReference<ModifyBoneMode> modeRef;
    public NativeReference<Vector3> boneValueRef;
    public NativeReference<float> alphaValueRef;

    public void ProcessRootMotion(AnimationStream stream) { }

    public void ProcessAnimation(AnimationStream stream)
    {
        switch (modeRef.Value)
        {
            case ModifyBoneMode.None:
                break;

            case ModifyBoneMode.Scale:
                var oriLocalScale = boneHandle.GetLocalScale(stream);
                var newLocalScale = Vector3.Lerp(oriLocalScale, boneValueRef.Value, alphaValueRef.Value);
                boneHandle.SetLocalScale(stream, newLocalScale);
                break;

            case ModifyBoneMode.Rotation:
                var oriLocalRotation = boneHandle.GetLocalRotation(stream);
                var newLocalRotation = Quaternion.Slerp(oriLocalRotation, Quaternion.Euler(boneValueRef.Value), alphaValueRef.Value);
                boneHandle.SetLocalRotation(stream, newLocalRotation);
                break;

            case ModifyBoneMode.Position:
                var oriLocalPosition = boneHandle.GetLocalPosition(stream);
                var newLocalPosition = Vector3.Lerp(oriLocalPosition, boneValueRef.Value, alphaValueRef.Value);
                boneHandle.SetLocalPosition(stream, newLocalPosition);
                break;

            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
}

[RequireComponent(typeof(Animator))]
public class ModifyBoneTest : MonoBehaviour
{
    public Transform bone;
    public bool singleLayerOptimization;
    public ModifyBoneMode mode = ModifyBoneMode.Rotation;
    public Vector3 boneValue;
    [Range(0f, 1f)]
    public float boneAlpha = 1;

    private PlayableGraph _graph;
    private AnimationLayerMixerPlayable _layerMixer;
    private NativeReference<ModifyBoneMode> _modeRef;
    private NativeReference<Vector3> _boneValueRef;
    private NativeReference<float> _alphaValueRef;

    private void Start()
    {
        var animator = GetComponent<Animator>();
        _graph = PlayableGraph.Create("ModifyBoneTest");
        _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        _modeRef = new NativeReference<ModifyBoneMode>(mode, Allocator.Persistent);
        _boneValueRef = new NativeReference<Vector3>(boneValue, Allocator.Persistent);
        _alphaValueRef = new NativeReference<float>(boneAlpha, Allocator.Persistent);

        var boneHandle = animator.BindStreamTransform(bone);
        var jobData = new ModifyBoneJob
        {
            boneHandle = boneHandle,
            modeRef = _modeRef,
            boneValueRef = _boneValueRef,
            alphaValueRef = _alphaValueRef,
        };
        var asp = AnimationScriptPlayable.Create(_graph, jobData, 1);
        _layerMixer = AnimationLayerMixerPlayable.Create(_graph, 1, singleLayerOptimization);
        _layerMixer.ConnectInput(0, asp, 0, 1f);

        var output = AnimationPlayableOutput.Create(_graph, "Animation", animator);
        output.SetSourcePlayable(_layerMixer);

        _graph.Play();
    }

    private void Update()
    {
        _modeRef.Value = mode;
        _boneValueRef.Value = boneValue;
        _alphaValueRef.Value = boneAlpha;
    }

    private void OnDestroy()
    {
        if (_graph.IsValid()) _graph.Destroy();
        if (_modeRef.IsCreated) _modeRef.Dispose();
        if (_boneValueRef.IsCreated) _boneValueRef.Dispose();
        if (_alphaValueRef.IsCreated) _alphaValueRef.Dispose();
    }
}
