using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditPassiveEffectCircle : MonoBehaviour
{
    [SerializeField]
    private GameObject circle;
    public void SetMaterial(Material material)
    {
        circle.GetComponent<Renderer>().material = material;
    }
}
