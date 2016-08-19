
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
    public vp_FPPlayerEventHandler EventHandler { get; private set; }
    public HandsomegunProperty HandsomegunProperty { get { return _handsomegunProperty; } }

    private HandsomegunProperty _handsomegunProperty;

    public bool IsInDefense { get; private set; }
    private float _lastDefenseTime;

    public bool SpecialAttackEffectived { get; private set; }
    private float _lastSpecialAttackTime;
    private Coroutine _saCoroutine;
    private Transform _transform;
    private CharacterController _characterCtr;

	private float _specialAttackGapUN, _defenseGapUN;

    void Awake()
    {
        Instance = this;
        DamangeHandler = GetComponent<vp_FPPlayerDamageHandler>();
        EventHandler = GetComponent<vp_FPPlayerEventHandler>();
        _handsomegunProperty = GetComponentInChildren<HandsomegunProperty>();
        _characterCtr = GetComponent<CharacterController>();
        _transform = transform;

		_specialAttackGapUN = 1 / _specialAttackGap;
		_defenseGapUN = 1 / _defenseGap;
    }

    public void GiveBullets()
    {
        bulletBase.gameObject.SetActive(true);
    }

    public void EnableControl(bool enable = true)
    {
		GetComponent<vp_FPInput> ().enabled = enable;
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

	public float GetSACdPercent()
	{
		return (Time.time - _lastSpecialAttackTime) * _specialAttackGapUN;
	}

	public float GetDefCdPercent()
	{
		return (Time.time - _lastDefenseTime) * _defenseGapUN;
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


    public void PlayEarthquake()
    {
        //m_Demo.Camera.Load(StompingCamera);
        //m_Demo.Controller.Load(SmackController);
        EventHandler.Earthquake.TryStart(new Vector3(0.2f, 0.2f, 10.0f));
        //m_Demo.ButtonColumnArrowFadeoutTime = Time.time + 9;
        //m_AudioSource.Stop();
        //m_AudioSource.pitch = Time.timeScale;
        //m_AudioSource.PlayOneShot(m_EarthquakeSound);
    }

    private IEnumerator CoDetectSpecialAttack()
    {
        yield return new WaitForSeconds(_specialAttackEffectiveTime);

        SpecialAttackEffectived = true;

        _handsomegunProperty.Shooter.SpecialAttackFire();

        StartCoroutine(CoClearSpecialAttack());
    }

    private IEnumerator CoClearSpecialAttack()
    {
        yield return new WaitForSeconds(1.0f);

        SpecialAttackEffectived = false;
        _saCoroutine = null;
    }

    public void ForceMove(Vector3 dir)
    {
        _characterCtr.Move(dir);
    }


}
