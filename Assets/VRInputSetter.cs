using UnityEngine;
using System.Collections;

public class VRInputSetter : MonoBehaviour 
{
	private enum STEP
	{
		READY, ATK, DEF, SA, COMPLETE
	}

	public bool IsInputOk { get; private set;}
	private STEP _curStep;
	private bool _isInConfirm;

	private KeyCode _curKey;
	private Event _ev;
	private string _curKeyCodeString;
	private string _recordKeyCodeString;

	private void Awake()
	{
		IsInputOk = PlayerPrefs.GetInt (Consts.IsInputOkKey, 0) != 0;
		if (IsInputOk) 
		{
			SetKeysRuntime ();
		}
		_curStep = STEP.READY;
		_isInConfirm = false;
	}

	public void SetInputKey()
	{
		IsInputOk = false;
		_curKeyCodeString = _recordKeyCodeString = string.Empty;
		_curStep = STEP.ATK;
		_isInConfirm = false;
		UIViewCtr.Instance.DisplayMsg ("请按下一个键做为【普通攻击】");
		UIViewCtr.Instance.InputUIView.DisableSkip ();
		UIViewCtr.Instance.InputUIView.AddListener (Skip);
	}

	private void Update()
	{
		if (IsInputOk)
			return;
	
		if (!_isInConfirm) {
			switch (_curStep) {
			case STEP.READY:
				break;
			case STEP.ATK:
			case STEP.DEF:
			case STEP.SA:
				if (!string.IsNullOrEmpty(_curKeyCodeString) 
					&& !_curKeyCodeString.Equals(Consts.InputHorizontal) 
					&& !_curKeyCodeString.Equals(Consts.InputVertical))
					StartConfirm ();
				break;
			case STEP.COMPLETE:
				SetKeysRuntime ();
				IsInputOk = true;
				Debug.Log ("set ok");
				UIViewCtr.Instance.DisplayMsg ("设置完毕，游戏开始！");
				UIViewCtr.Instance.ClearMsgIn (3);
				break;
			}
		} else {
			Confirm ();
		}

	}

	private void OnGUI()
	{
		if (IsInputOk)
			return;

		if (Input.anyKey)
		{
			_ev = Event.current;
			if (_ev.isKey)
			{
				_curKey = _ev.keyCode;
				if (_curKey.ToString() != "None")
					_curKeyCodeString = _curKey.ToString ();
			}
		}
		GUILayout.Label("MSG:" + _curKeyCodeString);
	}

	private void StartConfirm()
	{
		_isInConfirm = true;
		_recordKeyCodeString = _curKeyCodeString;
		_curKeyCodeString = string.Empty;
		Debug.Log ("insert last key to confirm! or escap to reset");
		UIViewCtr.Instance.DisplayMsg ("请再次按下该键确认");
	}

	private void Confirm()
	{
		if (!_curKeyCodeString.Equals (_recordKeyCodeString))
			return;

		string key = string.Empty;
		string nextTip = string.Empty;
		switch (_curStep) 
		{
		case STEP.ATK:
			key = Consts.InputAttackKey;
			nextTip = "防御";
			break;
		case STEP.DEF:
			key = Consts.InputDefenseKey;
			nextTip = "必杀技";
			UIViewCtr.Instance.InputUIView.EnableSkip ();
			break;
		case STEP.SA:
			key = Consts.InputSpecialAttackKey;
			UIViewCtr.Instance.InputUIView.EnableSkip ();
			break;
		}
		PlayerPrefs.SetString (key, _recordKeyCodeString);

		_curStep = (STEP)((int)_curStep + 1);
		_curKeyCodeString = _recordKeyCodeString = string.Empty;
		_isInConfirm = false;
		UIViewCtr.Instance.DisplayMsg (string.Format("请按下一个键做为【{0}】", nextTip));
	}

	private void Skip()
	{
		if(_curStep == STEP.ATK)
			Debug.LogError ("Can not skip atk");

		var key = string.Empty;
		var nextTip = string.Empty;
		switch (_curStep) 
		{
		case STEP.ATK:
			Debug.LogError ("Can not skip atk");
			break;
		case STEP.DEF:
			key = Consts.InputDefenseKey;
			nextTip = "必杀技";
			break;
		case STEP.SA:
			key = Consts.InputSpecialAttackKey;
			UIViewCtr.Instance.InputUIView.DisableSkip ();
			break;
		}
		PlayerPrefs.SetString (key, string.Empty);

		_curStep = (STEP)((int)_curStep + 1);
		_curKeyCodeString = _recordKeyCodeString = string.Empty;
		_isInConfirm = false;
		UIViewCtr.Instance.DisplayMsg (string.Format("请按下一个键做为【{0}】", nextTip));
	}

	private void SetKeysRuntime()
	{
		Consts.InputAttackKeyCode = PlayerPrefs.GetString (Consts.InputAttackKey, string.Empty);
		Consts.InputDefenseKeyCode = PlayerPrefs.GetString (Consts.InputDefenseKey, string.Empty);
		Consts.InputSpecialAttackKeyCode = PlayerPrefs.GetString (Consts.InputSpecialAttackKey, string.Empty);
		Debug.Log(string.Format("atk is {0},,def is {1},,SA is {2}", Consts.InputAttackKeyCode,
			Consts.InputDefenseKeyCode, Consts.InputSpecialAttackKeyCode));
	}

}
