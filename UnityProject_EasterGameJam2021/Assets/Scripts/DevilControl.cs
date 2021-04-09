using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class DevilControl : MonoBehaviour {

    public int playerID = 1;
    public int controllerID = 1;

    public Material matDevil;


    public Text txtInfo;

    public Animator anim;

    void Update () {
		//if( Input.GetButtonDown("Right_P" + controllerID))
  //      {
  //          iTex = iTex+1 >= texs.Length ? 0 : iTex+1;
  //          matDevil.SetTexture("_MainTex", texs[iTex]);
  //      }
        /*else if (Input.GetButtonDown("Left_P" + controllerID))
        {
            iTex = iTex-1 < 0 ? texs.Length-1 : iTex-1;
            matDevil.SetTexture("_MainTex", texs[iTex]);
        }*/
    }

    public void Active(int ctrl)
    {
        this.enabled = true;
        controllerID = ctrl;

        txtInfo.text = "Press 'B' to back out";

        anim.SetInteger("state", 1);
    }

    public void DeActive()
    {
        this.enabled = false;
        //iTex = 0;

        txtInfo.text = "Press 'A' to join";

        anim.SetInteger("state", 3);
    }

    public void StartGame()
    {
        if( this.enabled)
        {
            anim.SetInteger("state", 2);
        }
    }
}
