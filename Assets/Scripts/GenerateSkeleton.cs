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
