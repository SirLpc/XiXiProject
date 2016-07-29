using UnityEngine;
using System.Collections;
using System.Xml;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _defenseGap = 4f;
    [SerializeField] private GameObject bulletBase;
    [SerializeField] private float _specialAttackGap = 7;
    [SerializeField] private float _specialAttackEffectiveTime = 2;

    public static PlayerController Instance = null;
    public vp_FPPlayerDamageHandler DamangeHandler { get; private set; }

    private HandsomegunProperty _handsomegunProperty;

    public bool IsInDefense { get; private set; }
    private float _lastDefenseTime;

    public bool SpecialAttackEffectived { get; private set; }
    private float _lastSpecialAttackTime;
    private Coroutine _saCoroutine;
    private Transform _transform;

    void Awake()
    {
        Application.targetFrameRate = 45;

        Instance = this;
        DamangeHandler = GetComponent<vp_FPPlayerDamageHandler>();
        _handsomegunProperty = GetComponentInChildren<HandsomegunProperty>();
        _transform = transform;
    }

    public void GiveBullets()
    {
        bulletBase.gameObject.SetActive(true);
    }

    public void ApplyHeadRotation(Quaternion rot, bool local)
    {
        if (local)
            _transform.localRotation = rot;
        else
            _transform.rotation = rot;
    }

    public void TryDefense()
    {
        if (_handsomegunProperty == null) return;

        if (IsInDefense) return;

        if (Time.time - _lastDefenseTime < _defenseGap)
            return;

        _lastDefenseTime = Time.time;
        _handsomegunProperty.PlayAnimation(_handsomegunProperty.DefenseClip.name, true);
        IsInDefense = true;
        StartCoroutine(CoClearDefense());
    }

    private IEnumerator CoClearDefense()
    {
        yield return new WaitForSeconds(Consts.AniDefenseHurtDuration);
        IsInDefense = false;
    }

    public void TrySpecialAttack()
    {
        if (_handsomegunProperty == null) return;

        if (_saCoroutine != null) return;

        if (Time.time - _lastSpecialAttackTime < _specialAttackGap)
            return;

        SpecialAttackEffectived = false;
        _lastSpecialAttackTime = Time.time;
        _handsomegunProperty.PlayAnimation(_handsomegunProperty.SpecailAttackClip.name);
        _saCoroutine = StartCoroutine(CoDetectSpecialAttack());
    }

    public void ShutSpecialAttack()
    {
        if (_saCoroutine == null)
            return;

        StopCoroutine(_saCoroutine);
        _saCoroutine = null;
    }

    private IEnumerator CoDetectSpecialAttack()
    {
        //var counter = 0f;
        //while (counter < _specialAttackEffectiveTime)
        //{
        //    counter += Time.deltaTime;
        //    if (!vp_FPInput.DetectCancelInputSpecialAttack())
        //    {
        //        yield return null;
        //    }
        //    else
        //    {
        //        IsInSpecialAttack = false;
        //        _handsomegunProperty.StopAnimation();
        //        yield break;
        //    }
        //}

        yield return new WaitForSeconds(_specialAttackEffectiveTime);

        SpecialAttackEffectived = true;

        _handsomegunProperty.Shooter.SpecialAttackFire();

        StartCoroutine(CoClearSpecialAttack());
    }

    private IEnumerator CoClearSpecialAttack()
    {
        yield return new WaitForSeconds(1.8f);

        SpecialAttackEffectived = false;
        _saCoroutine = null;
    }

}
