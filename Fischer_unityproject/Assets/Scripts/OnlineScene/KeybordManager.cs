﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using TMPro;

public class KeybordManager : MonoBehaviour {

	public GameObject textMain; //メインテキスト
	public GameObject textNumber; //数字テキスト
	public GameObject userCard; 
	public GameObject imageBackground; //背景
	public Sprite[] cardSprite = new Sprite[10]; //カードの画像
	private DatabaseReference reference;
	private DatabaseReference refRoom; //部屋までのref
	private string roomnum_str = "";

	private string callnum = "";
	private int count = 0; //今何番目の数字か

	private void Start(){
		/*
		 * Firebase関連
		 */
		// Set up the Editor before calling into the realtime database.
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://fischer-66e26.firebaseio.com");
		// Get the root reference location of the database.
		reference = FirebaseDatabase.DefaultInstance.RootReference;
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
  		var dependencyStatus = task.Result;
  		if (dependencyStatus == Firebase.DependencyStatus.Available) {
		    // Create and hold a reference to your FirebaseApp, i.e.
		    //   app = Firebase.FirebaseApp.DefaultInstance;
		    // where app is a Firebase.FirebaseApp property of your application class.

		    // Set a flag here indicating that Firebase is ready to use by your
		    // application.
		  } else {
		    UnityEngine.Debug.LogError(System.String.Format(
		      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
		    // Firebase Unity SDK is not safe to use here.
		  }
		});
	}

	public void GetRef(string room){
		roomnum_str = room;
	}

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
		DecideSettingNum();
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

	//設定ナンバー決定
	private void DecideSettingNum(){
		Dictionary<string,object> number = new Dictionary<string, object>(){{"number",callnum}};
		reference.Child("online").Child("room").Child(roomnum_str).
			Child(PlayerPrefs.GetString("userid")).UpdateChildrenAsync(number);
		
		FirebaseDatabase.DefaultInstance
			.GetReference("online")
			.GetValueAsync().ContinueWith(task => {
				if (task.IsFaulted) {
				// Handle the error...
				}
				else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					// Do something with snapshot...
					if(snapshot.Child("room").Child(roomnum_str).
						Child(PlayerPrefs.GetString("enemyid")).Child("number").Value == null){
						//相手はまだ入力中
						WaitEnemy();
					}else{
						//相手の入力が終了している
						GameStart();
					}
				}
			});
	}

	private void WaitEnemy(){
		textMain.SetActive(true);
		textNumber.SetActive(false);
		textMain.GetComponent<Text>().text = "対戦相手が決めるまでもう少しお待ちください";
		reference.Child("online").Child("room").Child(roomnum_str).
			Child(PlayerPrefs.GetString("enemyid")).ChildAdded += HandleChildAdded;
	}

	void HandleChildAdded(object sender, ChildChangedEventArgs args) {
      if (args.DatabaseError != null) {
        Debug.LogError(args.DatabaseError.Message);
        return;
      }
      // Do something with the data in args.Snapshot
	  Debug.Log(args.Snapshot);
	  if(args.Snapshot.Key == "number"){
		  //敵のナンバー決定終了
		  GameStart();
	  }

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
