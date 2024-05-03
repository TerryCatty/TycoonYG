using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public interface ISaveble
{
    public string Save();

    public void LoadData(string value);

    public void DeleteData();
}

[Serializable]
public class SaveDataInfo
{

    public string loadData = "";

}
public class SavesManager : MonoBehaviour
{
    [SerializeField] private List<ISaveble> _listSaves = new List<ISaveble>();


    [DllImport("__Internal")]
    private static extern void SaveDataExtern(string value);
    [DllImport("__Internal")]
    private static extern void LoadDataExtern(string objName);

    [SerializeField] private TextMeshProUGUI textDebug;

    private BenchesManager benchesManager;
    private MoneyManager moneyManager;


    public static SavesManager instance = null;

    [SerializeField] private SaveDataInfo saveDataInfo = new SaveDataInfo();

    private void Awake()
    {

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        LoadData();

        moneyManager = FindAnyObjectByType<MoneyManager>();
        benchesManager = FindAnyObjectByType<BenchesManager>();

        /*_listSaves.Add(moneyManager);
        _listSaves.Add(benchesManager);*/

        benchesManager.Init(this);
        moneyManager.Init(this);

        
    }

    public void DeleteData()
    {
        foreach (ISaveble obj in _listSaves.ToList())
        {
            obj.DeleteData();
        }

        SaveData();
    }

    public void AddSaveObj(ISaveble obj)
    {
        if (_listSaves.Contains(obj)) return;

        _listSaves.Add(obj);
    }

    public void SaveData()
    {
        
        string save = "";

        foreach(ISaveble obj in _listSaves)
        {
            save += obj.Save() + ";";
        }

        saveDataInfo.loadData = save;


        string jsonString = JsonUtility.ToJson(saveDataInfo);


        try
        {
            Log("save - " + jsonString);
            SaveDataExtern(jsonString);
        }
        catch
        {
            Log("Error to save");
        }
    }

    public void LoadData()
    {
        Load(saveDataInfo.loadData);
        try
        {
            LoadDataExtern(gameObject.name);
        }
        catch
        {

            Log("Error to load " + gameObject.name);
        }

    }

    public void Load(string value)
    {
        //value = "{\"benchManager0000\":{\"openBenches\":1}};{\"MoneyManager\":{\"money\":100}};{\"7262_bench\":{\"openPlaces\":2}};\r";
        SaveDataInfo info = JsonUtility.FromJson<SaveDataInfo>(value);
        Log("load - " + info);
        saveDataInfo = info;

        foreach(ISaveble obj in _listSaves.ToList())
        {
            obj.LoadData(saveDataInfo.loadData);
        }

    }

    public string GetLoadData()
    {
        return saveDataInfo.loadData;
    }

    public void Log(string value)
    {
        textDebug.text += value + "\n";
    }
}
