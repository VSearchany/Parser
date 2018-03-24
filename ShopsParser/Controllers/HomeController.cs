using ShopsParser.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using ShopsParser.Parsers;

namespace ShopsParser.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Search(string SearchedItem)
        {
            if (SearchedItem != null)
            {
                CommonParser belChipParser = new BelChipParser(SearchedItem);
                CommonParser chipDipParser = new ChipDipParser(SearchedItem);
                List<RadioItem> Items = new List<RadioItem>();
                Items.AddRange(belChipParser.Items);
                Items.AddRange(chipDipParser.Items);
                return View(Items);
            }
            return View("Index");
        }
    }
}