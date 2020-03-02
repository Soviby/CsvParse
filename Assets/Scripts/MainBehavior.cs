using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public  class MainBehavior :MonoBehaviour
{
    static string path = Application.streamingAssetsPath+ @"/DemoBase.csv";


    private void OnGUI()
    {
        if (GUILayout.Button("解析"))
        {

            StartCoroutine(DB.ParseAndInitCsvData(path));
        }
    }
}

