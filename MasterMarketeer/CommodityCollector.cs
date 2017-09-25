using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsAccessBridgeInterop;
using MasterMarketeer.Market;
using ServiceStack;

namespace MasterMarketeer
{
    public class CommodityCollector
    {
        // These are two Win32 constants that we'll need, they were explained in an earlier blog.
        const int WM_GETTEXT = 0x000D;
        const int WM_GETTEXTLENGTH = 0x000E;

        // SendMessage overload.
        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(int hWnd, int Msg, int wParam, int lParam);

        // SendMessage overload.
        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(int hWnd, int Msg, int wParam, StringBuilder lParam);

        // The GetClassName function takes a handle as a parameter as well as a StringBuilder
        // and the max capacity of the StringBuilder as parameters. It'll return the windows
        // class name by filling up the StringBuilder - though not any longer than the max capacity.
        // If the class is longer than the max capacity it will simply be cropped. Having a larger
        // capacity than necessary is simply a matter of performance.
        [DllImport("User32.Dll")]
        public static extern void GetClassName(int hWnd, StringBuilder s, int nMaxCount);

        // The EnumWindows function will enumerate all windows in the system. Each window will cause
        // the PCallBack callback function to be called.
        [DllImport("user32.Dll")]
        static extern bool EnumWindows(WindowCallback callback, int lParam);

        /// <summary>
        /// This is the delegate that sets the signature for the callback function of the EnumWindows function.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private delegate bool WindowCallback(int hwnd, int lParam);

        /// <summary>
        /// The access bridge
        /// </summary>
        private readonly AccessBridge _accessBridge;

        /// <summary>
        /// The node containing the commodity market data
        /// </summary>
        private AccessibleContextNode _commodityNode;

        /// <summary>
        /// The window handle
        /// </summary>
        private IntPtr _windowHandle;

        /// <summary>
        /// The current selected commodityMarket
        /// </summary>
        private string _currentIsland;

        /// <summary>
        /// The event that is invoked when collecting data is finished
        /// </summary>
        public event EventHandler OnFinishCollecting;

        public CommodityCollector(AccessBridge accessBridge, string currentIsland)
        {
            _accessBridge = accessBridge;
            _currentIsland = currentIsland;
        }

        private bool EnumerateWindowsCallback(int handle, int lParam)
        {
            // First we'll find the class of the window as that is usually the parameter that narrows our search down the furthest.
            // As classes are usually rather short, a capacity of 256 ought to be plenty.
            var sbClass = new StringBuilder(256);
            GetClassName(handle, sbClass, sbClass.Capacity);

            // As explained in an earlier blog we then get the text of the window.
            var txtLength = SendMessage(handle, WM_GETTEXTLENGTH, 0, 0);
            var sbText = new StringBuilder(txtLength + 1);
            SendMessage(handle, WM_GETTEXT, sbText.Capacity, sbText);

            if (sbText.ToString().Contains("on the Obsidian ocean"))
                _windowHandle = new IntPtr(handle);

            return true;
        }

        public void Collect()
        {
            EnumWindows(EnumerateWindowsCallback, 0);
            if (_windowHandle == null)
                return;

            var window = _accessBridge.CreateAccessibleWindow(_windowHandle);

            var frame = window.GetChildren().First();
            if (frame == null)
                return;

            FindCommodityMarketNode(frame);
            if (_commodityNode == null)
                return;

            

            var commodityMarket = new CommodityMarket();
            commodityMarket.Island = _currentIsland;
            commodityMarket.Rows = new List<MarketRow>();

            for (int i = 0; i < _commodityNode.GetInfo().childrenCount - 6; i = i + 6)
            {
                var row = new MarketRow();

                row.Commodity = _commodityNode.FetchChildNode(i).GetValue();
                row.Outlet = _commodityNode.FetchChildNode(i + 1).GetValue();
                row.BuyPrice = _commodityNode.FetchChildNode(i + 2).GetInteger();
                row.WillBuy = _commodityNode.FetchChildNode(i + 3).GetInteger();
                row.SellPrice = _commodityNode.FetchChildNode(i + 4).GetInteger();
                row.WillSell = _commodityNode.FetchChildNode(i + 5).GetInteger();

                commodityMarket.Rows.Add(row);
            }

            var outputFormat = (string)Properties.Settings.Default["output_format"];
            if (outputFormat == "json")
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + _currentIsland + "-" + DateTime.Now.ToString("MM-dd-yy H mm ss") + ".json";
                var jsonString = commodityMarket.ToJson();
                File.WriteAllText(path, jsonString);
            }
            else if (outputFormat == "csv")
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + _currentIsland + "-" + DateTime.Now.ToString("MM-dd-yy H mm ss") + ".csv";
                var csv = commodityMarket.ToCsv();
                File.WriteAllText(path, csv);
            }

            OnFinishCollecting?.Invoke(this, new EventArgs());
        }

        private void FindCommodityMarketNode(AccessibleNode node)
        {
            var properties = node.GetProperties(PropertyOptions.AccessibleContextInfo);

            foreach (var property in properties)
            {
                if (property.Name == "Role" && (string) property.Value == "table")
                    _commodityNode = (AccessibleContextNode)node;
            }

            var children = node.GetChildren();
            foreach (var child in children)
                FindCommodityMarketNode(child);
        }
    }
}