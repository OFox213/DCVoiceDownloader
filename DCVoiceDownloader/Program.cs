using HtmlAgilityPack;
using RestSharp;
using System;
using System.Net;
using System.Text;
using System.Xml;
main();
void main()
{
    Console.WriteLine("CONNECTING HEARTS!\n디시 보플 다운로더 V1.0\nby OFox213, https://github.com/OFox213/DCVoiceDownloader\n\n");
    Console.Write("소스 텍스트 입력 : ");
    string? sourceHtml = Console.ReadLine();
    if (sourceHtml == null || sourceHtml.Length == 0)
    {
        Console.WriteLine("오류가 발생하였습니다.\n소스 텍스트가 html 형식이 아닙니다.");
        main();
    } else
    {
        loadFromSourceHtml(sourceHtml);
    }
}

void loadFromSourceHtml(string sourceHtml)
{
    try
    {
        //보이스리플 소스로부터 링크 가져오기
        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(sourceHtml);
     
        var node1 = htmlDocument.DocumentNode.SelectSingleNode("iframe");
        if (node1 == null)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("HTML 소스 형식이 올바르지 않습니다.");
            Console.ResetColor();
            main();
            return;
        }
        string? sourceSrc = node1.Attributes["src"].Value;

        var client = new RestSharp.RestClient(sourceSrc + "&vr_open=1");
        var request = new RestRequest();
        request.Method = Method.Get;
        request.AddHeader("Origin", "https://dcinside.com");
        var response = client.Execute(request);

        //링크로부터 보이스리플 HTML 가져오기
        HtmlDocument htmlDocument2 = new HtmlDocument();
        htmlDocument2.LoadHtml(response.Content);
        string? audioSourceSrc = htmlDocument2.DocumentNode.SelectSingleNode("//input").Attributes["value"].Value;
        if(!Directory.Exists(Directory.GetCurrentDirectory() + "\\download"))
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\download");
        }
        string path = Directory.GetCurrentDirectory() + "\\download\\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-ff") + ".m4a";

        var client2 = new RestSharp.RestClient(audioSourceSrc);
        var request2 = new RestRequest();
        request2.Method = Method.Get;
        request2.AddHeader("Origin", "https://dcinside.com");
        byte[]? dre=client2.DownloadData(request2);
        if(dre != null)
        {
            File.WriteAllBytes(path, dre);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("다운로드 완료");
            Console.ResetColor();
            Console.WriteLine("저장됨 : " + path);
            Console.WriteLine("\n\n\n");
            main();
        } else
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("다운로드에 실패하였습니다.");
            Console.ResetColor();
            main();
        }

    }
    catch(Exception e)
    {
        Console.Clear();
        Console.WriteLine(e);
    }
}


