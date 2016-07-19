using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _defenseGap = 4f;
    [SerializeField] private GameObject bulletBase;
    [SerializeField] private float _specialAttackBearGap = 1;
    [SerializeField] private float _specialAttackGap = 7;
    [SerializeField] private float _specialAttackEffectiveTime = 2;

    public static PlayerController Instance = null;
    public vp_FPPlayerDamageHandler DamangeHandler { get; set; }

    private HandsomegunProperty _handsomegunProperty;

    public bool IsInDefense { get; private set; }
    private float _lastDefenseTime;

    public bool IsInSpecialAttack { get; private set; }
    public float SpecialAttackBearFrame { get { return _specialAttackBearGap; } }
    private float _lastSpecialAttackTime;

    void Awake()
    {
        Instance = this;
        DamangeHandler = GetComponent<vp_FPPlayerDamageHandler>();
        _handsomegunProperty = GetComponentInChildren<HandsomegunProperty>();
    }

    public void GiveBullets()
    {
        bulletBase.gameObject.SetActive(true);
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

        if (IsInSpecialAttack) return;

        if (Time.time - _lastSpecialAttackTime < _specialAttackGap)
            return;

        IsInSpecialAttack = true;
        _lastSpecialAttackTime = Time.time;
        _handsomegunProperty.PlayAnimation(_handsomegunProperty.SpecailAttackClip.name);
        StartCoroutine(CoDetectSpecialAttack());
    }

    private IEnumerator CoDetectSpecialAttack()
    {
        var counter = 0f;
        while (counter < _specialAttackEffectiveTime)
        {
            if (vp_FPInput.DetectCancelInputSpecialAttack())
            {
                IsInSpecialAttack = false;
                yield break;
            }
            else
                yield return null;
        }
        Debug.Log("attack ok!");
        IsInSpecialAttack = false;
    }

}
