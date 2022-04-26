using System;
using System.IO;
using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;

namespace SanctionScannerTask
{
    class Program
    {
        /*
         * ParseHtml takes a path or a web page URL and returns the nodes of HtmlNodes
         * In this case, class names used statically for the specified e-commerce web-site
         * The project uses HtmlAgilityPack package to parsing Html documents
         */
        public static List<HtmlNode> ParseHtml(string html)
        {
            #region start

            HtmlDocument doc = new HtmlDocument();
            doc.Load(html);
            var links = doc.DocumentNode.Descendants("td")  
                                    .Where(node => node.GetAttributeValue("class", "default")
                                    .Contains("searchResultsGalleryItem")).ToList(); //List of IEnumerable<HtmlNode>'s

            #endregion

            return links;
        }

        /*
         * This function takes html nodes and parsing the value specified classes to gathering values of products
         */
        public static void GetValues(List<HtmlNode> htmlNodes, List<string> list, List<double> prices)
        {
            Console.WriteLine("{0,40} {1,48}\n", "| Product Names |", "| Prices |");
            foreach (var link in htmlNodes)
            {
                var productName = link.SelectSingleNode(".//a[contains(@class,'classifiedTitle')]").GetDirectInnerText().Trim();
                list.Add(productName);

                var priceStr = link.SelectSingleNode(".//div[@class='searchResultsPriceValue']").InnerText.Trim();
                var index = Convert.ToInt32(priceStr.IndexOf(' '));

                double price = double.Parse(priceStr.Substring(0, index));
                prices.Add(price);

                Console.WriteLine("{0,-70} {1, 20:N0}", productName, priceStr + "\n");
            }
            Console.WriteLine("\n\n-------- The Average Price of Product Prices --------");
            Console.WriteLine("\n\t\t" + prices.Average() + " TL");
        }

        static void Main(string[] args)
        {
            //initialization of specified variable types
            var productNames = new List<string>();
            var prices = new List<double>();

            //getting html file from directory
            string htmlFileName = @"vitrin.html";
            string htmlFullName = Path.GetFullPath(htmlFileName, Environment.CurrentDirectory);

            //Parsing html file
            var links = ParseHtml(htmlFullName);

            //getting parsed values
            GetValues(links, productNames, prices);

            //writing to the text file
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter output = new StreamWriter(@"./List.txt");
                output.WriteLine("{0,-20} {1,70}\n","| Product Names |", "| Prices |");

                for(int i = 0; i < prices.Count; i++)
                {
                    output.WriteLine("{0,-70} {1, 20:N0}", productNames[i], prices[i] + " TL\n");
                }

                output.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            Console.ReadKey();
        }
    }
}
