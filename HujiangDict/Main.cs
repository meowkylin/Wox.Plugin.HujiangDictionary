using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSoup.Select;
using NSoup.Nodes;
using Wox.Plugin;
using Wox.Infrastructure.Http;

namespace HujiangDict
{
    public class Main : IPlugin
    {
        public void Init(PluginInitContext context)
        {
        }
        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();
            const string icoPath = "Images\\HujiangDict.ico";

            if(query.Search.Length == 0 || query.Search.Equals(" "))
            {
                results.Add(new Result
                {
                    Title = "いらっしゃいませ",
                    SubTitle = "(◍•ᴗ•◍) from Meowkylin",
                    IcoPath = icoPath
                });
                return results;
            }

            var html = Http.Get("http://dict.hjenglish.com/jp/jc/" + query.Search).Result;
            Document doc = NSoup.Parse.Parser.Parse(html, "http://dict.hjenglish.com/jp/jc/");
            Elements parts = doc.Select("div.jp_word_comment");

            if(parts == null)
            {
                results.Add(new Result
                {
                    Title = "这个它找不到",
                    SubTitle = " 这锅我不背 ヽ(￣д￣;)ノ ",
                    IcoPath = icoPath
                });
                return results;
            }
            else
            {
                foreach (Element part in parts)
                {
                    var kana = part.Select("span[title=假名]").First.Text();
                    var tone = part.Select("span[title=音调]").First.Text();
                    results.Add(new Result
                    {
                        Title = kana + tone,
                        SubTitle = " (假名 || 音调) ",
                        IcoPath = icoPath
                    });

                    Elements meanings = part.Select("li.flag");
                    foreach (Element meaning in meanings)
                    {
                        var translation = meaning.Children.First.Text();
                        var _example = meaning.Select("div.cmd_sent").First;
                        var example = "";
                        if (_example == null)
                            example = "惯用语";
                        else
                            example = _example.Text();

                        results.Add(new Result
                        {
                            Title = translation,
                            SubTitle = " 例句： " + example,
                            IcoPath = icoPath
                        });
                    }
                }
            }
            return results;

        }
    }
    
}
