using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIViewCtr : MonoBehaviour
{
	public static UIViewCtr Instance = null;

    private float _fadeOutSpeed = 2f;

	[SerializeField]
	private Text _msgText;

    [SerializeField]
    private Image _hurtImg;
    [SerializeField]
    private Image _maskImg;

    [SerializeField] private GameObject _iconImg;

	public InputUIView InputUIView;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		_msgText.text = string.Empty;
	}

	public void DisplayMsg(string msg)
	{
		_msgText.text = msg;
	}

    public void TempTipMsg(string msg, float seconds)
    {
        var oldMsg = _msgText.text;
        _msgText.text = msg;
        StartCoroutine(CoTempTipMsg(oldMsg, seconds));
    }

    public void DisplayHurt()
    {
        _hurtImg.enabled = true;
        StartCoroutine(CoClearHurtImg());
    }

    private IEnumerator CoTempTipMsg(string oldMsg, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _msgText.text = oldMsg;
    }

    private IEnumerator CoClearHurtImg()
    {
        yield return new WaitForSeconds(.5f);
        _hurtImg.enabled = false;
    }

	public void ClearMsgIn(float seconds)
	{
        StopAllCoroutines();
        StartCoroutine (CoClearMsg (seconds));
	}

	private IEnumerator CoClearMsg(float seconds)
	{
		yield return new WaitForSeconds (seconds);
		_msgText.text = string.Empty;
	}

    public void FadeOut(Action after)
    {
        _maskImg.color = new Color(0, 0, 0, 0);
        _maskImg.gameObject.SetActive(true);
        StartCoroutine(CoFadeOut(after));
    }

    private IEnumerator CoFadeOut(Action after)
    {
        while (_maskImg.color.a < .98f)
        {
            yield return null;
            var c = Color.Lerp(_maskImg.color, new Color(0, 0, 0, 1), _fadeOutSpeed * Time.deltaTime);
            _maskImg.color = c;
        }
        if(after != null)
            after.Invoke();
    }

    public void FadeIn(Action after)
    {
        _maskImg.color = new Color(0, 0, 0, 1);
        _maskImg.gameObject.SetActive(true);
        StartCoroutine(CoFadeIn(after));
    }

    private IEnumerator CoFadeIn(Action after)
    {
        while (_maskImg.color.a > .02f)
        {
            yield return null;
            var c = Color.Lerp(_maskImg.color, new Color(0, 0, 0, 0), _fadeOutSpeed * Time.deltaTime);
            _maskImg.color = c;
        }
        _maskImg.gameObject.SetActive(false);
        if (after != null)
            after.Invoke();
    }

    public void ShowIcon(bool show = true)
    {
        _iconImg.SetActive(show);
    }
}
