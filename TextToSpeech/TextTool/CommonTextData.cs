using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[Serializable]
public class CommonTextData : BaseTextData
{

    public override string PlayerID => name;

    public override void Analysis(string paragraphText, int Serial, Action<Exception, string> error)
    {
        string[] sp = Regex.Split(paragraphText, "(?<=.+([0-9]+))");

        id = Regex.Match(paragraphText, ".+([0-9]+)").Value;
        name = Regex.Match(id, "(?=\\()").Value;//匹配大括号


    }
}

