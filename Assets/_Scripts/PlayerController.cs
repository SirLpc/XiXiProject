using UnityEngine;
using System.Collections;
using System.Xml;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _defenseGap = 4f;
    [SerializeField] private GameObject bulletBase;
    [SerializeField] private float _specialAttackBearGap = 1;
    [SerializeField] private float _specialAttackGap = 7;
    [SerializeField] private float _specialAttackEffectiveTime = 2;
    [SerializeField] private CharacterController _characterController;

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
            counter += Time.deltaTime;
            if (!vp_FPInput.DetectCancelInputSpecialAttack())
            {
                yield return null;
            }
            else
            {
                IsInSpecialAttack = false;
                _handsomegunProperty.StopAnimation();
                yield break;
            }
        }

        _handsomegunProperty.Shooter.SpecialAttackFire();

        _characterController.Move(Vector3.right*3f);

        //不能马上结束，要等玩家抬起鼠标后才行，不然很别扭
        while (vp_FPInput.DetectCancelInputSpecialAttack())
        {
            yield return null;
        }
        StartCoroutine(CoClearSpecialAttack());
    }

    private IEnumerator CoClearSpecialAttack()
    {
        yield return new WaitForSeconds(1.8f);

        IsInSpecialAttack = false;
    }

}
