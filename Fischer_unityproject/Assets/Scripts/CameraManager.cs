using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	private float angle = 0; //回転角
	private float speed = 3f; //回転速度
	private float dis = 7; //回転半径

	// Use this for initialization
	void Start () {
		
	}

	void FixedUpdate(){
		angle += Time.deltaTime * speed;
		updateAngle();
		updatePos();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void updateAngle(){
		this.transform.localRotation = Quaternion.Euler(21f,angle,0f);
	}

	private void updatePos(){
		float sin = (float)Math.Sin(angle * (Math.PI / 180));
		float cos = (float)Math.Cos(angle * (Math.PI / 180));
		this.transform.localPosition = new Vector3(-1*dis*sin,3,-1*dis*cos);
	}

}
