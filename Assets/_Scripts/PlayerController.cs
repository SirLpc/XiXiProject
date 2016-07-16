using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _defenseGap = 4f;
    [SerializeField] private GameObject bulletBase;

    public static PlayerController Instance = null;
    public vp_FPPlayerDamageHandler DamangeHandler { get; set; }
    public bool IsInDefense { get; private set; }

    private HandsomegunProperty _handsomegunProperty;
    private float _lastDefenseTime;


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
        if (_handsomegunProperty == null)
            return;

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
}
