using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;


  static  class TextTool
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
            //MatchCollection


    }

    public static void LoadText(string Path)
    {
        string str = File.ReadAllText(Path,Encoding.UTF8);


    }




     




    //string a = "2019 - 07 - 31 20:09:43 ftianf(790947404)伊斯科 / 约翰 你们迷糊的醒来，本能的坐了起来，发现自己身处于不明金属制成的舱内，舱内还微微冒着白色烟，你们感到寒冷，注意到全身裸体，因寒冷不想在这舱内多呆一步遍本能的爬了出来";

    //MatchCollection MatchSpilit = Regex.Matches(Text, "[\u4e00-\u9fa5_a-zA-Z0-9]*\n");
    // MatchCollection MatchSpilit = Regex.Matches(Text, "[\\w-:()/ ]*");
    // MatchCollection MatchSpilit = Regex.Matches(Text, "((?!hello).)+$/");
    // MatchCollection MatchSpilit = Regex.Matches(text, "[\\w-:()/，。 ]+");

    // MatchCollection MatchSpilit = Regex.Matches(text, "[\\w-:()/，。 ]+(?=\\n\\r\\n)");
    //MatchCollection MatchSpilit = Regex.Matches(text, "(?<!\\r\\n\\r\\n)[\\w-:()/，。 ]+");
    //MatchCollection MatchSpilit = Regex.Matches(text, "(?<=\\r\\n\\r\\n)[\\w-:()/，。 ]+");


    //    foreach (Match match in MatchSpilit)
    //    {
    //        Debug.Log(match.Value);
    //    }


    //    foreach (string g in G)
    //    {
    //        Debug.Log(g);
    //    }
    }

