using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;


static class TextTool
{



    public static List<TextData> QQLogSplit(string text)
    {

        string[] SingleConversation = Regex.Split(text, "\\r\\n\\r\\n", RegexOptions.IgnoreCase);//靠非-正则提取每句话部分

        List<TextData> dataList = new List<TextData>();
        foreach (string Convesation in SingleConversation)
        {
            TextData data = new TextData();
            data.SetTextData(Convesation);
            dataList.Add(data);
        }
        return dataList;

    }

    public static void LoadText(string Path)
    {
        string str = File.ReadAllText(Path, Encoding.UTF8);


    }
}

