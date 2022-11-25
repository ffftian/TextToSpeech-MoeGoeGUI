using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class SpeakerText
{
    public string SpeakerTextData;
    public string EmotionPath;
    [JsonIgnore]
    public string EmotionSubName
    {
        get
        {
            return System.IO.Path.GetFileName(EmotionPath);
        }
    }

}

