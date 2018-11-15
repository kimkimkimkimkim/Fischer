using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonKeypad : MonoBehaviour {

	private GameObject gameView; //ゲームビュー
	private GameObject text; //自分のテキスト

	private void Start(){
		gameView = transform.parent.parent.parent.gameObject;
		text = transform.GetChild(0).gameObject;
	}

	public void OnClick(){
		if(text.GetComponent<Text>().text=="call"){
			//コールボタンをクリック
			gameView.GetComponent<GameManagerSingle>().ClickCall();
		}else if(text.GetComponent<Text>().text=="reset"){
			//リセットボタンをクリック
			gameView.GetComponent<GameManagerSingle>().ClickReset();
		}else{
			//数字ボタンをクリック
			gameView.GetComponent<GameManagerSingle>().ClickNum(text.GetComponent<Text>().text);
		}
	}

}
