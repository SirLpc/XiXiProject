using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class InputUIView : MonoBehaviour 
{
	[SerializeField]
	private Button _skipBtn;

	public void DisableSkip()
	{
		_skipBtn.gameObject.SetActive (false);
	}

	public void EnableSkip()
	{
		//_skipBtn.gameObject.SetActive (true);
	}

	public void AddListener(Action onclick)
	{
		_skipBtn.onClick.RemoveAllListeners ();
		_skipBtn.onClick.AddListener (()=>onclick());
	}

}
