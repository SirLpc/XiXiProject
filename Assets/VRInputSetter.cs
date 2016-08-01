using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VRInputSetter : MonoBehaviour
{
    private enum STEP
    {
        READY, ATK, DEF, SA, COMPLETE
    }

    public bool IsInputOk { get; private set; }
    private STEP _curStep;
    private bool _isInConfirm;

    private KeyCode _curKey;
    private Event _ev;
    private string _curKeyCodeString;
    private string _recordKeyCodeString;
    private Dictionary<STEP, string> _recordKeys;

    private void Awake()
    {
        IsInputOk = PlayerPrefs.GetInt(Consts.IsInputOkKey, 0) != 0;
        if (IsInputOk)
        {
            SetKeysRuntime();
        }
        _curStep = STEP.READY;
        _isInConfirm = false;
        _recordKeys = new Dictionary<STEP, string>(3)
        {
            {STEP.ATK, string.Empty},
            {STEP.DEF, string.Empty},
            {STEP.SA, string.Empty}
        };
    }

    public void SetInputKey()
    {
        IsInputOk = false;
        _curKeyCodeString = _recordKeyCodeString = string.Empty;
        _curStep = STEP.ATK;
        _isInConfirm = false;
        UIViewCtr.Instance.DisplayMsg("请按下一个键做为[普通攻击]");
    }

    private void Update()
    {
        if (IsInputOk)
            return;

        if (!_isInConfirm)
        {
            switch (_curStep)
            {
                case STEP.READY:
                    break;
                case STEP.ATK:
                    if (IsKeyValid())
                        StartConfirm();
                    break;
                case STEP.DEF:
                case STEP.SA:
                    if (IsKeyValid())
                    {
                        if (!_curKeyCodeString.Equals(_recordKeys[STEP.ATK]))
                        {
                            if (_recordKeys.ContainsValue(_curKeyCodeString))
                            {
                                UIViewCtr.Instance.TempTipMsg("该键已被占用", 2);
                                _recordKeyCodeString = _curKeyCodeString = string.Empty;
                            }
                            else
                            {
                                StartConfirm();
                            }
                        }
                        else
                        {
                            Skip();
                        }
                    }
                    break;
                case STEP.COMPLETE:
                    SetKeysRuntime();
                    IsInputOk = true;
                    Debug.Log("set ok");
                    UIViewCtr.Instance.DisplayMsg("设置完毕，游戏开始！");
                    UIViewCtr.Instance.ClearMsgIn(3);
                    break;
            }
        }
        else
        {
            if(IsKeyValid())
                Confirm();
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
                    _curKeyCodeString = _curKey.ToString();
            }
        }
        GUILayout.Label("MSG:" + _curKeyCodeString);
    }

    private bool IsKeyValid()
    {
        return !string.IsNullOrEmpty(_curKeyCodeString)
               && !_curKeyCodeString.Equals(Consts.InputHorizontal)
               && !_curKeyCodeString.Equals(Consts.InputVertical);
    }

    private void StartConfirm()
    {
        _isInConfirm = true;
        _recordKeyCodeString = _curKeyCodeString;
        _curKeyCodeString = string.Empty;
        Debug.Log("insert last key to confirm! or escap to reset");
        UIViewCtr.Instance.DisplayMsg("请再次按下该键确认");
    }

    private void Confirm()
    {
        if (!_curKeyCodeString.Equals(_recordKeyCodeString))
        {
            _isInConfirm = false;
            _curKeyCodeString = _recordKeyCodeString = string.Empty;
            var tipTemp = string.Empty;
            switch (_curStep)
            {
                case STEP.ATK:
                    tipTemp = "普通攻击";
                    break;
                case STEP.DEF:
                    tipTemp = "防御";
                    break;
                case STEP.SA:
                    tipTemp = "必杀技";
                    break;
            }
            UIViewCtr.Instance.DisplayMsg(string.Format("不一致的输入,请重新设置[{0}]键", tipTemp));
            return;
        }

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
                break;
            case STEP.SA:
                key = Consts.InputSpecialAttackKey;
                break;
        }
        _recordKeys[_curStep] = _recordKeyCodeString;
        PlayerPrefs.SetString(key, _recordKeyCodeString);

        _curStep = (STEP) ((int) _curStep + 1);
        _curKeyCodeString = _recordKeyCodeString = string.Empty;
        _isInConfirm = false;
        UIViewCtr.Instance.DisplayMsg(string.Format("请按下一个键做为[{0}]\n(可按刚才设置的[攻击键]跳过)", nextTip));
    }

    private void Skip()
    {
        var key = string.Empty;
        var nextTip = string.Empty;
        switch (_curStep)
        {
            case STEP.ATK:
                Debug.LogError("Can not skip atk");
                break;
            case STEP.DEF:
                key = Consts.InputDefenseKey;
                nextTip = "必杀技";
                break;
            case STEP.SA:
                key = Consts.InputSpecialAttackKey;
                break;
        }
        PlayerPrefs.SetString(key, string.Empty);

        _curStep = (STEP) ((int) _curStep + 1);
        _curKeyCodeString = _recordKeyCodeString = string.Empty;
        _isInConfirm = false;
        UIViewCtr.Instance.DisplayMsg(string.Format("请按下一个键做为[{0}]\n(可按刚才设置的攻击键跳过)", nextTip));
        if(_curStep == STEP.SA)
            UIViewCtr.Instance.TempTipMsg(string.Format("[{0}]键设置被跳过", "防御"), 2f);
    }

    private void SetKeysRuntime()
    {
        Consts.InputAttackKeyCode = PlayerPrefs.GetString(Consts.InputAttackKey, string.Empty);
        Consts.InputDefenseKeyCode = PlayerPrefs.GetString(Consts.InputDefenseKey, string.Empty);
        Consts.InputSpecialAttackKeyCode = PlayerPrefs.GetString(Consts.InputSpecialAttackKey, string.Empty);
        Debug.Log(string.Format("atk is {0},,def is {1},,SA is {2}", Consts.InputAttackKeyCode, Consts.InputDefenseKeyCode, Consts.InputSpecialAttackKeyCode));
    }
}
