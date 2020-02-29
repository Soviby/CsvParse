using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public  class MainBehavior :MonoBehaviour
{
    static string path = Application.streamingAssetsPath+@"/demo.csv";


    private void OnGUI()
    {
        if (GUILayout.Button("解析"))
        {
            
            var list = CsvHelper.AnalysisCsvListByFile(path);

            foreach (var item in list)
            {
                Debug.Log("------");
                foreach (var value in item)
                {
                    Debug.Log(value);
                }
                Debug.Log("------");
            }
        }
    }
}

