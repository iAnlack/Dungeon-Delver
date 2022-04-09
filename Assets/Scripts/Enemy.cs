using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static Vector3[] Directions = new Vector3[] 
        { Vector3.right, Vector3.up, Vector3.left, Vector3.down };

    [Header("Set in Inspector: Enemy")]
    public float MaxHealth = 1;
    public float KnockbackSpeed = 10;
    public float KnockbackDuration = 0.25f;
    public float InvincibleDuration = 0.5f;
    public GameObject[] RandomItemDrops;
    public GameObject GuaranteedItemDrop = null;

    [Header("Set Dynamycally: Enemy")]
    public float Health;
    public bool Invincible = false;
    public bool Knockback = false;

    private float _invincibleDone = 0;
    private float _knockbackDone = 0;
    private Vector3 _knockbackVel;

    protected Animator Animator;
    protected Rigidbody Rigidbody;
    protected SpriteRenderer SpriteRenderer;

    protected virtual void Awake()
    {
        Health = MaxHealth;
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        // Проверить состояние неуязвимости и необходимость выполнить отскок
        if (Invincible && Time.time > _invincibleDone)
        {
            Invincible = false;
        }
        SpriteRenderer.color = Invincible ? Color.red : Color.white;
        if (Knockback)
        {
            Rigidbody.velocity = _knockbackVel;
            if (Time.time < _knockbackDone)
            {
                return;
            }
        }

        Animator.speed = 1;
        Knockback = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (Invincible)
        {
            return; // Выйти, если Дрей пока неуязвим
        }
        DamageEffect damageEffect = collider.gameObject.GetComponent<DamageEffect>();
        if (damageEffect == null)
        {
            return; // Если компонент DamageEffect отсутсвует - выйти
        }

        Health -= damageEffect.Damage; // Вычесть величину ущерба из уровня здоровья
        if (Health <= 0)
        {
            Die();
        }
        Invincible = true; // Сделать Дрея неуязвимым
        _invincibleDone = Time.time + InvincibleDuration;

        if (damageEffect.Knockback) // Выполнить отбрасывание
        {
            // Определить направление отскока
            Vector3 delta = transform.position - collider.transform.root.position;
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                // Отбрасывание по горизонтали
                delta.x = (delta.x > 0) ? 1 : -1;
                delta.y = 0;
            }
            else
            {
                // Отбрасывание по вертикали
                delta.x = 0;
                delta.y = (delta.y > 0) ? 1 : -1;
            }

            // Применить скорость отбрасывания к компоненту Rigidbody
            _knockbackVel = delta * KnockbackSpeed;
            Rigidbody.velocity = _knockbackVel;

            // Установить режим Knockback и время прекращения отбрасывания
            Knockback = true;
            _knockbackDone = Time.time + KnockbackDuration;
            Animator.speed = 0;
        }
    }

    private void Die()
    {
        GameObject go;
        if (GuaranteedItemDrop != null)
        {
            go = Instantiate<GameObject>(GuaranteedItemDrop);
            go.transform.position = transform.position;
        }
        else if (RandomItemDrops.Length > 0)
        {
            int n = Random.Range(0, RandomItemDrops.Length);
            GameObject prefab = RandomItemDrops[n];
            if (prefab != null)
            {
                go = Instantiate<GameObject>(prefab);
                go.transform.position = transform.position;
            }
        }

        Destroy(gameObject);
    }
}
