using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class BossCtr : MonoBehaviour
{
    #region ===字段===

    [SerializeField]
    private Transform _farMostPosTr;
    [SerializeField]
    private GameObject _zhiZhuPref;

    private float _lookSpeed = 10f;
    private float _activeAnimDuration = 11f;
    private float _forceBackDistance = 5f;

    private const int MaxZhiZhuNum = 5;
    public static int CurZhiZhuNum;

    private BossState _curState;
    private Animator _anim;
    private Transform _myTransform;
    private Transform _playerTransform;
    private BossUICtr _uiCtr;

    private float _nextAtkTime;
    private AttackData[] _attackDatas;
    private AttackData _lastAttackData;

    private const int AtkNums = 4;
    private int[] AtkRests = new int[AtkNums] { 3, 4, 5, 6 };
    private float[] AtkDamages = new float[AtkNums] { 10, 20, 30, 0 };
    private string[] AtkNames;
    private const string SpawnName = "spawn";

    private bool _isAttackEffectived;
    private bool _isActived;

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
    public BossUICtr UICtr { get { return _uiCtr; } }
    #endregion

    #region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _uiCtr = GetComponentInChildren<BossUICtr>();
        _myTransform = transform;
        _curState = BossState.INACTIVE;
        _nextAtkTime = float.MaxValue;
        _isActived = false;
    }

    private void Start()
    {
        _playerTransform = PlayerController.Instance.transform;

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

        _uiCtr.EnableHpSlider(false);
    }

    private void Update()
    {
        UpdateAttack();
    }

    private void LateUpdate()
    {
        LateUpdateLookAtPlayer();
    }

    #endregion

    #region ===方法===

    public void ActiveBoss()
    {
        if (_isActived || _curState != BossState.INACTIVE)
            return;

        _isActived = true;

        _anim.SetBool(Consts.AniIsActive, _isActived);

        StartCoroutine(CoCompleteActive());
    }

    public void DisActiveBoss()
    {
        _curState = BossState.INACTIVE;
        _nextAtkTime = float.MaxValue;

        _uiCtr.EnableHpSlider(false);
    }

    public void AttackSuccess()
    {
        if (_isAttackEffectived)
            return;

        _isAttackEffectived = true;
        PlayerController.Instance.DamangeHandler.Damage(_lastAttackData.Damage);
        PlayerController.Instance.ForceMove(_myTransform.forward * _forceBackDistance);
    }

    private void UpdateAttack()
    {
        if (CurState != BossState.IDEL || CurState == BossState.INACTIVE)
            return;

        if (Time.time > _nextAtkTime)
        {
            _nextAtkTime = float.MaxValue;
            DoAttack(GetAttack());
        }
    }

    private void LateUpdateLookAtPlayer()
    {
        if (CurState != BossState.IDEL)
            return;

        var pY = new Vector3(_playerTransform.position.x, 0, _playerTransform.position.z);
        var mY = new Vector3(_myTransform.position.x, 0, _myTransform.position.z);
        Vector3 dir = pY - mY;
        Vector3 cross = Vector3.Cross(transform.forward, dir.normalized);
        float dot = Vector3.Dot(transform.forward, dir.normalized);
        if (cross.y > 0.1f)
        {
            _myTransform.Rotate(Vector3.up, Time.deltaTime * _lookSpeed);
        }
        else if (cross.y < -0.1f)
        {
            _myTransform.Rotate(Vector3.down, Time.deltaTime * _lookSpeed);
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
            DoSpawn();
    }

    private void DoSpawn()
    {
        OnAttackComplete();
        if (CurZhiZhuNum >= MaxZhiZhuNum)
            return;

        Instantiate(_zhiZhuPref, _myTransform.position, Quaternion.identity);
        CurZhiZhuNum++;
    }

    private IEnumerator CoCompleteActive()
    {
        yield return new WaitForSeconds(_activeAnimDuration);

        _nextAtkTime = Time.time;
        _curState = BossState.IDEL;
        _uiCtr.EnableHpSlider(true);
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
