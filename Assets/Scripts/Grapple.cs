using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    public enum EMode { None, GrapplerOut, GrapplerInMiss, GrapplerInHit }

    [Header("Set in Inspector")]
    public float GrappleSpeed = 10;
    public float GrappleLength = 7;
    public float GrappleInLength = 0.5f;
    public int UnsafeTileHealthPenalty = 2;
    public TextAsset MapGrappleable;

    [Header("Set Dynamically")]
    public EMode Mode = EMode.None;
    // Номера плиток, на которые можно забросить крюк
    public List<int> GrappleTiles;
    public List<int> UnsafeTiles;

    private Dray _dray;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private Collider _drayCollider;

    private GameObject _grapHead;
    private LineRenderer _grapLine;
    private Vector3 _p0, _p1;
    private int _facing;

    private Vector3[] _directions = new Vector3[] { Vector3.right, Vector3.up, Vector3.left, Vector3.down };

    private void Awake()
    {
        string gTiles = MapGrappleable.text;
        gTiles = Utils.RemoveLineEndings(gTiles);
        GrappleTiles = new List<int>();
        UnsafeTiles = new List<int>();
        for (int i = 0; i < gTiles.Length; i++)
        {
            switch (gTiles[i])
            {
                case 'S':
                    GrappleTiles.Add(i);
                    break;

                case 'X':
                    UnsafeTiles.Add(i);
                    break;
            }
        }

        _dray = GetComponent<Dray>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _drayCollider = GetComponent<Collider>();

        Transform trans = transform.Find("Grappler");
        _grapHead = trans.gameObject;
        _grapLine = _grapHead.GetComponent<LineRenderer>();
        _grapHead.SetActive(false);
    }

    private void Update()
    {
        if (!_dray.HasGrappler)
        {
            return;
        }

        switch (Mode)
        {
            case EMode.None:
                // Если нажата клавиша применения крюка
                if (Input.GetKeyDown(KeyCode.X))
                {
                    StartGrapple();
                }
                break;
        }
    }

    private void StartGrapple()
    {
        _facing = _dray.GetFacing();
        _dray.enabled = false;
        _animator.CrossFade("Dray_Attack_" + _facing, 0);
        _drayCollider.enabled = false;
        _rigidbody.velocity = Vector3.zero;

        _grapHead.SetActive(true);

        _p0 = transform.position + (_directions[_facing] * 0.5f);
        _p1 = _p0;
        _grapHead.transform.position = _p1;
        _grapHead.transform.rotation = Quaternion.Euler(0, 0, 90 * _facing);

        _grapLine.positionCount = 2;
        _grapLine.SetPosition(0, _p0);
        _grapLine.SetPosition(1, _p1);
        Mode = EMode.GrapplerOut;
    }

    private void FixedUpdate()
    {
        switch (Mode)
        {
            case EMode.GrapplerOut: // Крюк брошен
                _p1 += _directions[_facing] * GrappleSpeed * Time.fixedDeltaTime;
                _grapHead.transform.position = _p1;
                _grapLine.SetPosition(1, _p1);

                // Проверить попал ли крюк куда-нибудь
                int tileNum = TileCamera.GET_MAP(_p1.x, _p1.y);
                if (GrappleTiles.IndexOf(tileNum) != -1)
                {
                    // Крюк попал на плитку, за которую можно зацепиться!
                    Mode = EMode.GrapplerInHit;
                    break;
                }
                if ((_p1 - _p0).magnitude >= GrappleLength)
                {
                    // Крюк улетел на всю длину верёвки, но никуда не попал
                    Mode = EMode.GrapplerInMiss;
                }
                break;

            case EMode.GrapplerInMiss: // Игрок промахнулся, вернуть крюк на х2 скорости
                _p1 -= _directions[_facing] * 2 * GrappleSpeed * Time.fixedDeltaTime;
                if (Vector3.Dot((_p1 - _p0), _directions[_facing]) > 0)
                {
                    // Крюк всё ещё перед Дреем
                    _grapHead.transform.position = _p1;
                    _grapLine.SetPosition(1, _p1);
                }
                else
                {
                    StopGrapple();
                }
                break;

            case EMode.GrapplerInHit: // Крюк зацепился, поднять Дрея на стену
                float dist = GrappleInLength + GrappleSpeed * Time.fixedDeltaTime;
                if (dist > (_p1 - _p0).magnitude)
                {
                    _p0 = _p1 - (_directions[_facing] * GrappleInLength);
                    transform.position = _p0;
                    StopGrapple();
                    break;
                }
                _p0 += _directions[_facing] * GrappleSpeed * Time.fixedDeltaTime;
                transform.position = _p0;
                _grapLine.SetPosition(0, _p0);
                _grapHead.transform.position = _p1;
                break;
        }
    }

    private void StopGrapple()
    {
        _dray.enabled = true;
        _drayCollider.enabled = true;

        // Проверить безопасность плитки
        int tileNum = TileCamera.GET_MAP(_p0.x, _p0.y);
        if (Mode == EMode.GrapplerInHit && UnsafeTiles.IndexOf(tileNum) != -1)
        {
            // Дрей попал на небезопасную плитку
            _dray.ResetInRoom(UnsafeTileHealthPenalty);
        }

        _grapHead.SetActive(false);

        Mode = EMode.None;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Enemy e = collider.GetComponent<Enemy>();
        if (e == null)
        {
            return;
        }

        Mode = EMode.GrapplerInMiss;
    }
}
