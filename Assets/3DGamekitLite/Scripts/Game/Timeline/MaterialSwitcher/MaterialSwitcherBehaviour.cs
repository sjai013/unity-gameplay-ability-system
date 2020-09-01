using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class MaterialSwitcherBehaviour : PlayableBehaviour
{
    [System.Serializable]
    public class MaterialIndexPair
    {
        public Material replacementMaterial;
        public int materialIndexToReplace;
    }

    public MaterialIndexPair[] materialIndexPairs;
    [HideInInspector]
    public Material[] materials;
    [HideInInspector]
    public bool setupCorrectly;

    public bool SetMaterials(Material[] sharedMaterialsCopy)
    {
        for (int i = 0; i < materialIndexPairs.Length; i++)
        {
            if (materialIndexPairs[i].replacementMaterial == null)
                return false;

            if (materialIndexPairs[i].materialIndexToReplace >= sharedMaterialsCopy.Length || materialIndexPairs[i].materialIndexToReplace < 0)
                return false;
        }

        materials = new Material[sharedMaterialsCopy.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            bool assigned = false;

            for (int j = 0; j < materialIndexPairs.Length; j++)
            {
                if (i == materialIndexPairs[j].materialIndexToReplace)
                {
                    materials[i] = new Material (materialIndexPairs[j].replacementMaterial);
                    assigned = true;
                }
            }

            if (!assigned)
            {
                materials[i] = new Material(sharedMaterialsCopy[i]);
            }
        }
        setupCorrectly = true;
        return true;
    }
}
