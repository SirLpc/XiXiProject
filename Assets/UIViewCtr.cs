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

	public void ClearMsgIn(float seconds)
	{
		StartCoroutine (CoClearMsg (seconds));
	}

	private IEnumerator CoClearMsg(float seconds)
	{
		yield return new WaitForSeconds (seconds);
		_msgText.text = string.Empty;
	}
}
