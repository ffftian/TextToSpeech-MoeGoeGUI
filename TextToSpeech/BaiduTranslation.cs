using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;

namespace TextToSpeech
{
    public class BaiduTranslation
    {
        static DateTime datetime;
        readonly string To = Languages.日语;
        readonly string ID = "20221015001394333";
        readonly string Key = "SbUmYQ2Q4Y2zPCmO_xZa";
        //readonly TimeSpan Delay;
        const string API_URL = "http://api.fanyi.baidu.com/api/trans/vip/translate";


        ///<summary>
        ///向百度翻译发起一个翻译请求。由于翻译结果会自动按换行分割语句，所以需要从索引器访问完成的翻译。
        ///<br/>
        ///语种可以使用静态类<see cref="Languages"/>中的常量选择
        ///</summary>
        ///<param name="source">需要翻译的文字，上限5000字符</param>
        ///<param name="from">来源语种，默认为自动检测<inheritdoc cref="Languages" path="/remarks"/></param>
        ///<returns>此次请求的错误码。如果没有错则为null
        ///<list type="table"> 
        ///		<item>
        ///			<term>52000</term>
        ///			<description>成功。</description>
        ///		</item>
        ///		<item>
        ///			<term>52001</term>
        ///			<description>请求超时。请重试</description>
        ///		</item>
        ///		<item>
        ///			<term>52002</term>
        ///			<description>系统错误。请重试</description>
        ///		</item>
        ///		<item>
        ///			<term>52003</term>
        ///			<description>未授权用户。请检查appid是否正确或者服务是否开通</description>
        ///		</item>
        ///		<item>
        ///			<term>54000</term>
        ///			<description>必填参数为空。请检查是否少传参数</description>
        ///		</item>
        ///		<item>
        ///			<term>54001</term>
        ///			<description>签名错误。请检查您的签名生成方法</description>
        ///		</item>
        ///		<item>
        ///			<term>54003</term>
        ///			<description>访问频率受限。请降低您的调用频率，或进行身份认证后切换为高级版/尊享版</description>
        ///		</item>
        ///		<item>
        ///			<term>54004</term>
        ///			<description>账户余额不足。请前往<see href ="https://api.fanyi.baidu.com/api/trans/product/desktop">管理控制台</see>为账户充值</description>
        ///		</item>
        ///		<item>
        ///			<term>54005</term>
        ///			<description>长query请求频繁。请降低长query的发送频率，3s后再试</description>
        ///		</item>
        ///		<item>
        ///			<term>58000</term>
        ///			<description>客户端IP非法。检查个人资料里填写的IP地址是否正确，可前往<see href ="https://api.fanyi.baidu.com/access/0/3">开发者信息-基本信息</see> 修改 </description>
        ///		</item>
        ///		<item>
        ///			<term>58001</term>
        ///			<description>译文语言方向不支持。检查译文语言是否在语言列表里</description>
        ///		</item>
        ///		<item>
        ///			<term>58002</term>
        ///			<description>服务当前已关闭。请前往<see href ="https://api.fanyi.baidu.com/choose">管理控制台</see>开启服务</description>
        ///		</item>
        ///		<item>
        ///			<term>90107</term>
        ///			<description>认证未通过或未生效。请前往<see href ="https://api.fanyi.baidu.com/myIdentify">我的认证</see>查看认证进度</description>
        ///		</item>
        ///</list>
        ///</returns>  
        public  string GetTranslation(string source, string from = Languages.中文_简体)
        { 
            var hc = new HttpClient();
            var utf8 = HttpUtility.UrlEncode(source, System.Text.Encoding.UTF8);
            var randomNum = DateTime.Now.Millisecond.ToString();


            var md5 = new MD5CryptoServiceProvider();
            var result = Encoding.UTF8.GetBytes(ID + source + randomNum + Key);
            var output = md5.ComputeHash(result);
            var md5Sign = BitConverter.ToString(output).Replace("-", "").ToLower();


            //var md5Sign = new string(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(ID + source + randomNum + Key)) ;
            var url = $"{API_URL}?q={utf8}&from={from}&to={To}&appid={ID}&salt={randomNum}&sign={md5Sign}";
            //if (DateTime.Now - datetime < Delay)
            //    Task.Delay(datetime + Delay - DateTime.Now).Wait();
            datetime = DateTime.Now;
            //var s = hc.GetStringAsync(url).Result;
            var data =  JsonConvert.DeserializeObject<JObject>(hc.GetStringAsync(url).Result);

            string text = "";

            foreach(JObject obj  in data["trans_result"].Children())
            {
                text += $"{obj["dst"]}\n";
            }
            
            //JsonData datas = JsonMapper.ToObject(s);
            // JsonData res = datas["trans_result"][0]["dst"];
            return text;
            //return res.ToString();

            //var r = System.Text.Json.JsonSerializer.Deserialize<TranslationResult>(s);

            //if (r.error_code != null)

            //    return r.error_code;
            //var d = r.trans_result.ToDictionary(t => t.src, t => t.dst);
            //if (dic.ContainsKey(r.from))
            //    dic[r.from].Add(d);
            //else
            //    dic[r.from] = new Result(d);
            //return null;
        }
        struct TranslationResult
        {
            /// <summary>
            /// 错误码
            /// </summary>
            public string error_code { get; set; }
            /// <summary>
            /// 源语种
            /// </summary>
            public string from { get; set; }
            /// <summary>
            /// 目标语种
            /// </summary>
            public string to { get; set; }
            /// <summary>
            /// 翻译结果（按段落分为数组）
            /// </summary> 
            public TranslationParagraph[] trans_result { get; set; }
            /// <summary>
            /// 每个段落的翻译
            /// </summary> 
            public struct TranslationParagraph
            {
                /// <summary>
                /// 源字符串
                /// </summary>
                public string src { get; set; }
                /// <summary>
                /// 翻译后的字符串
                /// </summary>
                public string dst { get; set; }
            }
        }
        /// <summary>
        /// 完整语种列表 
        /// </summary>
        /// <remarks> 
        /// 常用语种有：
        /// <list type="table">
        /// <item><term>中文</term><description>zh</description></item>
        /// <item><term>英语</term><description>en</description></item>
        /// <item><term>日语</term><description>jp</description></item>
        /// <item><term>韩语</term><description>kor</description></item> 
        /// <item><term>法语</term><description>fra</description></item>
        /// <item><term>俄语</term><description>ru</description></item>
        /// <item><term>德语</term><description>de</description></item> 
        /// <item><term>文言文</term><description>wyw</description></item>
        /// </list>
        /// </remarks>
        public static class Languages
        {
            public const string 自动检测 = "auto";
            public const string 阿拉伯语 = "ara";
            public const string 爱尔兰语 = "gle";
            public const string 奥克语 = "oci";
            public const string 阿尔巴尼亚语 = "alb";
            public const string 阿尔及利亚阿拉伯语 = "arq";
            public const string 阿肯语 = "aka";
            public const string 阿拉贡语 = "arg";
            public const string 阿姆哈拉语 = "amh";
            public const string 阿萨姆语 = "asm";
            public const string 艾马拉语 = "aym";
            public const string 阿塞拜疆语 = "aze";
            public const string 阿斯图里亚斯语 = "ast";
            public const string 奥塞梯语 = "oss";
            public const string 爱沙尼亚语 = "est";
            public const string 奥杰布瓦语 = "oji";
            public const string 奥里亚语 = "ori";
            public const string 奥罗莫语 = "orm";
            public const string 波兰语 = "pl";
            public const string 波斯语 = "per";
            public const string 布列塔尼语 = "bre";
            public const string 巴什基尔语 = "bak";
            public const string 巴斯克语 = "baq";
            public const string 巴西葡萄牙语 = "pot";
            public const string 白俄罗斯语 = "bel";
            public const string 柏柏尔语 = "ber";
            public const string 邦板牙语 = "pam";
            public const string 保加利亚语 = "bul";
            public const string 北方萨米语 = "sme";
            public const string 北索托语 = "ped";
            public const string 本巴语 = "bem";
            public const string 比林语 = "bli";
            public const string 比斯拉马语 = "bis";
            public const string 俾路支语 = "bal";
            public const string 冰岛语 = "ice";
            public const string 波斯尼亚语 = "bos";
            public const string 博杰普尔语 = "bho";
            public const string 楚瓦什语 = "chv";
            public const string 聪加语 = "tso";
            public const string 丹麦语 = "dan";
            public const string 德语 = "de";
            public const string 鞑靼语 = "tat";
            public const string 掸语 = "sha";
            public const string 德顿语 = "tet";
            public const string 迪维希语 = "div";
            public const string 低地德语 = "log";
            public const string 俄语 = "ru";
            public const string 法语 = "fra";
            public const string 菲律宾语 = "fil";
            public const string 芬兰语 = "fin";
            public const string 梵语 = "san";
            public const string 弗留利语 = "fri";
            public const string 富拉尼语 = "ful";
            public const string 法罗语 = "fao";
            public const string 盖尔语 = "gla";
            public const string 刚果语 = "kon";
            public const string 高地索布语 = "ups";
            public const string 高棉语 = "hkm";
            public const string 格陵兰语 = "kal";
            public const string 格鲁吉亚语 = "geo";
            public const string 古吉拉特语 = "guj";
            public const string 古希腊语 = "gra";
            public const string 古英语 = "eno";
            public const string 瓜拉尼语 = "grn";
            public const string 韩语 = "kor";
            public const string 荷兰语 = "nl";
            public const string 胡帕语 = "hup";
            public const string 哈卡钦语 = "hak";
            public const string 海地语 = "ht";
            public const string 黑山语 = "mot";
            public const string 豪萨语 = "hau";
            public const string 吉尔吉斯语 = "kir";
            public const string 加利西亚语 = "glg";
            public const string 加拿大法语 = "frn";
            public const string 加泰罗尼亚语 = "cat";
            public const string 捷克语 = "cs";
            public const string 卡拜尔语 = "kab";
            public const string 卡纳达语 = "kan";
            public const string 卡努里语 = "kau";
            public const string 卡舒比语 = "kah";
            public const string 康瓦尔语 = "cor";
            public const string 科萨语 = "xho";
            public const string 科西嘉语 = "cos";
            public const string 克里克语 = "cre";
            public const string 克里米亚鞑靼语 = "cri";
            public const string 克林贡语 = "kli";
            public const string 克罗地亚语 = "hrv";
            public const string 克丘亚语 = "que";
            public const string 克什米尔语 = "kas";
            public const string 孔卡尼语 = "kok";
            public const string 库尔德语 = "kur";
            public const string 拉丁语 = "lat";
            public const string 老挝语 = "lao";
            public const string 罗马尼亚语 = "rom";
            public const string 拉特加莱语 = "lag";
            public const string 拉脱维亚语 = "lav";
            public const string 林堡语 = "lim";
            public const string 林加拉语 = "lin";
            public const string 卢干达语 = "lug";
            public const string 卢森堡语 = "ltz";
            public const string 卢森尼亚语 = "ruy";
            public const string 卢旺达语 = "kin";
            public const string 立陶宛语 = "lit";
            public const string 罗曼什语 = "roh";
            public const string 罗姆语 = "ro";
            public const string 逻辑语 = "loj";
            public const string 马来语 = "may";
            public const string 缅甸语 = "bur";
            public const string 马拉地语 = "mar";
            public const string 马拉加斯语 = "mg";
            public const string 马拉雅拉姆语 = "mal";
            public const string 马其顿语 = "mac";
            public const string 马绍尔语 = "mah";
            public const string 迈蒂利语 = "mai";
            public const string 曼克斯语 = "glv";
            public const string 毛里求斯克里奥尔语 = "mau";
            public const string 毛利语 = "mao";
            public const string 孟加拉语 = "ben";
            public const string 马耳他语 = "mlt";
            public const string 苗语 = "hmn";
            public const string 挪威语 = "nor";
            public const string 那不勒斯语 = "nea";
            public const string 南恩德贝莱语 = "nbl";
            public const string 南非荷兰语 = "afr";
            public const string 南索托语 = "sot";
            public const string 尼泊尔语 = "nep";
            public const string 葡萄牙语 = "pt";
            public const string 旁遮普语 = "pan";
            public const string 帕皮阿门托语 = "pap";
            public const string 普什图语 = "pus";
            public const string 齐切瓦语 = "nya";
            public const string 契维语 = "twi";
            public const string 切罗基语 = "chr";
            public const string 日语 = "jp";
            public const string 瑞典语 = "swe";
            public const string 萨丁尼亚语 = "srd";
            public const string 萨摩亚语 = "sm";
            public const string 塞尔维亚_克罗地亚语 = "sec";
            public const string 塞尔维亚语 = "srp";
            public const string 桑海语 = "sol";
            public const string 僧伽罗语 = "sin";
            public const string 世界语 = "epo";
            public const string 书面挪威语 = "nob";
            public const string 斯洛伐克语 = "sk";
            public const string 斯洛文尼亚语 = "slo";
            public const string 斯瓦希里语 = "swa";
            public const string 塞尔维亚语_西里尔 = "src";
            public const string 索马里语 = "som";
            public const string 泰语 = "th";
            public const string 土耳其语 = "tr";
            public const string 塔吉克语 = "tgk";
            public const string 泰米尔语 = "tam";
            public const string 他加禄语 = "tgl";
            public const string 提格利尼亚语 = "tir";
            public const string 泰卢固语 = "tel";
            public const string 突尼斯阿拉伯语 = "tua";
            public const string 土库曼语 = "tuk";
            public const string 乌克兰语 = "ukr";
            public const string 瓦隆语 = "wln";
            public const string 威尔士语 = "wel";
            public const string 文达语 = "ven";
            public const string 沃洛夫语 = "wol";
            public const string 乌尔都语 = "urd";
            public const string 西班牙语 = "spa";
            public const string 希伯来语 = "heb";
            public const string 希腊语 = "el";
            public const string 匈牙利语 = "hu";
            public const string 西弗里斯语 = "fry";
            public const string 西里西亚语 = "sil";
            public const string 希利盖农语 = "hil";
            public const string 下索布语 = "los";
            public const string 夏威夷语 = "haw";
            public const string 新挪威语 = "nno";
            public const string 西非书面语 = "nqo";
            public const string 信德语 = "snd";
            public const string 修纳语 = "sna";
            public const string 宿务语 = "ceb";
            public const string 叙利亚语 = "syr";
            public const string 巽他语 = "sun";
            public const string 英语 = "en";
            public const string 印地语 = "hi";
            public const string 印尼语 = "id";
            public const string 意大利语 = "it";
            public const string 越南语 = "vie";
            public const string 意第绪语 = "yid";
            public const string 因特语 = "ina";
            public const string 亚齐语 = "ach";
            public const string 印古什语 = "ing";
            public const string 伊博语 = "ibo";
            public const string 伊多语 = "ido";
            public const string 约鲁巴语 = "yor";
            public const string 亚美尼亚语 = "arm";
            public const string 伊努克提图特语 = "iku";
            public const string 伊朗语 = "ir";
            public const string 中文_简体 = "zh";
            public const string 中文_繁体 = "cht";
            public const string 中文_文言文 = "wyw";
            public const string 中文_粤语 = "yue";
            public const string 扎扎其语 = "zaz";
            public const string 中古法语 = "frm";
            public const string 祖鲁语 = "zul";
            public const string 爪哇语 = "jav";
        }
    }
}
