using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public class TextData
{
    public string ID;
    public string QQ;
    public string RoleName;
    public string Log;
    public string Time;
    public void SetTextData(string ALLmessage)
    {

        //string[] SingleConversation = Regex.Replace(message, "\\r\\n\\r\\n", RegexOptions.IgnoreCase);//靠非正则搞定了
        Time = Regex.Match(ALLmessage, "\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2} ").Value;//单独提取时间
        Match QQName = Regex.Match(ALLmessage, "[\u4E00-\u9FA5A-Za-z0-9_⑨]+\\([0-9]+\\)");

        RoleName = QQName.Value.Split('(')[0];//提取用户名

        QQ = QQName.Value.Split('(')[1].Split(')')[0];


        string[] SingleConversation = Regex.Split(ALLmessage, "\\r\\n", RegexOptions.IgnoreCase);

        for (int a = 0; a < SingleConversation.Length; a++)
        {
            if (a == 0)//捕获第一行
            {
                ID = SingleConversation[a];
            }

            if (a != 0)
            {
                Log += SingleConversation[a];
            }

        }
    }
}

