using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

public class FirebaseManager : MonoBehaviour {

	DatabaseReference reference;

	public GameObject rankingSpace;

	void Start() {
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

	public class User {
    public string username;
    public int score;

    public User() {
    }

    public User(string username, int score) {
        this.username = username;
        this.score = score;
    }
	}

	public void writeNewScore(string userId, string name, int score) {
    	User user = new User(name,score);
    	string json = JsonUtility.ToJson(user);
    	reference.Child("ranking").Child(userId).SetRawJsonValueAsync(json);
	}

	public void getRanking(){
		//RealtileDataBaseから現在のランキングを取得
		//bossNoのノードからtimeで昇順ソートして最大10件を取る（非同期)
		reference.Child("ranking").OrderByChild("score").GetValueAsync().ContinueWith(task =>{
  		if(task.IsFaulted){ //取得失敗
    	//Handle the Error
			Debug.Log("error");
	  	}else if(task.IsCompleted){ //取得成功
				Debug.Log("取得成功");
		    DataSnapshot snapshot = task.Result; //結果取得
				//Debug.Log(snapshot.ToString());
		    IEnumerator<DataSnapshot> en = snapshot.Children.GetEnumerator(); //結果リストをenumeratorで処理
		    int rank = 1;
				bool flag = false;
				string userid = PlayerPrefs.GetString("userid");
		    while(en.MoveNext()){ //１件ずつ処理
					DataSnapshot data = en.Current; //データ取る
					string name = data.Child("username").GetValue(true).ToString(); //名前取る
					string score = data.Child("score").GetValue(true).ToString().Substring(1); //スコアを取る
					if(score=="")score = "0";
					string id = data.Key.ToString(); //useridを取る
					if(rank<=10){
						//Textに反映
						GameObject row = rankingSpace.transform.GetChild(rank).gameObject;
						row.transform.GetChild(0).gameObject.GetComponent<Text>().text = (rank).ToString(); //順位
						row.transform.GetChild(1).gameObject.GetComponent<Text>().text = score; //スコア
						row.transform.GetChild(2).gameObject.GetComponent<Text>().text = name; //名前
						if(userid == id){
							//自分の時はハイライト
							row.GetComponent<Image>().enabled = true;
							flag = true;
						}
					}
					if(flag && rank > 10){
						break;
					}
					if(userid == id && rank>10){
						//Textに反映
						GameObject row = rankingSpace.transform.GetChild(10).gameObject;
						row.transform.GetChild(0).gameObject.GetComponent<Text>().text = (rank).ToString(); //順位
						row.transform.GetChild(1).gameObject.GetComponent<Text>().text = score; //スコア
						row.transform.GetChild(2).gameObject.GetComponent<Text>().text = name; //名前
						row.GetComponent<Image>().enabled = true;
						break;
					}
		      rank++;
		    }

			}
		});
	}


}

