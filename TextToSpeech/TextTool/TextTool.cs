using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;


public static class TextTool
{
    public static List<BaseTextData> LogSplit (string text, Action<Exception, string> error)
    {
        if(IsQQLog(text))
        {
            return QQLogSplit(text,error);
        }
        else if(IsCommonLog(text))
        {
            return CommonLogSplit(text, error);
        }
        return null;
    }

    public static bool IsQQLog(string text)
    {
        return Regex.IsMatch(text, "^(?=\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2})");
    }
    public static bool IsCommonLog(string text)
    {
        return Regex.IsMatch(text, "^(?=.+([0-9]+))");
    }

    public static List<BaseTextData> QQLogSplit(string text, Action<Exception,string> error)
    {
        string[] SingleConversation = Regex.Split(text, "(?=\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2})");//这个靠零宽正向先行断言匹配了每一句话前是否是日期格式，问题在于必须跳过第一格。
        List<BaseTextData> dataList = new List<BaseTextData>();
        for (int i = 1; i < SingleConversation.Length; i++)//略过第一行，第一行固定为空。
        {
            QQTextData txt = new QQTextData();
            txt.Analysis(SingleConversation[i], i, error);
            dataList.Add(txt);
        }
        return dataList;
    }
    public static List<BaseTextData> CommonLogSplit(string text, Action<Exception, string> error)
    {
        string[] SingleConversation = Regex.Split(text, "\\r\\n\\r\\n");
        //string[] SingleConversation = Regex.Split(text, "(?=.?\\([0-9]+\\))");

        List<BaseTextData> dataList = new List<BaseTextData>();
        for (int i = 1; i < SingleConversation.Length; i++)//略过第一行，第一行固定为空。
        {
            CommonTextData txt = new CommonTextData();
            txt.Analysis(SingleConversation[i], i, error);
            dataList.Add(txt);
        }
        return dataList;

    }
}