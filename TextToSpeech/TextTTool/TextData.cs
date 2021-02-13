using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/// <summary>
/// QQ文本类。
/// </summary>
public class TextData
{
    public string ID;
    public string QQ;
    public string RoleName;
    public string Log;
    public string Time;
    /// <summary>
    /// 提取通过txt文本读取的正则中的格式
    /// </summary>
    /// <param name="message"></param>
    ///  <param name="Serial"=debug用，用于标识序号></param>
    public void Analysis(string SingleText, int Serial)
    {
        try
        {
            Time = Regex.Match(SingleText, "\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2} ").Value;//提取时间
            Match QQAndName = Regex.Match(SingleText, "[\u4E00-\u9FA5A-Za-z0-9_⑨]+\\([0-9]+\\)");
            if (QQAndName.Length == 0)
            {
                QQAndName = Regex.Match(SingleText, "\\([0-9]+\\)");//当没有用户名时，只提取QQ号
            }
            RoleName = QQAndName.Value.Split('(')[0];//提取用户名 

            QQ = QQAndName.Value?.Split('(')[1].Split(')')[0];//提取QQ号

            string[] SingleConversation = Regex.Split(SingleText, "\\r\\n", RegexOptions.IgnoreCase);//QQ消息分开
            for (int a = 0; a < SingleConversation.Length; a++)
            {
                if (a == 0)
                {
                    ID = SingleConversation[a];//第一行为QQ消息ID
                }

                if (a != 0)
                {
                    Log += SingleConversation[a] + "\n";//剩下的为log
                }
            }
        }
        catch
        {
            throw new Exception($"错误的读取，序号{Serial}，输出原句:{SingleText}");
        }
    }
}

