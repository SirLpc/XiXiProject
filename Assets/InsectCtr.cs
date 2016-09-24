using UnityEngine;
using System.Collections;

public class InsectCtr : MonoBehaviour
{
    private float _atk = 1f;
    private float _atkGap = 5f;

    private NavMeshAgent _agent;
    private Transform _playerTransform;
    private Transform _myTransform;
    private Animator _anim;
    public Animator Anim { get { return _anim; } }

    private float _lastAtkTime;
    private bool _isInAttack;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _myTransform = transform;
    }

	private void Start ()
    {
        _playerTransform = PlayerController.Instance.transform;
	}

	void Update ()
    {
        if (!IsInAtkScope())
            Chase();
        else
        {
            TryAtk();
            Look();
        }
	}

    private void OnDisable()
    {
        if (!_agent.isActiveAndEnabled)
            return;
        _agent.Stop();
        _agent.enabled = false;
    }

    private bool IsInAtkScope()
    {
        return Vector3.Distance(_playerTransform.position, _myTransform.position) <= _agent.stoppingDistance + .5f;
    }

    private void Chase()
    {
        if(_isInAttack)
            _anim.SetBool(Consts.AniIsInAttack, false);
        _isInAttack = false;

        _agent.destination = _playerTransform.position;
        _agent.Resume();
    }

    private void Look()
    {
        Vector3 dir = new Vector3(
            _playerTransform.position.x,
            _myTransform.position.y,
            _playerTransform.position.z);
        _myTransform.LookAt(dir);
    }

    private void TryAtk()
    {
        if(!_isInAttack)
            _anim.SetBool(Consts.AniIsInAttack, true);
        _isInAttack = true;

        if (Time.time < _lastAtkTime + _atkGap)
            return;
        _lastAtkTime = Time.time;

        if(_agent.isActiveAndEnabled)
            _agent.Stop();

        _anim.SetTrigger(Consts.AniTriggerAttack);
    }

    //called from animator attack
    public void OnAttackComplete()
    {
        PlayerController.Instance.DamangeHandler.Damage(_atk);
    }
}
