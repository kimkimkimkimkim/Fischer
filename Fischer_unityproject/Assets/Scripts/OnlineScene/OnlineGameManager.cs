using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using TMPro;

public class OnlineGameManager : MonoBehaviour {

	public GameObject textMain; //メインテキスト
	public GameObject textNumber; //数字テキスト
	public GameObject textResult; //eatbiteの結果を表示するテキスト
	public GameObject keybord; //キーボード

	private DatabaseReference reference;

	/*	PlayerPrefs
	 {
		 "userid", string
		 "enemyid", string
		 "roomnum_str", string
		 "myturn", int
		 "gamestart", int
		 "usernum", string
		 "enemynum", string
	 }
	 */

	// Use this for initialization
	void Start () {
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

	public void GameStart(){
		PlayerPrefs.SetInt("gamestart",1);
		//先攻後攻を決める
		if(PlayerPrefs.GetInt("myturn") == 1){
			Debug.Log("先攻");
			//先攻
			Offense();
		}else{
			Debug.Log("後攻");
			//後攻
			Defense();
		}
	}
	
	private void Offense(){
		textMain.SetActive(true);
		textNumber.SetActive(false);
		textResult.SetActive(false);
		textMain.GetComponent<Text>().text = "あなたのターンです";

	}

	private void Defense(){
		textMain.SetActive(true);
		textNumber.SetActive(false);
		textResult.SetActive(false);
		textMain.GetComponent<Text>().text = "相手のターンです";

	}

	public void CallNum(string callnum){

	}

	public void JudgeEatBite(string callnum){
		int eat=0,bite=0;
		string enemynum = PlayerPrefs.GetString("enemynum");

		for(int i=0;i<3;i++){
			for(int j=0;j<3;j++){
				if(enemynum[i] == callnum[j]){
					if(i==j){
						eat++;
					}else{
						bite++;
					}
				}
			}
		}

		Debug.Log("enemynum:" + enemynum + " callnum:" + callnum);
		Debug.Log("eat:" + eat + " bite:" + bite);

	}

}
