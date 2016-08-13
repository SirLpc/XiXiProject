using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class BossCtr : MonoBehaviour
{
	#region ===字段===

    private BossState _curState;
    private Animator _anim;

	#endregion

	#region ===属性===
    public BossState CurState { get { return _curState; } }
    public Animator Anim { get { return _anim; } }
	#endregion

	#region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _curState = BossState.INACTIVE;
    }

    private float _lastAtkTime;
    private void Start()
    {
        //InvokeRepeating("Attack", 0, 5);
    }

    private void Attack()
    {
        if (CurState == BossState.INACTIVE)
            return;

        Anim.SetTrigger(Consts.AniTriggerAtk1);
    }

	#endregion

	#region ===方法===

    public void ActiveBoss()
    {
        if (_curState != BossState.INACTIVE)
            return;

        gameObject.SetActive(true);
        _anim.SetBool(Consts.AniIsActive, true);
        _curState = BossState.IDEL;
    }

	#endregion
}
