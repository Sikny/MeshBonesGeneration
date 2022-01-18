using UnityEngine;

public class GenerateSkeleton : MonoBehaviour
{
    public Mesh torso;
    public Mesh head;
    public Mesh lUpperArm;
    public Mesh lForeArm;
    public Mesh rUpperArm;
    public Mesh rForeArm;
    public Mesh rUpperLeg;
    public Mesh rForeLeg;
    public Mesh lUpperLeg;
    public Mesh lForeLeg;

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
        
    }
}
