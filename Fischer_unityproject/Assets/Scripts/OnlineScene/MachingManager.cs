using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class MachingManager : MonoBehaviour {

	private GameObject indicator;
	private DatabaseReference reference;
	private bool isLastRoom = true;

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

		//オブジェクトの参照
		referenceObject();

		//インジケーターの表示
		indicator.GetComponent<ActivityIndicator>().Show();

		//マッチング開始
		searchRoom();
		
	}

	//オブジェクトの参照
	private void referenceObject(){
		indicator = GameObject.Find("ActivityIndicator");
	}

	//部屋の検索
	private void searchRoom(){
		//部屋の中に一人しかいない時OK
		FirebaseDatabase.DefaultInstance
		.GetReference("onlineroom")
		.GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted) {
				
			}
			else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				IEnumerator<DataSnapshot> en = snapshot.Children.GetEnumerator();;

				while (en.MoveNext()) {
					DataSnapshot data = en.Current;
					/* 
					string name = (string)data.Child("name").Value;
					int score   = (int)(long)data.Child("score").Value;
					*/
					if(data.ChildrenCount == 1){
						//部屋に一人しかいない
						intoTheRoom(int.Parse(data.Key));
						Debug.Log(data.Key + "の部屋に参加");
						isLastRoom = false;
						break;
					}
				}

				//空いてる部屋がない
				if(isLastRoom){
					Debug.Log("部屋を作成");
					//部屋の作成
					createRoom();
				}


			}
		});
	}

	//部屋に入る
	private void intoTheRoom(int roomnum){
		//自分の情報をjson形式で保存
		string json = "{\"name\":\"challenger\",\"rate\":1500}";
		//部屋に自分の情報を反映
		reference.Child("onlineroom").Child(roomnum.ToString()).Child("challengerid").SetRawJsonValueAsync(json);

		//ゲームスタート
		gameStart();
	}

	//部屋を作成
	private void createRoom(){
		int roomnum = PlayerPrefs.GetInt("roomnum") + 1;
		PlayerPrefs.SetInt("roomnum",roomnum);

		//自分の情報をjson形式で保存
		string json = "{\"host\":\"hostman\",\"rate\":1500}";
		//部屋に自分の情報を反映
		reference.Child("onlineroom").Child(roomnum.ToString()).Child("hostid").SetRawJsonValueAsync(json);
	}

	//ゲームスタート
	private void gameStart(){

	}
}
