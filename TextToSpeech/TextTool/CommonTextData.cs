using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[Serializable]
public class CommonTextData : BaseTextData
{
    public override string GroupID => name;

    public override void Analysis(string paragraphText, int Serial, Action<Exception, string> error)
    {
        string[] singleConversation = Regex.Split(paragraphText, "\\r\\n");
        for (int i = 0; i < singleConversation.Length; i++)
        {
            switch (i)
            {
                case 0:
                    string[] value=  Regex.Split(singleConversation[i], "\\(|\\)", RegexOptions.IgnoreCase);
                    name = value[0];
                    id = value[1];
                    break;
                default:
                    log += singleConversation[i] + "\n";//剩下的为log
                    break;
            }
        }
        //string[] sp = Regex.Split(paragraphText, "(?<=.+([0-9]+))");

        //id = Regex.Match(paragraphText, ".+([0-9]+)").Value;
        //name = Regex.Match(id, "(?=\\()").Value;//匹配大括号


    }
}

