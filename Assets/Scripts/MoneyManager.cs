using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class MoneyManagerInfo
{
    public int money;
}

public class MoneyManager : MonoBehaviour, ISaveble
{
    [SerializeField] private string identificator;

    private MoneyManagerInfo moneyManagerInfo = new MoneyManagerInfo();

    public static MoneyManager instance = null;


    private SavesManager savesManager;



    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void Init(SavesManager savesManager)
    {
        this.savesManager = savesManager;

        savesManager.AddSaveObj(this);

        LoadData(savesManager.GetLoadData());
    }

    public void IncreaseMoney(uint value)
    {
        moneyManagerInfo.money += (int)value;

        savesManager.SaveData();
    }

    public int GetMoney()
    {
        return moneyManagerInfo.money;
    }

    public void CostMoney(uint value)
    {
        moneyManagerInfo.money -= (int)value;

        savesManager.SaveData();
    }

    public string Save()
    {
       string jsonString = "{" + $"\"{identificator}\":{JsonUtility.ToJson(moneyManagerInfo)}" + "}";
       return jsonString;
    }

    public void LoadData(string value)
    {
        string[] arrParam = value.Split(";");

        string parametersObj = "";

        foreach (string param in arrParam)
        {
            if (param.Contains(identificator))
            {
                parametersObj = param;
                break;
            }
        }

        if(parametersObj != "")
            Load(parametersObj);
    }
    private void Load(string value)
    {
        string temp = "{" + $"\"{identificator}\":";
        value = value.Remove(0, temp.Length);
        value = value.Remove(value.Length - 1);

        moneyManagerInfo = JsonUtility.FromJson<MoneyManagerInfo>(value);

        SavesManager.instance.Log("load - " + value);
    }

    public void DeleteData()
    {
        moneyManagerInfo.money = 0;
    }
}
