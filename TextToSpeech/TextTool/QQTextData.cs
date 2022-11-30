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
[Serializable]
public class QQTextData : BaseTextData
{
    public override string GroupID => qq;
    public string qq;
#if UNITY_EDITOR
    [UnityEngine.TextArea]
#endif
    //public string log;
    public string time;
    /// <summary>
    /// 提取通过txt文本读取的正则中的格式
    /// </summary>
    /// <param name="message"></param>
    ///  <param name="Serial"=debug用，用于标识序号></param>
    override public void Analysis(string SingleText, int Serial, Action<Exception, string> error)
    {
        try
        {
            time = Regex.Match(SingleText, "\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2} ").Value;//提取时间
            Match QQAndName = Regex.Match(SingleText, "(?<=(\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2} )).+\\([0-9]+\\)");//用0宽断言提取名称
            //Match QQAndName = Regex.Match(SingleText, "[\u4E00-\u9FA5A-Za-z0-9_⑨]+\\([0-9]+\\)");
            if (QQAndName.Length == 0)
            {
                QQAndName = Regex.Match(SingleText, "\\([0-9]+\\)");//当没有用户名时，只提取QQ号
            }
            name = QQAndName.Value.Split('(')[0];//提取用户名 

            qq = QQAndName.Value?.Split('(')[1].Split(')')[0];//提取QQ号

            string[] singleConversation = Regex.Split(SingleText, "\\r\\n", RegexOptions.IgnoreCase);//QQ消息分开
            for (int i = 0; i < singleConversation.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        textId = singleConversation[i];//第一行为QQ消息ID
                        break;
                    default:
                        log += singleConversation[i] + "\n";//剩下的为log
                        break;
                }
            }
        }
        catch (Exception e)
        {
            error.Invoke(e, $"错误的读取，序号{Serial}，输出原句:{SingleText}");
        }
    }
}

