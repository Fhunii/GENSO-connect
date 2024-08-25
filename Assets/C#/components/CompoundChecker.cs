using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    hydrogenIon,
    hydroxideIon,
    Carbon
}

public class CompoundChecker : MonoBehaviour
{
    private Dictionary<HashSet<ElementType>, bool> validCompounds;

    void Start()
    {
        validCompounds = new Dictionary<HashSet<ElementType>, bool>
        {
            { new HashSet<ElementType> { ElementType.hydrogenIon, ElementType.hydroxideIon }, true }, // H2O（水）
            { new HashSet<ElementType> { ElementType.Carbon, ElementType.hydroxideIon }, true },   // CO2（二酸化炭素）
            // 他の化合物も追加する
        };
    }

    public bool IsValidCompound(HashSet<ElementType> elements)
    {
        return validCompounds.ContainsKey(elements);
    }
}
