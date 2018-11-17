using UnityEngine;
using System.Collections;
 

public class Test : MonoBehaviour {
    public delegate void TestCallback();

    public static float totalTime = 0.0f;

	private bool isStart = false;
 

    void Start() { }

    void Update () {

        if(isStart){
			totalTime += Time.deltaTime;
		}

    }

     

    public void SetTimer(TestCallback Callback, int time) {
		isStart = true;
    }

	public void Timer(TestCallback Callback){

	}

}
