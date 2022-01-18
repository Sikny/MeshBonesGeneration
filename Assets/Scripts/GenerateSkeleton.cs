using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BodyPartType
{
    Torso, Head, UpperArmL, UpperArmR, ForeArmL, ForeArmR, HandL, HandR,
    UpperLegL, UpperLegR, ForeLegL, ForeLegR, FootL, FootR
}

public class GenerateSkeleton : MonoBehaviour
{
    public SerializableDictionary<BodyPartType, Mesh> meshes;

    public GameObject vertexSpherePrefab;
    public GameObject minMaxPrefab;

    public Material characterMaterial;

    private List<Mesh> _bones;

    [SerializeField] private SerializableDictionary<BodyPartType, BoneGenerator> generatedBones;
    public float epsilon;
    public float minDistance;

    

    private void Start()
    {
        generatedBones = new SerializableDictionary<BodyPartType, BoneGenerator>();
        _bones = new List<Mesh>();
        foreach (var bodyMesh in meshes)
        {
            _bones.Add(bodyMesh.Value);
            GameObject go = new GameObject(bodyMesh.Value.name);
            var boneGenerator = go.AddComponent<BoneGenerator>();
            boneGenerator.vertexSpherePrefab = vertexSpherePrefab;
            boneGenerator.minMaxPrefab = minMaxPrefab;
            boneGenerator.sourceMesh = bodyMesh.Value;
            boneGenerator.material = characterMaterial;
            generatedBones[bodyMesh.Key] = boneGenerator;
        }

        ParentBones();
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

    private void ParentBones()
    {
        // all members
        Transform torsoT = generatedBones[BodyPartType.Torso].transform;
        generatedBones[BodyPartType.Head].transform.SetParent(torsoT);
        generatedBones[BodyPartType.UpperArmL].transform.SetParent(torsoT);
        generatedBones[BodyPartType.UpperArmR].transform.SetParent(torsoT);
        generatedBones[BodyPartType.UpperLegL].transform.SetParent(torsoT);
        generatedBones[BodyPartType.UpperLegR].transform.SetParent(torsoT);
        
        // forearms
        generatedBones[BodyPartType.ForeArmL].transform.SetParent(generatedBones[BodyPartType.UpperArmL].transform);
        generatedBones[BodyPartType.ForeArmR].transform.SetParent(generatedBones[BodyPartType.UpperArmR].transform);
        
        // forelegs
        generatedBones[BodyPartType.ForeLegL].transform.SetParent(generatedBones[BodyPartType.UpperLegL].transform);
        generatedBones[BodyPartType.ForeLegR].transform.SetParent(generatedBones[BodyPartType.UpperLegR].transform);
        
        // hands
        generatedBones[BodyPartType.HandL].transform.SetParent(generatedBones[BodyPartType.ForeArmL].transform);
        generatedBones[BodyPartType.HandR].transform.SetParent(generatedBones[BodyPartType.ForeArmR].transform);
        
        // foots
        generatedBones[BodyPartType.FootL].transform.SetParent(generatedBones[BodyPartType.ForeLegL].transform);
        generatedBones[BodyPartType.FootR].transform.SetParent(generatedBones[BodyPartType.ForeLegR].transform);
    }
}
