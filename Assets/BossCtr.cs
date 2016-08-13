using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using ADBannerView = UnityEngine.iOS.ADBannerView;
using Random = UnityEngine.Random;


public class BossCtr : MonoBehaviour
{
    #region ===字段===

    [SerializeField]
    private Transform _farMostPosTr;

    private BossState _curState;
    private Animator _anim;
    private Transform _myTransform;

    private float _nextAtkTime;
    private AttackData[] _attackDatas;
    private AttackData _lastAttackData;

    private const int AtkNums = 4;
    private int[] AtkRests = new int[AtkNums] { 3, 4, 5, 6 };
    private float[] AtkDamages = new float[AtkNums] { 10, 20, 30, 0 };
    private string[] AtkNames;
    private const string SpawnName = "spawn";

    private bool _isAttackEffectived;

    public struct AttackData
    {
        public string AnimName;
        public float Rest;
        public float Damage;
        public float DisMin;
        public float DisMax;
        public BossState State;

        public AttackData(string name, float rest, float damage, float dismin, float dismax, BossState state)
        {
            this.AnimName = name;
            this.Rest = rest;
            this.Damage = damage;
            this.DisMin = dismin;
            this.DisMax = dismax;
            this.State = state;
        }
    }

    #endregion

    #region ===属性===
    public BossState CurState { get { return _curState; } }
    public Animator Anim { get { return _anim; } }
    #endregion

    #region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _myTransform = transform;
        _curState = BossState.INACTIVE;
        _nextAtkTime = float.MaxValue;
    }

    private void Start()
    {
        var pieceDis = Vector3.Distance(_farMostPosTr.position, _myTransform.position) * (1.0f / AtkNums);
        Debug.Log(Vector3.Distance(_farMostPosTr.position, _myTransform.position));
        Debug.Log(pieceDis);
        var dis = 0.0f;
        AtkNames = new string[AtkNums] { Consts.AniTriggerAtk1, Consts.AniTriggerAtk2, Consts.AniTriggerAtk3, SpawnName };
        _attackDatas = new AttackData[AtkNums];
        for (int i = 0; i < AtkNums; i++)
        {
            _attackDatas[i] = new AttackData(AtkNames[i], AtkRests[i], AtkDamages[i], dis, 0, (BossState)i);
            dis += pieceDis;
            _attackDatas[i].DisMax = i < AtkNums - 1 ? dis : float.MaxValue;
        }
    }

    private void Update()
    {
        UpdateAttack();
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

        _nextAtkTime = Time.time + 10f;
    }

    public void AttackSuccess()
    {
        if (_isAttackEffectived)
            return;

        _isAttackEffectived = true;
        PlayerController.Instance.DamangeHandler.Damage(_lastAttackData.Damage);
        Debug.LogError("Damaged " + _lastAttackData.Damage + " HP");
    }

    private void UpdateAttack()
    {
        if (CurState != BossState.IDEL)
            return;

        if (Time.time > _nextAtkTime)
        {
            _nextAtkTime = float.MaxValue;
            DoAttack(GetAttack());
        }
    }

    private AttackData GetAttack()
    {
        float curDis = Vector3.Distance(PlayerController.Instance.transform.position, _myTransform.position);
        var curData = _attackDatas[0];
        for (int i = 0; i < AtkNums; i++)
        {
            var data = _attackDatas[i];
            if (curDis.IsBetweenExclusive(data.DisMin, data.DisMax))
            {
                curData = data;
                break;
            }
        }
        if (Random.Range(0, 10) <= 7)
            return curData;

        var exceptionData = Array.FindAll(_attackDatas, data => data.AnimName != curData.AnimName);
        return exceptionData.Length > 0 ? exceptionData[Random.Range(0, exceptionData.Length)] : curData;
    }

    private void DoAttack(AttackData atkData)
    {
        _isAttackEffectived = false;
        _curState = atkData.State;
        _lastAttackData = atkData;
        if (!atkData.AnimName.Equals(SpawnName))
            Anim.SetTrigger(atkData.AnimName);
        else
            OnAttackComplete();
    }


    //Called from animation atks(if is atk spawn, from this script)
    public void OnAttackComplete()
    {
        _curState = BossState.IDEL;
        _nextAtkTime = Time.time + _lastAttackData.Rest;
        _isAttackEffectived = true;
    }
    #endregion
}
