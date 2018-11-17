using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class TimerManager : MonoBehaviour {

	public GameObject keybordManager;

	public bool isStart = false;
	private float t = 0;
	private int methodNum = 0;

	private void FixedUpdate(){
		if(isStart){
			t -= Time.deltaTime;
			UpdateSlider();
		}
	}

	public void SetTimer(float time,int n){
		isStart = true;
		GetComponent<Slider>().maxValue = time;
		t = time;
		methodNum = n;
	}

	private void UpdateSlider(){
		if(t<0){
			isStart = false;
			switch(methodNum){
			case 1:
				SetNumber();
				break;
			default:
				break;
			}
		}
		GetComponent<Slider>().value = t;
	}

	private void SetNumber(){
		string callnum = "";
		var ary = Enumerable.Range(0, 9).OrderBy(n => Guid.NewGuid()).Take(3).ToArray();
		callnum = ary[0].ToString() + ary[1].ToString() + ary[2].ToString();
		Debug.Log(callnum);
		keybordManager.GetComponent<KeybordManager>().DecideSettingNum(callnum);
	}


}
