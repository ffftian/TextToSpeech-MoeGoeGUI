using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


abstract public class BaseTextData
{
    public string SaveID
    {
        get
        {
            return Regex.Replace(textId, MiaoRegexTool.路径非法字符, "");
        }
    }
    /// <summary>
    /// 文本所在组ID，还是拿这个当序号好了
    /// </summary>
    abstract public string GroupID { get; }
    /// <summary>
    /// 文本唯一ID
    /// </summary>
    public string textId;
    /// <summary>
    /// 说话角色
    /// </summary>
    public string name;
#if UNITY_EDITOR
    [UnityEngine.TextArea]
#endif
    public string log;

    abstract public void Analysis(string paragraphText, int Serial, Action<Exception, string> error);

}

