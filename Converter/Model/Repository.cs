using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Converter.Model
{
    public class Repository
    {
        private IList<Currency> currencyList = new BindingList<Currency>();
        private bool isDownloaded = true;
        private bool isNoInfo = false;

        public IList<Currency> GetAllCurrency(string fileName)
        {
            try
            {
                FileInfo sourceFile = new FileInfo(fileName);
                string fileContent = String.Empty;

                if (!sourceFile.Exists || (sourceFile.LastWriteTime - DateTime.Now).Minutes > 240)
                {
                    var wc = new WebClient();
                    wc.DownloadFile("http://www.floatrates.com/feeds.html", fileName);
                }

                StreamReader streamReader = new StreamReader("Source.html");
                fileContent = streamReader.ReadToEnd();
                streamReader.Close();

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(fileContent);

                var res = doc.GetElementbyId("pb_1426").SelectNodes("div")[1].SelectNodes("ul/li");

                foreach (var item in res)
                {
                    currencyList.Add(new Currency()
                    {
                        Name = item.InnerText.Substring(0, item.InnerText.LastIndexOf("(")).Trim(),
                        Path = item.FirstChild.Attributes[0].Value,
                        Code = item.InnerText.Substring(item.InnerText.LastIndexOf("(") + 1, 3)
                    });
                }
                return currencyList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task GetFileAsync(Currency cur)
        {
            await Task.Run(() =>
            {
                try
                {
                    FileInfo source = new FileInfo(cur.Code + ".xml");
                    if (!source.Exists || (source.LastWriteTime - DateTime.Now).Minutes > 240 || source.Length == 0)
                    {
                        isDownloaded = false;
                        var wc = new WebClient();
                        wc.DownloadFile(new Uri(cur.Path), cur.Code + ".xml");
                        isDownloaded = true;
                        isNoInfo = false;
                    }
                }
                catch (WebException)
                {
                    isDownloaded = true;
                    isNoInfo = true;
                    throw new Exception($"There is no information for {cur.Code}");
                }
                catch (Exception)
                {
                    isDownloaded = true;
                    throw;
                }
                isNoInfo = false;
            });
        }

        public async Task<double> GetRateAsync(Currency curFrom, Currency curTo)
        {
            
            double res = 0.0;
            await Task.Run(() =>
            {
                if (isNoInfo)
                {
                    throw new Exception($"There is no information for {curFrom.Code}");
                }
                while (!isDownloaded) { }
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(curFrom.Code + ".xml");
                    XmlElement xRoot = document.DocumentElement;

                    XmlNode childNode = xRoot.SelectSingleNode($"item[targetCurrency='{curTo.Code}']");
                    if (childNode is null)
                    {
                        throw new Exception($"There is no updated information for the target currency {curTo.Code}");
                    }
                    var node = childNode.SelectSingleNode("exchangeRate");
                    res = double.Parse(node.InnerText, CultureInfo.InvariantCulture);
                }
                catch (XmlException)
                {
                    throw new Exception($"There is no updated information for the source currency {curFrom.Code}. Try later");
                }
                catch (Exception)
                {
                    throw;
                }
            });
            return res;
        }
    }
}
