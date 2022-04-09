using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum EType { Key, Health, Grappler }

    public static float COLLIDER_DELAY = 0.5f;

    [Header("Set in Inspector")]
    public EType ItemType;

    // Awake() � Activate() ������������ ��������� �� 0.5 �������
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
