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
	public GameObject imageResult; //結果画面

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

		var ref_online = FirebaseDatabase.DefaultInstance
      		.GetReference("online");

      	ref_online.Child("room").Child(PlayerPrefs.GetString("roomnum_str")).
		  	Child(PlayerPrefs.GetString("enemyid")).ChildChanged += HandleChildChanged;
	}

	void HandleChildChanged(object sender, ChildChangedEventArgs args) {
      if (args.DatabaseError != null) {
        Debug.LogError(args.DatabaseError.Message);
        return;
      }
      // Do something with the data in args.Snapshot
	  Debug.Log(args.Snapshot);
	  if(args.Snapshot.Key == "data"){
		//相手がナンバーをコールしたらそのナンバーとeatbiteを取得
		FirebaseDatabase.DefaultInstance
		.GetReference("online")
		.GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted) {
				
			}
			else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				Dictionary<string, object> data = new Dictionary<string, object>();
				data = snapshot.Child("room").Child(PlayerPrefs.GetString("roomnum_str")).
					Child(PlayerPrefs.GetString("enemyid")).Child("data").Value as Dictionary<string, object>;

				Debug.Log(data);
				
				textMain.SetActive(false);
				textNumber.SetActive(false);
				textResult.SetActive(true);
				textResult.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = data["callnum"].ToString();
				textResult.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = data["eat"].ToString();
				textResult.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = data["bite"].ToString();

				reference.Child("online").Child("room").Child(PlayerPrefs.GetString("roomnum_str")).
					Child(PlayerPrefs.GetString("userid")).Child("myturn").SetValueAsync(1);
				PlayerPrefs.SetInt("myturn",1);

				if(int.Parse(data["eat"].ToString()) == 3){
					Debug.Log("lose");
					Invoke("ShowLose",1.5f);
				}else{
					Invoke("Offense",2.5f);
				}

			}
		});
	  }

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

		Dictionary<string,object> data = new Dictionary<string, object>(){
			{"eat",eat},
			{"bite",bite},
			{"callnum",callnum}
		};
		reference.Child("online").Child("room").Child(PlayerPrefs.GetString("roomnum_str")).
			Child(PlayerPrefs.GetString("userid")).Child("data").UpdateChildrenAsync(data);
		reference.Child("online").Child("room").Child(PlayerPrefs.GetString("roomnum_str")).
			Child(PlayerPrefs.GetString("userid")).Child("myturn").SetValueAsync(0);

		textMain.SetActive(false);
		textNumber.SetActive(false);
		textResult.SetActive(true);
		textResult.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = callnum;
		textResult.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = eat.ToString();
		textResult.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = bite.ToString();

		if(eat == 3){
			Debug.Log("win");
			Invoke("ShowWin",1.5f);
		}else{
			Invoke("Defense",2.5f);
		}

	}

	private void ShowWin(){
		imageResult.SetActive(true);
		imageResult.transform.Find("Text").gameObject.GetComponent<Text>().text = "WIN";
		imageResult.transform.Find("Text").gameObject.GetComponent<Text>().color = Color.red;
		iTween.MoveFrom(imageResult, iTween.Hash("x",5,"time",1));
	}

	private void ShowLose(){
		imageResult.SetActive(true);
		imageResult.transform.Find("Text").gameObject.GetComponent<Text>().text = "LOSE";
		imageResult.transform.Find("Text").gameObject.GetComponent<Text>().color = Color.blue;
		iTween.MoveFrom(imageResult, iTween.Hash("x",5,"time",1));
	}

}
