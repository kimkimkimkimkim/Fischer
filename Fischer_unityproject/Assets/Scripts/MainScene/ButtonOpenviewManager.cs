using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOpenviewManager : MonoBehaviour {
	public GameObject otherView; //非表示にするView
	public GameObject myView; //表示するView

	public void OnClick(){
		myView.SetActive(true); //Viewを表示

		if(otherView == null)return; //非表示にするViewがなければ終了
		otherView.SetActive(false);
	}
}
