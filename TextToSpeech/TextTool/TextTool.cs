using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

public static class TextTool
{
    public static List<BaseTextData> LogSplit(string text, Action<Exception, string> error)
    {
        if (IsQQLog(text))
        {
            return QQLogSplit<BaseTextData>(text, error).ToList();
            //return QQLogSplit(text, error);
        }
        else if (IsCommonLog(text))
        {
            return CommonLogSplit<BaseTextData>(text, error).ToList();
        }
        return null;
    }

    public static bool IsQQLog(string text)
    {
        return Regex.IsMatch(text, "^(?=\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2})");
    }
    public static bool IsCommonLog(string text)
    {
        return Regex.IsMatch(text, "^(?=.+([\u4e00-\u9fa5_a-zA-Z0-9]+))");
        //return Regex.IsMatch(text, "^(?=.+([0-9]+))");
    }
    public static IEnumerable<T> QQLogSplit<T>(string text, Action<Exception, string> error) where T : BaseTextData
    {
        string[] SingleConversation = Regex.Split(text, "(?=\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2})");
        //List<BaseTextData> dataList = new List<BaseTextData>();
        for (int i = 1; i < SingleConversation.Length; i++)//略过第一行，第一行固定为空。
        {
            QQTextData txt = new QQTextData();
            txt.Analysis(SingleConversation[i], i, error);
            yield return txt as T;
        }
    }

    public static IEnumerable<T> CommonLogSplit<T>(string text, Action<Exception, string> error) where T : BaseTextData
    {
        string[] SingleConversation = Regex.Split(text, "\\r\\n\\r\\n");
        //string[] SingleConversation = Regex.Split(text, "(?=.?\\([0-9]+\\))");

        for (int i = 0; i < SingleConversation.Length; i++)
        {
            CommonTextData txt = new CommonTextData();
            txt.Analysis(SingleConversation[i], i, error);
            yield return txt as T;
        }

    }
}