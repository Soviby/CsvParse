using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Entity))]
public class InspectorDemo : Editor
{
    Entity entity;

    private void OnEnable()
    {
        entity = (Entity)target;

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();
        //中间的内容可变化
        EditorGUILayout.LabelField(entity.name);

        ShowFiled();

        //entity.class1.text = EditorGUILayout.TextField("text", entity.class1.text);

        //entity.class1.index = EditorGUILayout.IntField("index", entity.class1.index);

        //entity.class1.pos = EditorGUILayout.Vector3Field("pos", entity.class1.pos);


        EditorGUILayout.EndVertical();
    }

    void ShowFiled() {
        //使用反射
        //1.遍历entity中所有字段
        //2.遍历该字段中的所有字段 
        //3.字段的显示
        //1.判断字段类型
        //2. entity.class1.text=EditorGUILayout.TextField("text",entity.class1.text);

        var fileInfos = typeof(Entity).GetFields();
        foreach (var info in fileInfos)
        {
            var field_obj = info.GetValue(entity);
            var item_fieldInfos = field_obj.GetType().GetFields();
            foreach (var item_info in item_fieldInfos)
            {
                var item_field_obj = item_info.GetValue(field_obj);//组件中的字段
                FieldHandle(ref item_field_obj, item_info);
                item_info.SetValue(field_obj, item_field_obj);
            }
        }
    }

    void FieldHandle(ref object obj, FieldInfo fieldInfo) {
        var type = obj.GetType();
        if (type == typeof(string))
        {
            obj = EditorGUILayout.TextField(fieldInfo.Name, obj.ToString());
        }
        else if (type == typeof(Int32))
        {
            obj = EditorGUILayout.IntField(fieldInfo.Name, Convert.ToInt32(obj));
        }
        else if (type == typeof(Vector3))
        {
            obj = EditorGUILayout.Vector3Field(fieldInfo.Name, (Vector3)obj);
        }
    }
}
