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

    public GameObject parent;

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
            boneGenerator.Init();
        }
        
        parent = generatedBones[BodyPartType.Torso].gameObject;

        ParentBones();
        Join(parent);
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

            //Debug.Log(parentBone.name + "," + childBone.name);

            float distance0 = Vector3.Distance(parentBone.outBoneVectorMax,childBone.outBoneVectorMax);
            float distance1 = Vector3.Distance(parentBone.outBoneVectorMax,childBone.outBoneVectorMin);
            float distance2 = Vector3.Distance(parentBone.outBoneVectorMin,childBone.outBoneVectorMin);
            float distance3 = Vector3.Distance(parentBone.outBoneVectorMin,childBone.outBoneVectorMax);

            //Debug.Log(parentBone.outBoneVectorMax);

            float distance;

            distance = (distance0 < distance1) ? distance0 : distance1;
            distance = (distance < distance2) ? distance : distance2;
            distance = (distance < distance3) ? distance : distance3;

            //Debug.Log(distance0 + "," + distance1+ ","+distance2+ "," + distance3);

            if(distance < minDistance)
            {
                if (distance < epsilon)
                {
                    if (distance == distance0)
                    {
                        childBone.outBoneVectorMax = parentBone.outBoneVectorMax;
                    }
                    if (distance == distance1)
                    {
                        childBone.outBoneVectorMin = parentBone.outBoneVectorMax;
                    }
                    if (distance == distance2)
                    {
                        childBone.outBoneVectorMin = parentBone.outBoneVectorMin;
                    }
                    if (distance == distance3)
                    {
                        childBone.outBoneVectorMax = parentBone.outBoneVectorMin;
                    }
                }
                else {
                    string jointName = parentBone.gameObject.name + "To" + childBone.gameObject.name;
                    if (distance == distance0)
                    {
                        CreateJoint(childBone.outBoneVectorMax, parentBone.outBoneVectorMax, parentBone.transform, childBone.transform, jointName);
                    }
                    if (distance == distance1)
                    {
                        CreateJoint(childBone.outBoneVectorMin, parentBone.outBoneVectorMax, parentBone.transform, childBone.transform, jointName);
                    }
                    if (distance == distance2)
                    {
                        CreateJoint(childBone.outBoneVectorMin, parentBone.outBoneVectorMin, parentBone.transform, childBone.transform, jointName);
                    }
                    if (distance == distance3)
                    {
                        CreateJoint(childBone.outBoneVectorMax, parentBone.outBoneVectorMin, parentBone.transform, childBone.transform, jointName);
                    }
                }
            }

            if(currentChild.childCount != 0)
            {
                currentParent = currentChild;
                currentChild = currentChild.GetChild(0);
            }
            else
            {
                index += 1;
                if (index < mainParent.transform.childCount)
                {
                    currentParent = mainParent.transform;
                    currentChild = mainParent.transform.GetChild(index);
                }
            }
        }
    }

    private void CreateJoint(Vector3 child, Vector3 parent, Transform parentT, Transform childT, string jointName)
    {
        BoneGenerator bone = new GameObject(jointName).AddComponent<BoneGenerator>();
        var boneT = bone.transform;
        bone.outBoneVectorMin = child;
        bone.outBoneVectorMax = parent;
        boneT.SetParent(parentT);
        //childT.SetParent(boneT);
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
