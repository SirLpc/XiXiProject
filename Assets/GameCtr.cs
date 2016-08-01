﻿using UnityEngine;
using System.Collections;

public class GameCtr : MonoBehaviour 
{

	private VRInputSetter _inputSetter;

	private void Awake()
	{
		_inputSetter = GetComponent<VRInputSetter> ();
	}

	private IEnumerator Start () 
	{
		if (!_inputSetter.IsInputOk)
		{
			UIViewCtr.Instance.DisplayMsg ("即将为您初始化按键");
			yield return new WaitForSeconds (3);
			_inputSetter.SetInputKey ();
		}
		while (!_inputSetter.IsInputOk)
		{
			yield return null;
		}
		PlayerController.Instance.StartControl ();
	}
	

}