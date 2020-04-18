using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Entity :MonoBehaviour
{
    [HideInInspector]
    public Class1 class1 = new Class1();


    private void Update()
    {
        transform.position = class1.pos;

    }
    private void OnGUI()
    {
        if (GUILayout.Button("test")) {
            transform.position= class1.pos;

        }
    }
}
