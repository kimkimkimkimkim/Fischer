using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToOnlineScene : MonoBehaviour {

	public void OnClick(){
		SceneManager.LoadScene ("OnlineScene");
	}
}
