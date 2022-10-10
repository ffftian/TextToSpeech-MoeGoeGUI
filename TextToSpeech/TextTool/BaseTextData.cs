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
            return Regex.Replace(id, MiaoRegexTool.路径非法字符, "");
        }
    }
    abstract public string GroupID { get; }
    public string id;
    public string name;
#if UNITY_EDITOR
    [TextArea]
#endif
    public string log;

    abstract public void Analysis(string paragraphText, int Serial, Action<Exception, string> error);

}

