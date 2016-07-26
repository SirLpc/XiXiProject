using UnityEngine;

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
    public SAHurtState saHurtState;
    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    [HideInInspector]
    public Animator anim;

    public EnemyDamageHandler DamageHandler { get; private set; }

    public bool IsInDefenseHurt
    {
        get { return DamageHandler.IsInDefenseHurt; }
    }

    public bool IsActive { get; private set; }

    private float lastHurtPlayTime;

    private EnemySpawner _spawner;
    private float _normalSpeed;
    private float _fastSpeed;

    private void Awake()
    {
        chaseState = new ChaseState(this);
        alertState = new AlertState(this);
        patrolState = new PatrolState(this);
        attackState = new AttackState(this);
        saHurtState = new SAHurtState(this);

        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        _normalSpeed = navMeshAgent.speed;
        _fastSpeed = _normalSpeed*5f;

        DamageHandler = GetComponent<EnemyDamageHandler>();

        _spawner = GetComponentInParent<EnemySpawner>();
    }

    private void Start()
    {
        currentState = patrolState;
        IsAlive = true;

        _spawner.PlayerIn.AddListener(Active);
        _spawner.playerOut.AddListener(DisActive);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Time.frameCount % 2 != 0)
            return;

        if (IsAlive)
            currentState.UpdateState();
    }

    private void OnDestroy()
    {
        _spawner.PlayerIn.RemoveListener(Active);
        _spawner.playerOut.RemoveListener(DisActive);
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

            if (IsPlayingAttack())
                return false;
        }

        lastHurtPlayTime = Time.time;

        return true;
    }

    public bool IsPlayingHurt()
    {
        var curAniState = anim.GetCurrentAnimatorStateInfo(1);
        return curAniState.IsName("UpperLayer.hurt");
    }

    public bool IsPlayingAttack()
    {
        var cur = anim.GetCurrentAnimatorStateInfo(0);
        return cur.IsName("BaseLayer.attack1") || cur.IsName("BaseLayer.attack2");
    }

    /// <summary>
    /// Called from the anmation even!(animation name: "attack2 0" and "attack1 0")
    /// </summary>
    public void AttackComplete()
    {
        attackState.OnAttackComplete();
    }



    private void Active()
    {
        IsAlive = true;
        navMeshAgent.speed = _normalSpeed;
        currentState = patrolState;
        gameObject.SetActive(true);
    }

    private void DisActive()
    {
        IsAlive = false;
        navMeshAgent.speed = _fastSpeed;
        DamageHandler.ResetHP();
        currentState.ToPatrolState();
        //Will disable gameobject when patrol arrived the origin point.
    }




}