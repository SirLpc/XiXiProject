using UnityEngine;
using System.Collections;

public class StatePatternEnemy : MonoBehaviour
{
    public float searchingTurnSpeed = 120f;
    public float searchingDuration = 4f;
    public float sightRange = 20f;
    public Transform[] wayPoints;
    public Transform eyes;
    public Vector3 offset = new Vector3(0, .5f, 0);
    public MeshRenderer meshRendererFlag;

    public float AttackNum = 0.1f;
    public float AttackInterval = 3f;

    public bool IsAlive { get; set; }

    [HideInInspector]
    public Transform chaseTarget;
    [HideInInspector]
    public IEnemyState currentState;
    [HideInInspector]
    public ChaseState chaseState;
    [HideInInspector]
    public AlertState alertState;
    [HideInInspector]
    public PatrolState patrolState;
    [HideInInspector]
    public AttackState attackState;
    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    [HideInInspector]
    public Animator anim;

    public EnemyDamageHandler DamageHandler { get; private set; }

    public bool IsInDefenseHurt
    {
        get { return DamageHandler.IsInDefenseHurt; }
    }

    private float lastHurtPlayTime;

    private void Awake()
    {
        chaseState = new ChaseState(this);
        alertState = new AlertState(this);
        patrolState = new PatrolState(this);
        attackState = new AttackState(this);

        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        DamageHandler = GetComponent<EnemyDamageHandler>();
    }

    // Use this for initialization
    void Start()
    {
        currentState = patrolState;
        IsAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsAlive)
            currentState.UpdateState();
    }

    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }

    public bool CanPlayHurtAnim()
    {
        if (Time.time - lastHurtPlayTime < 1f)
            return false;

        if (IsPlayingHurt())
            return false;

        if (currentState == attackState)
        {
            if (Time.time - lastHurtPlayTime < AttackInterval)
                return false;

            var cur = anim.GetCurrentAnimatorStateInfo(0);
            if (cur.IsName("BaseLayer.attack1") || cur.IsName("BaseLayer.attack2"))
                return false;

            //var attackT = .7f;
            //var hurtT = .7f;
            //if(Time.time < attackState.LastAttackTime + attackT || Time.time > attackState.LastAttackTime + attackT + AttackInterval - hurtT)
            //{
            //    return false;
            //}
        }

        lastHurtPlayTime = Time.time;

        //if (currentState == patrolState)
        //    return true;
        //if (currentState == chaseState)
        //    return true;
        //if (currentState == alertState)
        //    return true;
        return true;
    }

    public bool IsPlayingHurt()
    {
        var curAniState = anim.GetCurrentAnimatorStateInfo(1);
        return curAniState.IsName("UpperLayer.hurt");
    }

    /// <summary>
    /// Called from the anmation even!(animation name: "attack2 0" and "attack1 0")
    /// </summary>
    public void AttackComplete()
    {
        attackState.OnAttackComplete();
    }
}