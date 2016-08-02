using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TempUIView : MonoBehaviour 
{

	[SerializeField]
	private Image saFiller, defFiller;

	private GameObject saGo, defGo;

	void Awake()
	{
		saGo = saFiller.transform.parent.gameObject;
		defGo = defFiller.transform.parent.gameObject;
	}

	void Update ()
	{
		var saPer = PlayerController.Instance.GetSACdPercent ();
		var defPer = PlayerController.Instance.GetDefCdPercent ();
		if (saPer >= 1) 
		{
			if (saGo.gameObject.activeSelf)
				saGo.gameObject.SetActive (false);
		} 
		else
		{
			if (!saGo.gameObject.activeSelf)
				saGo.gameObject.SetActive (true);
			saFiller.fillAmount = saPer;
		}
		if (defPer >= 1) 
		{
			if (defGo.gameObject.activeSelf)
				defGo.gameObject.SetActive (false);
		} 
		else 
		{
			if (!defGo.gameObject.activeSelf)
				defGo.gameObject.SetActive (true);
			defFiller.fillAmount = defPer;
		}

	}

}
