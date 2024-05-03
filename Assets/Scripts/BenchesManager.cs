using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BenchesInfo
{
    public int[] isBuying;

    public int openBenches;
}

public class BenchesManager : MonoBehaviour, ISaveble
{
    [SerializeField] private string identificator;

    [SerializeField] private BuingBench[] benches;

    [SerializeField] private GameObject buttonsEmptyParent;


    [SerializeField] private string templateSpawnButton;

    [SerializeField] private List<Bench> benchesList = new List<Bench>();

    [SerializeField] private BenchesInfo benchesInfo = new BenchesInfo();

    private SavesManager savesManager;

    
   
    public void Init(SavesManager savesManager)
    {
        benchesInfo.isBuying = new int[benches.Length];

        this.savesManager = savesManager;

        savesManager.AddSaveObj(this);

        LoadData(savesManager.GetLoadData());

        ActiveButton();
    }

    private void ActiveButton()
    {
        Button[] buttons = buttonsEmptyParent.GetComponentsInChildren<Button>();

        foreach (var button in buttons)
        {
            string parse = button.gameObject.name.Replace(templateSpawnButton, " ");

            int index = int.Parse(parse);

            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

            text.text = benches[index].cost.ToString();

            if (benchesInfo.isBuying[index] == 1) button.gameObject.SetActive(false);
        }
    }

    public void BuyBench(GameObject gameObject)
    {
        int index = int.Parse(gameObject.name.Replace(templateSpawnButton, ""));


        if (index >= benches.Length) return;


        if (MoneyManager.instance.GetMoney() < benches[index].cost) return;

        gameObject.SetActive(false);

        MoneyManager.instance.CostMoney((uint)benches[index].cost);

        benchesInfo.isBuying[index] = 1;

        benchesInfo.openBenches++;

        SavesManager.instance.SaveData();

        SpawnBench(index);

    }
    public void SpawnBench(int index)
    {
        Bench bench = Instantiate(benches[index].bench, transform);

        bench.transform.position = benches[index].position;

        System.Random rand = new System.Random(index);

        int id = rand.Next(0, 10000);

        bench.Init(id, savesManager);


        benchesList.Add(bench);



    }

    public void DeleteData()
    {
        benchesInfo.openBenches = 0;

        for(int i = 0; i < benchesInfo.isBuying.Length; i++)
        {
            benchesInfo.isBuying[i] = 0;
        }


    }

    public string Save()
    {
        string jsonStringManager = "{" + $"\"{identificator}\":{JsonUtility.ToJson(benchesInfo)}" + "}";
        return jsonStringManager;
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

        SavesManager.instance.Log("load - " + value);

        BenchesInfo info = JsonUtility.FromJson<BenchesInfo>(value);


        benchesInfo = info;


        SavesManager.instance.Log("openBenches - " + benchesInfo.openBenches);

        ActiveButton();

        for (int i = 0; i < benchesInfo.openBenches; i++)
        {
            SpawnBench(i);
        }
    }

}

[Serializable]

public struct BuingBench
{
    public Bench bench;
    public Vector2 position;
    public int cost;
}
