using UnityEngine;
using System.Collections;

/// <summary>
/// iOSやAndroidでインジケーターを表示するためのクラス
/// Unity上では表示されない
/// </summary>
public class ActivityIndicator : MonoBehaviour {

    //インジケーターの種類をインスペクターで設定する用
    //ビルド時にエラーが出ないように、かつ、常にインスペクターに両方表示するように#ifを追加
    #if UNITY_EDITOR || UNITY_IOS
        [SerializeField]
        private UnityEngine.iOS.ActivityIndicatorStyle _iosStyle;
    #endif
    #if UNITY_EDITOR || UNITY_ANDROID
        [SerializeField]
        private AndroidActivityIndicatorStyle _androidStyle;
    #endif

    //=================================================================================
    //初期化
    //=================================================================================

    private void Awake (){
        #if UNITY_IPHONE
            Handheld.SetActivityIndicatorStyle(_iosStyle);
        #elif UNITY_ANDROID
            Handheld.SetActivityIndicatorStyle(_androidStyle);
        #endif
    }

    //=================================================================================
    //表示の切り替え
    //=================================================================================

    /// <summary>
    /// インジケーターの表示
    /// </summary>
    public void Show(){
        //フレームの最後にインジケーターのアニメーションが始まるので、フレームの終わりまで待つ
        StartCoroutine(StartActivityIndicator());
    }

    private IEnumerator StartActivityIndicator(){
        Handheld.StartActivityIndicator();
        yield return new WaitForSeconds(0);
    }

    /// <summary>
    /// インジケーター非表示
    /// </summary>
    public void Hide(){
        Handheld.StopActivityIndicator ();
    }

}