using UnityEngine;

public class GenerateSkeleton : MonoBehaviour
{
    public Mesh torso;
    public Mesh head;
    public Mesh lUpperArm;
    public Mesh lForeArm;
    public Mesh lHand;
    public Mesh rUpperArm;
    public Mesh rForeArm;
    public Mesh rHand;
    public Mesh lUpperLeg;
    public Mesh lForeLeg;
    public Mesh lFoot;
    public Mesh rUpperLeg;
    public Mesh rForeLeg;
    public Mesh rFoot;

    public GameObject vertexSpherePrefab;
    public GameObject minMaxPrefab;

    public Material characterMaterial;

    private Mesh[] _bones;

    public float epsilon;
    public float minDistance;

    private void OnValidate()
    {
        _bones = new[]
        {
            torso, head, lUpperArm, lForeArm, rUpperArm, lForeArm, rUpperArm, rForeArm, rUpperLeg,
            rForeLeg, lUpperLeg, lForeLeg, rHand, lHand, lFoot, rFoot
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
            boneGenerator.material = characterMaterial;
        }
    }

    private void Join(GameObject mainParent)
    {
        int nbChild = mainParent.transform.childCount;
        int index = 0;
        Transform currentParent = mainParent.transform;
        Transform currentChild = mainParent.transform.GetChild(0);

        while(index < nbChild)
        {
            BoneGenerator parentBone = currentParent.GetComponent<BoneGenerator>();
            BoneGenerator childBone = currentChild.GetComponent<BoneGenerator>();

            float distance0 = Vector3.Distance(parentBone.outBoneVectorMax,childBone.outBoneVectorMax);
            float distance1 = Vector3.Distance(parentBone.outBoneVectorMax,childBone.outBoneVectorMin);
            float distance2 = Vector3.Distance(parentBone.outBoneVectorMin,childBone.outBoneVectorMin);
            float distance3 = Vector3.Distance(parentBone.outBoneVectorMin,childBone.outBoneVectorMax);

            Vector3 parentPoint, childPoint;
            parentPoint = Vector3.zero;
            childPoint = Vector3.zero;

            float distance;

            distance = (distance0 < distance1) ? distance0 : distance1;
            distance = (distance < distance2) ? distance : distance2;
            distance = (distance < distance3) ? distance : distance3;

            if(distance == distance0)
            {
                parentPoint = parentBone.outBoneVectorMax;
                childPoint = childBone.outBoneVectorMax;
            }
            if(distance == distance1)
            {
                parentPoint = parentBone.outBoneVectorMax;
                childPoint = childBone.outBoneVectorMin;
            }
            if(distance == distance2)
            {
                parentPoint = parentBone.outBoneVectorMin;
                childPoint = childBone.outBoneVectorMin;
            }
            if(distance == distance3)
            {
                parentPoint = parentBone.outBoneVectorMin;
                childPoint = childBone.outBoneVectorMax;
            }

            if(distance < minDistance)
            {
                if (distance < epsilon)
                {
                    Fusion(parentPoint, childPoint);
                }
                else CreateJoint(parentPoint, childPoint);
            }

            if(currentChild.childCount != 0)
            {
                currentParent = currentChild;
                currentChild = currentChild.GetChild(0);
            }
            else
            {
                index += 1;
                currentChild = mainParent.transform.GetChild(index);
            }
        }
    }


    private void Fusion(Vector3 parent, Vector3 child)
    {
        child = parent;
    }

    private void CreateJoint(Vector3 parent, Vector3 child)
    {
        Gizmos.DrawLine(parent, child);
    }
}
