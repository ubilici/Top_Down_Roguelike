using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyRanged : LivingEntity
{

    public enum enemyType
    {
        Melee, Ranged, Boss
    };

    public enemyType unitType;

    public enum State
    {
        Idle, Chasing, Attacking
    };

    State currentState;

    public ParticleSystem deathEffect;

    public static event System.Action OnDeathStatic;

    public Material enemyMaterial;
    public Material damageMaterial;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;

    public float attackRange = 15f;
    public float attackTime = 2f;

    float attackDistanceThreshold = 5f;
    float timeBetweenAttacks = 2f;
    float damage = 1f;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    public GameObject projectile;

    bool hasTarget; 
    
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public float viewRadius = 10;
    [Range(0, 360)]
    public float viewAngle = 45;


    

    void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }

        attackDistanceThreshold = attackRange;
    }

    protected override void Start()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();

        if (hasTarget)
        {
            currentState = State.Chasing;

            targetEntity.OnDeath += OnTargetDeath;

            StartCoroutine(UpdatePath());
        }

        nextAttackTime = Time.time + attackTime;
    }

    public void SetCharacteristics(float moveSpeed, float enemyDamage, float enemyHealth, Color skinColor)
    {
        pathfinder.speed = moveSpeed;
        damage = enemyDamage;
        startingHealth = enemyHealth;
        skinMaterial = transform.GetChild(0).GetComponent<Renderer>().sharedMaterial;
        skinMaterial.color = skinColor;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!dead)
        {
            AudioManager.instance.PlaySound("Impact");
            if (damage >= health)
            {
                if (OnDeathStatic != null)
                {
                    OnDeathStatic();
                }
                AudioManager.instance.PlaySound("Enemy Death");
                Instantiate(deathEffect, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection));
            }
            base.TakeHit(damage, hitPoint, hitDirection);
        }
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    if (FindVisibleTargets())
                    {
                        Debug.Log("OK");
                        AudioManager.instance.PlaySound("Enemy Attack");
                        StartCoroutine(AttackRanged());
                        //Projectile newProjectile = Instantiate(projectile, transform.position, transform.rotation) as Projectile;

                    }

                }
            }
                transform.LookAt(FindObjectOfType<Player>().transform.position);
        }
    }

    bool FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    return true;
                }
            }

        }
        return false;
    }

    IEnumerator AttackRanged()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;


        float attackSpeed = 3;
        float percent = 0;

        transform.GetChild(0).GetComponent<Renderer>().material = damageMaterial;
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {

            if (percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                Projectile newProjectile = Instantiate(projectile, transform.position, transform.rotation) as Projectile;
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;

            yield return null;
        }

        currentState = State.Chasing;
        pathfinder.enabled = true;

        transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = enemyMaterial;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.1f;
        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * (myCollisionRadius + targetCollisionRadius);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

}
