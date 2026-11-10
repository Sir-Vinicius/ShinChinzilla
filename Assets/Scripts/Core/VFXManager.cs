using System;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [Header("Prefabs de VFX")]
    [SerializeField] private GameObject impactVFXPrefab;
    [SerializeField] private GameObject blockBreakVFXPrefab;
    [SerializeField] private GameObject tailAttackVFXPrefab;
    [SerializeField] private GameObject cannonLaunchVFXPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayVFX(int vfxType, Vector3 position, Quaternion rotation)
    {
        GameObject prefabToSpawn = null;

        switch (vfxType)
        {
            case 0: prefabToSpawn = impactVFXPrefab; break;
            case 1: prefabToSpawn = blockBreakVFXPrefab; break;
            case 2: prefabToSpawn = tailAttackVFXPrefab; break;
            case 3: prefabToSpawn = cannonLaunchVFXPrefab; break;
        }

        if (prefabToSpawn != null)
        {
            GameObject vfx =Instantiate(prefabToSpawn, position, rotation);

            Destroy(vfx, 2f);  
        }
    }
}
