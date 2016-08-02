using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIViewCtr : MonoBehaviour
{
	public static UIViewCtr Instance = null;

	[SerializeField]
	private Text _msgText;

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

    private IEnumerator CoTempTipMsg(string oldMsg, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _msgText.text = oldMsg;
    }

	public void ClearMsgIn(float seconds)
	{
		StartCoroutine (CoClearMsg (seconds));
	}

	private IEnumerator CoClearMsg(float seconds)
	{
		yield return new WaitForSeconds (seconds);
		_msgText.text = string.Empty;
		StopAllCoroutines ();
	}
}
