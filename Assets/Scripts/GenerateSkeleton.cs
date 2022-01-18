using UnityEngine;

public class GenerateSkeleton : MonoBehaviour
{
    public Mesh torso;
    public Mesh head;
    public Mesh lUpperArm;
    public Mesh lForeArm;
    public Mesh rUpperArm;
    public Mesh rForeArm;
    public Mesh lUpperLeg;
    public Mesh lForeLeg;
    public Mesh rUpperLeg;
    public Mesh rForeLeg;

    public GameObject vertexSpherePrefab;
    public GameObject minMaxPrefab;

    private Mesh[] _bones;

    private void OnValidate()
    {
        _bones = new Mesh[]
        {
            torso, head, lUpperArm, lForeArm, rUpperArm, lForeArm, rUpperArm, rForeArm, rUpperLeg,
            rForeLeg, lUpperLeg, lForeLeg
        };
    }

    private void Start()
    {
        foreach (var bone in _bones)
        {
            GameObject go = new GameObject(bone.name);
            var boneGenerator = go.AddComponent<BoneGenerator>();
            boneGenerator.vertexSpherePrefab = vertexSpherePrefab;
            boneGenerator.minMaxPrefab = minMaxPrefab;
            boneGenerator.sourceMesh = bone;
        }
    }
}
