using UnityEngine;
using System.Collections;
 
public class Main : MonoBehaviour {
	public GameObject test;
    void Start () {
		test.GetComponent<Test>().SetTimer (CallBack, 5);
	}
    void Update () {
        // 定義済みのメソッドを呼ぶ場合
        //Test.Hoge (Hoge, "Hoge Method");
        // 匿名関数も使える
        //Test.Hoge ((a,b) => { Debug.Log (a+":"+b.ToString()); }, "Lambda");
    }
 
    void CallBack(	) {
        Debug.Log("終わりました");
    }
}

