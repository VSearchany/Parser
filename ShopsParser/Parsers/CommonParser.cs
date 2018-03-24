using HtmlAgilityPack;
using ShopsParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShopsParser.Parsers
{
    abstract public class CommonParser
    {
        protected HtmlDocument html;
        protected WebClient wClient;
        public const string NO_IMAGE = "https://www.rcherz.com/images/no-logo.png";
        public List<RadioItem> Items { get; set; }

        protected string SearchedItem { get; set; }

        protected CommonParser(string item)
        {
            SearchedItem = item;
            wClient = SetWebClient();
            html = new HtmlDocument();
            Items = new List<RadioItem>();
        }

        private WebClient SetWebClient()
        {
            WebClient wClient = new WebClient();
            wClient.Proxy = null;
            wClient.Encoding = System.Text.Encoding.GetEncoding("utf-8");
            return wClient;
        }

        protected void GetNewItem(HtmlNode node)
        {
            RadioItem newItem = new RadioItem();
            GetFields(node, newItem);
            Items.Add(newItem);
        }

        protected void GetFields(HtmlNode node, RadioItem newItem)
        {
            newItem.Name = GetName(node);
            newItem.Link = GetLink(node);
            newItem.Picture = GetPicture(node);
            newItem.Availability = GetAvailability(node);
            newItem.Cost = GetCost(node);
        }

        protected abstract string GetName(HtmlNode node);

        protected abstract string GetLink(HtmlNode node);

        protected abstract string GetPicture(HtmlNode node);

        protected abstract string GetAvailability(HtmlNode node);

        protected abstract string GetCost(HtmlNode node);
    }
}
