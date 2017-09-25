using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsAccessBridgeInterop;

namespace MasterMarketeer
{
    public partial class MainWindow : Form
    {
        private AccessBridge _accessBridge;
        private string _commodityMarket;

        public MainWindow()
        {
            InitializeComponent();

            selectedCommodityMarket.SelectedIndexChanged += (sender, args) =>
            {
                _commodityMarket = selectedCommodityMarket.Text;
            };

            collectData.Click += (sender, args) =>
            {
                if (!collectData.Enabled)
                    return;

                if (string.IsNullOrWhiteSpace(_commodityMarket))
                    return;

                collectData.Enabled = false;

                var collector = new CommodityCollector(_accessBridge, _commodityMarket);
                collector.OnFinishCollecting += (o, eventArgs) =>
                {
                    collectData.Enabled = true;
                };

                collector.Collect();
            };
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            _accessBridge = new AccessBridge();
            _accessBridge.Initialize();
        }
    }
}
