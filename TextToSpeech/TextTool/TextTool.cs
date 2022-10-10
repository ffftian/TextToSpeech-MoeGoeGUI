﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


public static class TextTool
{
    public static List<BaseTextData> LogSplit (string text, Action<Exception, string> error)
    {


    }

    public static List<TText> QQLogSplit<TText>(string text, Action<Exception,string> error) where TText : QQTextData, new()
    {
        string[] SingleConversation = Regex.Split(text, "(?=\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2})");//这个靠零宽正向先行断言匹配了每一句话前是否是日期格式，问题在于必须跳过第一格。
        List<TText> dataList = new List<TText>();
        for (int i = 1; i < SingleConversation.Length; i++)//略过第一行，第一行固定为空。
        {
            TText txt = new TText();
            txt.Analysis(SingleConversation[i], i, error);
            dataList.Add(txt);
        }
        return dataList;
    }
    public static List<CommonTextData> CommonLogSplit(string text, Action<Exception, string> error)
    {
        string[] SingleConversation = Regex.Split(text, "(?=.+([0-9]+))");

        List<CommonTextData> dataList = new List<CommonTextData>();
        for (int i = 1; i < SingleConversation.Length; i++)//略过第一行，第一行固定为空。
        {
            CommonTextData txt = new CommonTextData();
            txt.Analysis(SingleConversation[i], i, error);
            dataList.Add(txt);
        }
        return dataList;

    }
}