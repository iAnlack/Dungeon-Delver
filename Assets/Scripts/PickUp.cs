using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum EType { Key, Health, Grappler }

    public static float COLLIDER_DELAY = 0.5f;

    [Header("Set in Inspector")]
    public EType ItemType;

    // Awake() и Activate() деактивируют коллайдер на 0.5 секунды
    private void Awake()
    {
        GetComponent<Collider>().enabled = false;
        Invoke("Activate", COLLIDER_DELAY);
    }

    private void Activate()
    {
        GetComponent<Collider>().enabled = true;
    }
}
