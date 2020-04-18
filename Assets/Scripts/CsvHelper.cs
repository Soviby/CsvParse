using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CsvHelper
{
    public static void PaseAllDate(string text , ref List<List<string >> result) {
        if (result == null) result = new List<List<string>>();
        if (text == null) return;

        List<string> line = new List<string>();//当前行
        char[] chars = text.ToCharArray();//所有字符
        int length = chars.Length;//总字符长度

        StringBuilder filed = new StringBuilder();//当前字段
        bool isInQutation = false;//在双引号中
        bool isStarFiled = true;//是否是字段的开始

        for (int i = 0; i < length; i++)
        {
            char ch = chars[i];
            //在双引号内
            if (isInQutation)
            {
                if (ch == '"')
                {
                    //连续2个"，则转义一个"
                    if (i < length - 1 && chars[i + 1] == '"')
                    {
                        filed.Append('"');
                        i++;
                    }
                    else//结束字符串模式
                    {
                        isInQutation = false;
                    }

                }//否则都加入到当前字段
                else
                {
                    filed.Append(ch);
                }
            }
            else {
                switch (ch) {
                    case '"':
                        if (isStarFiled)
                            isInQutation = true;
                        else
                            filed.Append(ch);
                        break;
                    case ','://结束一个字段
                        line.Add(filed.ToString());//添加当前行
                        filed.Length = 0;//清空字段
                        isStarFiled = true;//开始新字段
                        break;
                    case '\r':
                    case '\n':
                        if (filed.Length > 0 || isStarFiled)//如果有字段则加入
                        {
                            line.Add(filed.ToString());
                            filed.Length = 0;
                        }
                        if (line.Count > 0)//如果改行非空，则加入结果
                        {
                            result.Add(line);
                            line = new List<string>();
                        }
                        isStarFiled = true;
                        if (i < length - 1 && chars[i + 1] == '\n')//跳过\r\n
                            i++;
                        break;
                    default://默认加到字段
                        isStarFiled = false;
                        filed.Append(ch);
                        break;
                }
            }
        }
        //如果有数据（isInFiled标记）则加入，否则空的单元格是没有意义的
        if (filed.Length > 0 || isStarFiled && line.Count > 0)
            line.Add(filed.ToString());
        //非空行，则加入
        if (line.Count > 0)
            result.Add(line);

        if (result.Count == 0) result = null;
    }


    public static List<List<string>> AnalysisCsvListByFile(string csvPath)
    {
        if (File.Exists(csvPath))
        {
            string csvInfo = File.ReadAllText(csvPath, Encoding.UTF8);
            List<List<string>> result = new List<List<string>>();
            PaseAllDate(csvInfo, ref result);
            return result;
        }
        else
        {
            throw new FileNotFoundException("未找到文件：" + csvPath);
        }
    }
}

