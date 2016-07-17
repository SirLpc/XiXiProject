using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestScript : MonoBehaviour {
    Text txt;
	// Use this for initialization
	void Start () {
        txt = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {

        //if (Input.GetButtonDown("Fire1"))
        //    txt.text = "fiiiir1111111";
        //if (Input.GetButtonDown("Fire2"))
        //    txt.text = "fiiiir22222222";
        //if (Input.GetButtonDown("Fire3"))
        //    txt.text = "fiiiir3333333";
        //if (Input.GetButtonDown("Jump"))
        //    txt.text = "JumpJumpJumpJump";
        //if (Input.GetButtonDown("Submit"))
        //    txt.text = "SubmitSubmitSubmit";
        //if (Input.GetButtonDown("Cancel"))
        //    txt.text = "CancelCancelCancelCancel";
        //if (Input.GetMouseButton(0))
        //    txt.text = "mmbt0";
        var n = Input.GetJoystickNames();
        var str = "--";
        foreach (var item in n)
        {
            str += item + "--";
        }
        txt.text = str;
        

        var axiH = Input.GetAxis("Horizontal");
        if (axiH != 0) txt.text = "HHHH";
        var axiY = Input.GetAxis("Vertical");
        if (axiY != 0) txt.text = "VVV";

    }
}
