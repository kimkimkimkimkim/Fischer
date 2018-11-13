using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeybordManager : MonoBehaviour {

	public GameObject textMain; //メインテキスト
	public GameObject textNumber; //数字テキスト
	public GameObject userCard; 
	public GameObject imageBackground; //背景
	public Sprite[] cardSprite = new Sprite[10]; //カードの画像

	private string callnum = "";
	private int count = 0; //今何番目の数字か

	public void OnClick(int num){
		if(num==10){
			Call();
			return;
		}
		if(num==11){
			Reset();
			return;
		}
		//数字だった場合
		if(count>=3)return;
		callnum += num.ToString();
		count++;
		Render();
	}

	private void Call(){
		if(count!=3)return;
		//コールの処理
		GameStart();
	}

	private void Reset(){
		if(count==0)return;
		count--;
		callnum = callnum.Remove(count);
		Render();
	}


	private void Render(){
		textMain.SetActive(false);
		textNumber.SetActive(true);
		textNumber.GetComponent<TextMeshProUGUI>().text = callnum;
	}	

	private void GameStart(){
		for(int i=0;i<3;i++){
			GameObject card = userCard.transform.GetChild(i).gameObject;
			char c = callnum[i];
			int num = int.Parse(c.ToString());
			card.GetComponent<SpriteRenderer>().sprite = cardSprite[num];
		}
		iTween.ValueTo(imageBackground,iTween.Hash("from",1,"to",0,"onupdate","UpdateColor","time",1.5,
			"onupdatetarget",gameObject,"oncomplete","Complete","oncompletetarget",gameObject,
			"EaseType",iTween.EaseType.easeInQuint));
	}

	private void UpdateColor(float alfa){
		Color color = imageBackground.GetComponent<Image>().color;
		imageBackground.GetComponent<Image>().color = new Color(color.r,color.g,color.b,alfa);
	}

	private void Complete(){
		imageBackground.SetActive(false);
		textNumber.SetActive(false);
		textMain.SetActive(true);
		textMain.GetComponent<Text>().text = "Game Start!";
	}
}
