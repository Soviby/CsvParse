using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public static class CsvUtils
{
    public static int TITLE_LINE_IDX = 1;
    public static int BREAK_COUNT_FOR_LINE = 200;
    /// <summary>
    /// 解析CSV，填充list
    /// </summary>
    /// <param name="data"></param>
    /// <param name="typename"></param>
    /// <param name="scType"></param>
    /// <param name="ret"></param>
    /// <returns></returns>
    public static async Task Deserialize(byte[] data,string typename ,Type csType, IList ret) {
        string text ="";
        var t = Task.Run(async() => {
            await DeserializeAsync(text,typename,csType,ret);
        });
        await t;
    }

    static async Task DeserializeAsync(string text,string typename,Type csType,IList ret) {
        List<List<string>> lint_list = new List<List<string>>();
        //解析整个文件

        if (lint_list == null || lint_list.Count <= TITLE_LINE_IDX + 1) return;
        Type type = csType;
        //分析字段名
        List<string> title = lint_list[TITLE_LINE_IDX];//第一行标题
        List<MemberInfo> field_list = new List<MemberInfo>();
        ParseInfoList(type,title,ref field_list);

        //分析每行数据
        for (int i= TITLE_LINE_IDX+1;i< lint_list.Count;i++) {
            List<string> data = lint_list[i];

            var obj = ParseObj(field_list,title,data,type);
            if (obj != null)
                ret.Add(obj);

            if (i % BREAK_COUNT_FOR_LINE == 0) await Task.Delay(10);//每超过200条，则等待10毫秒
        }
    }

    //解析字段名
    public static void ParseInfoList(Type type, List<string> line, ref List<MemberInfo> field_list) {

        foreach (string name in line) {
            string name2 = Regex.Replace(name,@"__\w+$","");
            MemberInfo info = type.GetField(name2);
            if (info == null)
            {
                info = type.GetProperty(name2);
                if (info == null)
                {
                    Console.WriteLine("Warning: type{0} lost field {1}",type.Name,name2);
                }
            }
            field_list.Add(info);
        }
    }
    //反射生成对象
    public static object ParseObj(List<MemberInfo> info_list,List<string> title,List<string> data,Type type) {
        //创建对象
        var obj = Activator.CreateInstance(type);
        var iTypeName = type.Name;
        //解析每个字段
        int cout = Math.Min(data.Count, title.Count);
        for (int i=0;i<cout;i++) {
            var info = info_list[i];
            if (info == null) continue;
            var value = data[i];

            try
            {
                //类型转换
                object value2;
                //转换
                if (info is FieldInfo)
                    value2 = ChangeType4Csv(value,(info as FieldInfo).FieldType);
                else
                    value2 = ChangeType4Csv(value, (info as PropertyInfo).PropertyType);


                if (info is FieldInfo) 
                    (info as FieldInfo).SetValue(obj,value2);
                else
                     (info as PropertyInfo).SetValue(obj, value2);

                }
            catch (Exception e)
            {
                //类型转换错误
                Debug.LogError($"{iTypeName}类型转换错误");
            }
        }
        return obj;
    }
    /// <summary>
    /// 转换CSV类型
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object ChangeType4Csv(string value,Type type) {
        if (value != null && value.GetType() == type) return value;
        if (value == "") value = null;

        if (type == typeof(string))
        {
            if (value == null) return "";
            return value;
        }
        if (type == typeof(Int32))
        {
            if (string.IsNullOrEmpty(value)) return 0;
            int ret = 0;
            if (value.Contains(".")) value = value.Substring(0,value.IndexOf('.'));
            if (int.TryParse(value, out ret))
                return ret;
            else
                return 0;
        }
        if (type == typeof(float))
        {
            if (string.IsNullOrEmpty(value)) return 0f;
            int ret = 0;
            if (int.TryParse(value, out ret))
                return ret;
            else
                return 0f;
        }
        throw new NotSupportedException(string.Format("类型转换错误！ type:{0},value:{1}",type,value));
    }
}
