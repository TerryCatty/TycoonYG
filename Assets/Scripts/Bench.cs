using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using YG;

[System.Serializable]
public class BenchData { 

    public int openPlaces;

}
public class Bench : MonoBehaviour, ISaveble
{

    private bool isInit = false;

    [SerializeField] private string identificator;

    [SerializeField] private GameObject itemPrefab;

    [SerializeField] private List<Transform> items;
    private List<Transform> tempList = new List<Transform>();

    [SerializeField] private float timeBetweenMoves;

    private float timer;

    [SerializeField] private float stepValue;

    [SerializeField] private Transform conveyor;

    [SerializeField] private Transform targetContainer;

    [SerializeField] private int countItems;

    [SerializeField] private int costPlace;

    [SerializeField] private uint moneyGet;

    [SerializeField] private BenchData data;

    [SerializeField] private Vector2[] positions;

    [SerializeField] private float sizeX;

    private SavesManager savesManager;

    public void Init(int id, SavesManager savesManager)
    {
        this.savesManager = savesManager;

        savesManager.AddSaveObj(this);


        identificator = id + "_bench";

        LoadData(savesManager.GetLoadData());

        InitGrid();

        SpawnItems();


        timer = timeBetweenMoves;

        isInit = true;
    }
    
    private void InitGrid()
    {
        positions = new Vector2[countItems];

        float offset = sizeX / ((float)countItems - 1);

        for (int i = 0; i < countItems; i++)
        {
            positions[i] = new Vector2((sizeX / 2) - offset * i, 0);
        }
    }
   
    public string Save()
    {
        string jsonString = "{" + $"\"{identificator}\":{JsonUtility.ToJson(data)}" + "}";

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

        if (parametersObj != "")
            Load(parametersObj);
    }

    private void Load(string value)
    {
        string temp = "{" + $"\"{identificator}\":";
        value = value.Remove(0, temp.Length);
        value = value.Remove(value.Length - 1);


        data = JsonUtility.FromJson<BenchData>(value);
    }

    public void DeleteData()
    {
        data.openPlaces = 0;
    }

    private void SpawnItems()
    {
        if(data.openPlaces > positions.Length) data.openPlaces = positions.Length;

        for (int i = 0; i < data.openPlaces; i++)
        {
            GameObject spawnedItem = Instantiate(itemPrefab, positions[i], Quaternion.identity);

            spawnedItem.transform.SetParent(conveyor);

            spawnedItem.transform.localPosition = positions[i];

            items.Add(spawnedItem.transform);
        }
    }

    private void Update()
    {
        if (isInit == false) return;

        if(timer <= 0)
        {
            timer = timeBetweenMoves;
            foreach(Transform item in items)
            {
                item.position = new Vector2(item.position.x - stepValue, item.position.y);
                CheckItemPosition(item);
            }

            UpdateListItems();

            CheckListNull();
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    private void CheckItemPosition(Transform item)
    {
        if(item.position.x < targetContainer.position.x)
        {
            MoneyManager.instance.IncreaseMoney(moneyGet);
            tempList.Add(item);
            Destroy(item.gameObject);
        }
    }

    private void UpdateListItems()
    {
        if (tempList.Count == 0) return;

        foreach(Transform item in tempList)
        {
            items.Remove(item);
        }

        tempList = new List<Transform>();
    }

    private void CheckListNull()
    {
        if(items.Count == 0)
        {
            SpawnItems();
        }
    }

    public void AddPlace()
    {
        if (MoneyManager.instance.GetMoney() < costPlace || data.openPlaces >= countItems) return;

        data.openPlaces++;

        MoneyManager.instance.CostMoney((uint)costPlace);

        SavesManager.instance.SaveData();
    }


}
