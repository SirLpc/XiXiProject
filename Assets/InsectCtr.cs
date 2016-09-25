using UnityEngine;
using System.Collections;

public class InsectCtr : ZhiZhuCtr
{
    private EnemySpawner _spawner;
    private float _defenseBackSpeed = 3f;


    protected override void Awake()
    {
        base.Awake();
        _spawner = GetComponentInParent<EnemySpawner>();
        _spawner.PlayerIn.AddListener(ActiveInsect);
        gameObject.SetActive(false);
    }

    private void ActiveInsect()
    {
        gameObject.SetActive(true);
    }

    protected override void TryAtk()
    {
        if (!_isInAttack)
        {
            _anim.SetBool(Consts.AniIsInAttack, true);
        }
        _isInAttack = true;

        if (Time.time < _lastAtkTime + _atkGap)
            return;
        _lastAtkTime = Time.time;

        if (_agent.isActiveAndEnabled)
            _agent.Stop();

        var ran = Random.Range(0, 3);
        _anim.SetTrigger(ran == 0 ? Consts.AniTriggerAttack2 : Consts.AniTriggerAttack);
    }

    public void ToSAHurt()
    {
        _isInSAHurt = true;
        _anim.SetTrigger(Consts.AniTriggerSAHurt);
        StartCoroutine(CoEndSAHurt());
    }

    private IEnumerator CoEndSAHurt()
    {
        yield return new WaitForSeconds(3f);
        _isInSAHurt = false;
    }

    public void DefenseDamage()
    {
        _isInDefenseHurt = true;
        _anim.SetTrigger(Consts.AniTriggerDefenseHurt);
        StartCoroutine(CoBeenDefenseBack());
    }

    private IEnumerator CoBeenDefenseBack()
    {
        var t = 0f;
        while (t < Consts.AniDefenseHurtDuration)
        {
            t += Time.deltaTime;
            yield return null;
            transform.Translate(Vector3.back * Time.deltaTime * _defenseBackSpeed);
        }

        _isInDefenseHurt = false;
    }

    public override void OnAttackComplete()
    {
        if (!PlayerController.Instance.IsInDefense)
        {
            base.OnAttackComplete();
        }
        else
        {
            DefenseDamage();
        }
    }

    public void SetDie()
    {
        _isAlive = false;
        _agent.Stop();
        _agent.enabled = false;
        _anim.SetTrigger(Consts.AniTriggerDie);

        Destroy(_spawner.gameObject, 5f);
    }

}
