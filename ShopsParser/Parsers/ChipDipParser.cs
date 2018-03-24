using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ShopsParser.Parsers
{
    public class ChipDipParser : CommonParser
    {
        const string HOST = "https://www.ru-chipdip.by";
        int page = 1;

        public ChipDipParser(string item) : base(item)
        {
            bool SearchMore = true;
            int page = 1;
            while (SearchMore)
            {
                try
                {
                    HtmlDocument html = new HtmlDocument();
                    if (page == 1)
                        html.LoadHtml(wClient.DownloadString(HOST + "/search?searchtext=" + SearchedItem));
                    else
                        html.LoadHtml(wClient.DownloadString(HOST + "/search?searchtext=" + SearchedItem + "&page=" + page.ToString()));
                    ++page;
                    var htmlNodes = html.DocumentNode.SelectNodes("//a").Where(x => x.GetAttributeValue("class", null) == "link group-header");
                    if (htmlNodes.Count() == 0)
                        SearchMore = false;
                    else
                    {
                        foreach (var node in htmlNodes)
                        {
                            string Url = HOST + node.GetAttributeValue("href", null);
                            Url = Url.Replace("https://www.ru-chipdip.by/catalog-show/", "https://www.ru-chipdip.by/catalog/");
                            GetItems(Url);
                        }
                    }
                }
                catch (WebException)
                {
                    SearchMore = false;
                }
            }
        }

        private void GetItems(string Url)
        {
            bool SearchMore = true;
            int page = 1;
            while (SearchMore)
            {
                try
                {
                    HtmlDocument html = new HtmlDocument();
                    if (page == 1)
                        html.LoadHtml(wClient.DownloadString(Url));
                    else
                        html.LoadHtml(wClient.DownloadString(Url + "?page=" + page.ToString()));
                    ++page;
                    var htmlNodes = html.DocumentNode.SelectNodes("//tr").Where(x => x.GetAttributeValue("class", null) == "with-hover");
                    if (htmlNodes.Count() == 0)
                        SearchMore = false;
                    else
                    {
                        ParsePage(htmlNodes);
                    }
                }
                catch (WebException)
                {
                    SearchMore = false;
                }
            }
        }

        private void ParsePage(IEnumerable<HtmlNode> htmlNodes)
        {
            foreach (HtmlNode node in htmlNodes)
            {
                GetNewItem(node);
            }
        }

        private HtmlNode GetNameAndLinkTag(HtmlNode node)
        {
            var tag = node.SelectNodes(".//td").Where(x => x.GetAttributeValue("class", null) == "h_name").First();
            var NameDiv = tag.SelectNodes(".//div").Where(x => x.GetAttributeValue("class", null) == "name");
            return NameDiv.Count() > 0 ? GetCommonItemTag(NameDiv) : GetPromoItemTag(NameDiv, tag);
        }

        private HtmlNode GetCommonItemTag(IEnumerable<HtmlNode> NameDiv)
        {
            return NameDiv.First().SelectSingleNode(".//a");
        }

        private HtmlNode GetPromoItemTag(IEnumerable<HtmlNode> NameDiv, HtmlNode tag)
        {
            NameDiv = tag.SelectNodes(".//div").Where(x => x.GetAttributeValue("class", null) == "name sprite_b item_promo");
            return NameDiv.First().SelectSingleNode(".//a");
        }

        protected override string GetName(HtmlNode node)
        {
            return GetNameAndLinkTag(node).InnerText;
        }

        protected override string GetLink(HtmlNode node)
        {
            return (HOST + GetNameAndLinkTag(node).GetAttributeValue("href", null));
        }

        protected override string GetPicture(HtmlNode node)
        {
            var tag = node.SelectNodes(".//td").Where(x => x.GetAttributeValue("class", null) == "img").First();
            var img = tag.SelectSingleNode(".//div/span/img");
            if (img == null)
                return NO_IMAGE;
            else
                return img.GetAttributeValue("src", NO_IMAGE);
        }

        protected override string GetAvailability(HtmlNode node)
        {
            var tag = node.SelectNodes(".//td").Where(x => x.GetAttributeValue("class", null) == "h_av nw").First();
            if (tag.SelectSingleNode(".//div/span").GetAttributeValue("class", null) == "item__avail_available")
                return "Available";
            else
                return "Not available";
        }

        protected override string GetCost(HtmlNode node)
        {
            var tag = node.SelectNodes(".//td").Where(x => x.GetAttributeValue("class", null) == "h_pr nw").First();
            return tag.SelectSingleNode(".//span").InnerText;
        }
    }
}