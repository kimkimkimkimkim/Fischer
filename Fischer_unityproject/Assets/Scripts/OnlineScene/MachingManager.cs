using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using TMPro;

public class MachingManager : MonoBehaviour {

	public GameObject textMain; //メインテキスト
	public GameObject textNumber; //数字テキスト
	public GameObject keybord; //キーボード

	private GameObject indicator;
	private DatabaseReference reference;
	private bool isLastRoom = true;
	private string userid = "";
	private string roomnum_str = "";

	

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

		//初回起動時useridを設定
		if(PlayerPrefs.GetString("userid") == ""){
			System.Guid guid = System.Guid.NewGuid ();
			string uuid = guid.ToString ();
			PlayerPrefs.SetString("userid",uuid);
		}else{
			//Debug.Log(PlayerPrefs.GetString(PPKey.userid.ToString()));
		}

		//オブジェクトの参照
		referenceObject();

		//インジケーターの表示
		indicator.GetComponent<ActivityIndicator>().Show();

		//userid設定
		userid = PlayerPrefs.GetString("userid");

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
		.GetReference("online")
		.GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted) {
				
			}
			else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;

				IEnumerator<DataSnapshot> en = snapshot.Child("room").Children.GetEnumerator();;

				while (en.MoveNext()) {
					DataSnapshot data = en.Current;
					/* 
					string name = (string)data.Child("name").Value;
					int score   = (int)(long)data.Child("score").Value;
					*/
					if(data.ChildrenCount == 1 ){
						Dictionary<string,object> user = new Dictionary<string, object>();
						user = data.Value as Dictionary<string,object>;
						string key_userid = "";
						foreach (string key in user.Keys) {
							key_userid = key;
						}
						Debug.Log(key_userid);
						if(key_userid != PlayerPrefs.GetString("userid")){
							//部屋に一人しかいないかつその一人が自分じゃない
							PlayerPrefs.SetString("enemyid",key_userid);
							roomnum_str = data.Key;
							intoTheRoom(int.Parse(data.Key));
							Debug.Log(data.Key + "の部屋に参加");
							isLastRoom = false;
							break;
						}else{
							//部屋に一人しかいないけどその一人が自分
							Debug.Log("自分が作った部屋に入りました");
							waitMember(int.Parse(data.Key));
							isLastRoom = false;
							break;
						}
					}
				}

				//空いてる部屋がない
				if(isLastRoom){
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
		reference.Child("online").Child("room").Child(roomnum.ToString()).Child(userid).SetRawJsonValueAsync(json);

		//ゲームスタート
		gameStart();
	}

	//部屋を作成
	private void createRoom(){
		int roomnum=0;
		//部屋の中に一人しかいない時OK
		FirebaseDatabase.DefaultInstance
		.GetReference("online")
		.GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted) {
				
			}
			else if (task.IsCompleted) {
				//Firebaseから今ある部屋数を取得してそれに一足した部屋番号の部屋を作成
				DataSnapshot snapshot = task.Result;
				roomnum = int.Parse(snapshot.Child("roomnum").Value.ToString()) + 1;
				roomnum_str = roomnum.ToString();
				reference.Child("online").Child("roomnum").SetValueAsync(roomnum);

				//自分の情報をjson形式で保存
				string json = "{\"host\":\"hostman\",\"rate\":1500}";
				//部屋に自分の情報を反映
				reference.Child("online").Child("room").
					Child(roomnum.ToString()).Child(userid).SetRawJsonValueAsync(json);
				Debug.Log(roomnum+"の部屋を作成");
				waitMember(roomnum);
			}
		});
	}

	private void waitMember(int roomnum){
		Debug.Log("入ってくるのを待ってます");
		var ref_online = FirebaseDatabase.DefaultInstance
      		.GetReference("online");

      	ref_online.Child("room").Child(roomnum.ToString()).ChildAdded += HandleChildAdded;
	}

	void HandleChildAdded(object sender, ChildChangedEventArgs args) {
      if (args.DatabaseError != null) {
        Debug.LogError(args.DatabaseError.Message);
        return;
      }
      // Do something with the data in args.Snapshot
	  Debug.Log(args.Snapshot.Key);
	  if(args.Snapshot.Key != PlayerPrefs.GetString("userid")){
		  //敵のidを保存
		  PlayerPrefs.SetString("enemyid",args.Snapshot.Key);
		  //誰かが入ってきた
		  gameStart();
	  }

    }

	//ゲームスタート
	private void gameStart(){

		//インジケーターの非表示
		indicator.GetComponent<ActivityIndicator>().Hide();

		//自分のナンバーを設定する
		textNumber.SetActive(false);
		textMain.GetComponent<Text>().text = "プレイヤーが揃いました。\nナンバーをセットして対戦を開始してください。";
		keybord.SetActive(true);
		keybord.GetComponent<KeybordManager>().GetRef(roomnum_str);
	}
}
