using Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class DB
{
    public static List<TestBase> TestBaseList = new List<TestBase>();



    static string GetCsvTypeName(string rel_pathname) {
        var name = Path.GetFileName(rel_pathname);
        var idx = name.IndexOf("--");
        if (idx < 0) idx = name.IndexOf('.');
        if (idx > 0) name = name.Substring(0, idx);
        return name;
    }

    public static IEnumerator ParseAndInitCsvData(string rel_pathname, byte[] data) {
        string type_name = GetCsvTypeName(rel_pathname);
        var list_filed = typeof(DB).GetField(type_name+"List");
        var map_field = typeof(DB).GetField(type_name+"Map");

        IList data_list = null;
        if (list_filed != null)
        {
            data_list = list_filed.GetValue(null) as IList;
            var data_type = Type.GetType("Entity."+type_name);
            data_list.Clear();
            //yield return Sta
            var t = Task.Run(async () => {
                await CsvUtils.Deserialize(data, "Entity." + type_name, data_type, data_list);
            });
            while (!t.IsCompleted)
            {
                yield return 10;
            }
            if (map_field != null)
            {
                var map = map_field.GetValue(null) as IDictionary;
                map.Clear();
                var id_fileLd = data_type.GetField("id");
                if (id_fileLd != null)
                {
                    if (id_fileLd != null)
                    {
                        if (data_list.Count == 0)
                            Debug.LogError($"{rel_pathname}表数据为0");
                        for (var i=0;i<data_list.Count;++i) {
                            var ele = data_list[i];
                            var id = (int)id_fileLd.GetValue(ele);
                            map.Add(id,ele);
                        }
                    }
                    else {
                        Debug.LogError($"{rel_pathname}表找不到id字段");
                    }

                }
            }

        }
    }
}

