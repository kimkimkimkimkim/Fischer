using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCloseViewManager : MonoBehaviour {
	
	public GameObject otherView; //表示にするView
	public GameObject myView; //非表示するView

	public void OnClick(){
		myView.SetActive(false); //Viewを非表示

		otherView.SetActive(true); //Viewを表示
	}
}
