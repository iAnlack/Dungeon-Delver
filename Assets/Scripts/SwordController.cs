using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    private GameObject _sword;
    private Dray _dray;

    private void Start()
    {
        _sword = transform.Find("Sword").gameObject;
        _dray = transform.parent.GetComponent<Dray>();

        // Деактивировать меч
        _sword.SetActive(false);
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 90 * _dray.Facing);
        _sword.SetActive(_dray.Mode == Dray.EMode.Attack);
    }
}
