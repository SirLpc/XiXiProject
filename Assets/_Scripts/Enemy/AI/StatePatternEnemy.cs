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

    private void Awake()
    {
        chaseState = new ChaseState(this);
        alertState = new AlertState(this);
        patrolState = new PatrolState(this);
        attackState = new AttackState(this);

        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
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
        var curAniState = anim.GetCurrentAnimatorStateInfo(1);
        if (curAniState.IsName("UpperLayer.hurt"))
            return false;

        if (currentState == patrolState)
            return true;
        if (currentState == chaseState)
            return true;
        if (currentState == alertState)
            return true;
        if (currentState == attackState)
        {
            //anim.GetCurrentAnimatorStateInfo().
        }
        return true;
    }

    public void AttackComplete()
    {
        attackState.OnAttackComplete();
    }
}