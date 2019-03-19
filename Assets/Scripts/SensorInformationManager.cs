using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorInformationManager : MonoBehaviour {

    /// <summary>
    /// このクラスをシングルトンとしてふるまわせるためのインスタンス
    /// </summary>
    public static SensorInformationManager Instance;

    /// <summary>
    /// センサーの値を表示するTemperatureScoreとHumidityScoreのLabelをインスペクターから設定
    /// </summary>
    public TextMesh TemperatureScoreLabel;
    public TextMesh HumidityScoreLabel;

    /// <summary>
    /// センサー値の閾値超過を表示するArarmMesaageのLabelをインスペクターから設定
    /// </summary>
    public TextMesh TemperatureArarmLabel;
    public TextMesh HumidityArarmLabel;

    // センサー値の閾値超過を判定する基準
    private int TemperatureThreshold = 30;
    private int HumidityThreshold = 70;

    //センサー情報取得処理を行う間隔
    private float IntervalTime = 5.0f;

    /// <summary>
    /// Initialises this class
    /// </summary>
    private void Awake()
    {
        // このクラスのインスタンスをシングルトンとして利用する
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        // センサーの値を表示するTemperatureScoreとHumidityScoreのLabelの初期値を表示
        TemperatureScoreLabel.text = "取得中...";
        HumidityScoreLabel.text = "取得中...";

        // センサー値の閾値超過を表示するArarmMesaageのLabelの初期値を表示
        TemperatureArarmLabel.text = "";
        HumidityArarmLabel.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        IntervalTime = IntervalTime + Time.deltaTime;
        //Debug.Log(IntervalTime);
        if (IntervalTime >= 5.0f)
        {
            StartCoroutine(AzureServices.Instance.CallAzureFunction());
            IntervalTime = 0;
        }
    }

    public void SetSensorInformation(SensorInformationEntity entity)
    {
        double ShowTemperatureScore = (double)Math.Round(entity.temperature, 2, MidpointRounding.AwayFromZero);
        double ShowHumidityScore = (double)Math.Round(entity.humidity, 2, MidpointRounding.AwayFromZero);

        TemperatureScoreLabel.text = ShowTemperatureScore.ToString();
        HumidityScoreLabel.text = ShowHumidityScore.ToString();

        if (ShowTemperatureScore > TemperatureThreshold)
        {
            TemperatureArarmLabel.text = "温度は閾値を超過しました。";
            TemperatureArarmLabel.color = Color.red;
        }
        else
        {
            TemperatureArarmLabel.text = "温度は正常です";
            TemperatureArarmLabel.color = Color.green;
        }

        if (ShowHumidityScore > HumidityThreshold)
        {
            HumidityArarmLabel.text = "湿度は閾値を超過しました。";
            HumidityArarmLabel.color = Color.red;
        }
        else
        {
            HumidityArarmLabel.text = "湿度は正常です";
            HumidityArarmLabel.color = Color.green;
        }
    }
}

/// <summary>
/// 取得したセンサ情報を格納するクラス
/// </summary>
public class SensorInformationEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string deviceid { get; set; }
    public DateTime eventenqueuedutctime { get; set; }
    public double humidity { get; set; }
    public double temperature { get; set; }
}
