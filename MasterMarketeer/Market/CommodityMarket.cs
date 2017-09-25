using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace MasterMarketeer.Market
{
    [Csv(CsvBehavior.FirstEnumerable)]
    public class CommodityMarket
    {
        /// <summary>
        /// The island of the commodity market
        /// </summary>
        public string Island { get; set; }

        /// <summary>
        /// The market rows
        /// </summary>
        public List<MarketRow> Rows { get; set; }
    }
}
