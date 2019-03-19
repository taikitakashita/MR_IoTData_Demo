using System;
using UnityEngine;
using System.IO;
using System.Net;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class AzureServices : MonoBehaviour
{
    /// <summary>
    /// このクラスをシングルトンのようにふるまわせるるためのインスタンス
    /// </summary>
    public static AzureServices Instance;

    /// <summary>
    /// AzureFunctionのエンドポイントのURL
    /// </summary>
    private readonly string azureFunctionEndpoint = "https://mriottestfunctions.azurewebsites.net/api/HttpTrigger1";

    private void Awake()
    {
        // このクラスをシングルトンとしてふるまわせる
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        // （UnityがTLSのサポートを追加するまで）UnityエディタでのみTLS証明書チェックを無効にする
    #if UNITY_EDITOR
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
    #endif
    }

    /// <summary>
    /// AzureFunctionの処理を呼び出す
    /// </summary>
    public IEnumerator CallAzureFunction()
    {
        Debug.Log("センサー情報取得処理開始");
        SensorInformationEntity sensorinformationentity = null;

        using (UnityWebRequest www = UnityWebRequest.Get(azureFunctionEndpoint))
        {
            // 受信したデータを格納するハンドラの初期化
            www.downloadHandler = new DownloadHandlerBuffer();
            //Debug.Log("++++++++++++++++++++1");

            // 送受信を開始し、完了するまで待つ
            yield return www.SendWebRequest();
            //Debug.Log("++++++++++++++++++++2");

            // ダウンロードした内容を文字列として取得
            string jsonResponse = www.downloadHandler.text;
            //Debug.Log("++++++++++++++++++++3");

            // 取得したJSON文字列を逆シリアライズ化
            sensorinformationentity = JsonConvert.DeserializeObject<SensorInformationEntity>(jsonResponse);
            //Debug.Log("++++++++++++++++++++4");
        }
        SensorInformationManager.Instance.SetSensorInformation(sensorinformationentity);
        //Debug.Log("++++++++++++++++++++5");
        Debug.Log("センサー情報取得及び分析処理完了");
    }
}
