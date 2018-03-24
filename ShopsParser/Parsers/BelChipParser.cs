using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ShopsParser.Parsers
{
    public class BelChipParser : CommonParser
    {
        const string HOST = "http://belchip.by";
        private IEnumerable<HtmlNode> htmlNodes;

        public BelChipParser(string item) : base(item)
        {
            html.LoadHtml(wClient.DownloadString(HOST + "/search/?query=" + SearchedItem));
            GetItems();
        }

        private void GetItems()
        {
            htmlNodes = html.DocumentNode.SelectNodes("//div").Where(x => x.GetAttributeValue("class", null) == "cat-item");
            foreach (HtmlNode node in htmlNodes)
            {
                GetNewItem(node);
            }
        }

        protected override string GetName(HtmlNode node)
        {
            var NameAndLink = node.SelectSingleNode(".//h3/a");
            return NameAndLink.InnerText;
        }

        protected override string GetLink(HtmlNode node)
        {
            var NameAndLink = node.SelectSingleNode(".//h3/a");
            return (HOST + NameAndLink.GetAttributeValue("href", null));
        }

        protected override string GetPicture(HtmlNode node)
        {
            var cat_pic = node.SelectNodes(".//div").Where(x => x.GetAttributeValue("class", null) == "cat-pic").First();
            var pic_a = node.SelectNodes(".//a").Where(x => x.GetAttributeValue("class", null) == "example1").First().GetAttributeValue("href", NO_IMAGE);
            string result = HOST + '/' + pic_a;
            return result;
        }

        protected override string GetAvailability(HtmlNode node)
        {
            HtmlNode tag = GetCostAndAvailabilityTag(node);
            if (tag.InnerText == "цена по запросу")
                return "Not available";
            else
                return "Available";
        }

        protected override string GetCost(HtmlNode node)
        {
            HtmlNode tag = GetCostAndAvailabilityTag(node);
            if (GetAvailability(node) == "Available")
                return SearchCostString(tag);
            else
                return "Цена по запросу";
        }

        private string SearchCostString(HtmlNode node)
        {
            var Cost = node.SelectSingleNode(".//div").InnerText;
            return Cost;
        }

        private HtmlNode GetCostAndAvailabilityTag(HtmlNode node)
        {
            var butt_add = node.SelectNodes(".//div").Where(x => x.GetAttributeValue("class", null) == "butt-add");
            return (butt_add.First().SelectSingleNode(".//span"));
        }
    }
}