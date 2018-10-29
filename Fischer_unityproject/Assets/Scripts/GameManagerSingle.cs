using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerSingle : MonoBehaviour {

	public  GameObject historyPrefab; //履歴のプレハブ

	private GameObject selectView; //3,4,5枚かを選ぶビュー
	private GameObject containerKeypad; //キーパッドコンテナ
	private GameObject containerCard; //カードコンテナ
	private GameObject historyContent; //履歴のスクロールビューのコンテンツ
	private GameObject imageWin; //勝利時に表示する画像
	private int[] callNum = new int[3]; //コールした数字配列
	private int[] eneNum = new int[3]; //相手の数列
	private int count = 0; //コールした数字の数

	// Use this for initialization
	void Start () {
		selectView = transform.parent.Find("SelectGameMode").gameObject;
		containerKeypad = transform.Find("ContainerKeyPad").gameObject;
		containerCard = transform.Find("ContainerCard").gameObject;
		historyContent = transform.Find("History").Find("Scroll View").GetChild(0).GetChild(0).gameObject;
		imageWin = transform.Find("Fischer").gameObject;
		SettingKeyPad();
		CreateEneNum();
	}

	void OnEnable(){
		CreateEneNum();
		ClickReset();
		ClickReset();
		ClickReset();

		int childN = historyContent.transform.childCount;
		for(int i=0;i<childN;i++){
			Destroy(historyContent.transform.GetChild(i).gameObject);
		}
	}

	private void SettingKeyPad(){
		int num = 0; //数字
		for(int i=0;i<4;i++){
			GameObject row = containerKeypad.transform.GetChild(i).gameObject; //キーパッドの列コンテナ
			for(int j=0;j<3;j++){
				GameObject button = row.transform.GetChild(j).gameObject; //ボタン
				GameObject text = button.transform.GetChild(0).gameObject; //ボタンのテキスト
				text.GetComponent<Text>().text = num.ToString(); //ボタンのテキストに数字を代入
				num++; //数字をカウントアップ

				//再下段だけ違う処理
				if(i==3){
					switch(j){
						case  0:
							text.GetComponent<Text>().text = "call"; //ボタンのテキストに数字を代入
							break;
						case 1:
							text.GetComponent<Text>().text = "9"; //ボタンのテキストに数字を代入
							break;
						case 2:
							text.GetComponent<Text>().text = "reset"; //ボタンのテキストに数字を代入
							break;
						default:
							break;
					}
				}
			}
		}
	}

	public void ClickNum(string num){
		if(count==3)return; //３つとも数字を選んでいたらそのままリターン

		callNum[count] = int.Parse(num); //callNum配列の内容変更
		containerCard.transform.GetChild(count).GetChild(0).GetComponent<Text>().text 
			= num;	//UIにも反映
		count++; //カウントアップ

	}


	//コールする
	public void ClickCall(){
		if(count!=3)return; //3つ選んでなかったらそのままリターン

		//プレハブの生成
		GameObject hist = (GameObject)Instantiate(historyPrefab);
		hist.transform.SetParent(historyContent.transform);
		hist.transform.localPosition = new Vector3 (0,0,0);
		hist.transform.localRotation = Quaternion.Euler(0,0, 0);
		hist.transform.localScale = new Vector3(1,1,1);

		CalNumeron(eneNum,callNum,hist);

		//初期状態に戻す
		ClickReset();
		ClickReset();
		ClickReset();
	}

	//１つ数字を削除
	public void ClickReset(){
		if(count==0)return; //コールナンバーがなかったらそのままリターン
		count--; //カウントダウン
		containerCard.transform.GetChild(count).GetChild(0).GetComponent<Text>().text = ""; //UIに反映
	}

	//相手の数列を作成
	public void CreateEneNum(){
		//シャッフルする配列
		int[] ary = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

		//Fisher-Yatesアルゴリズムでシャッフルする
		System.Random rng = new System.Random();
		int n = ary.Length;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			int tmp = ary[k];
			ary[k] = ary[n];
			ary[n] = tmp;
		}

		//enenumに代入
		for(int i=0;i<3;i++){
			eneNum[i] = ary[i];
		}
	}

	//ヌメロンの計算
	public void CalNumeron(int[] eneary,int[] userary,GameObject prefab){
		int eat=0,bite=0;

		//一個一個確認していく（eatとbiteを計算）
		for(int i=0;i<3;i++){
			for(int j=0;j<3;j++){
				if(userary[i]==eneary[j]){
					if(i==j){
						eat++;
					}else{
						bite++;
					}
				}
			}
		}

		//UIに反映
		string str = string.Format("{0}{1}{2}",userary[0],userary[1],userary[2]);
		prefab.transform.GetChild(0).gameObject.GetComponent<Text>().text = str;
		prefab.transform.GetChild(1).gameObject.GetComponent<Text>().text = eat.ToString();
		prefab.transform.GetChild(2).gameObject.GetComponent<Text>().text = bite.ToString();

		//eatが3なら終了
		//Numeronの表示
		if(eat==3){
			ShowWinImage();
			Invoke("Finish",3.5f);
		}	

	}

	private void ShowWinImage(){
		imageWin.SetActive(true);
		iTween.ValueTo(gameObject,iTween.Hash("from",0,"to",1,"time",2,
			"onupdate","UpdateAlfa","onupdatetarget",gameObject));
	}

	private void UpdateAlfa(float a){
		imageWin.GetComponent<Image>().color = new Color(1,1,1,a);
	}

	private void Finish(){
		imageWin.SetActive(false);
		gameObject.SetActive(false);
		selectView.SetActive(true);
	}

}
