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

        //left these in just incase being used in future / something else.
        // SendMessage overload.
        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(int hWnd, int Msg, int wParam, int lParam);

        // SendMessage overload.
        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(int hWnd, int Msg, int wParam, StringBuilder lParam);


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


        public void Collect()
        {
            //fetch process handle by matching window title -- can be made more specific to avoid collision with webbrowsers. 
            Process[] processes = Process.GetProcesses();
            foreach (Process proc in processes)
            {

                if (proc.MainWindowTitle.Contains("Puzzle Pirates") && proc.MainWindowTitle.Contains("ocean"))
                {
                    _windowHandle = proc.MainWindowHandle;
                    break;
                }
            }

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

             bool isInventory = false;
            var parent = _commodityNode.GetParent().GetParent();
            if(FindNodePropertyValue(parent, "Role") == "scroll pane")
            {
                //get child in scrollpane; should be vp for labels!
                
                if(((AccessibleContextNode)parent).GetInfo().childrenCount >= 4)
                {
                    var vpChildren = ((AccessibleContextNode)parent).FetchChildNode(3);
                    if(FindNodePropertyValue(vpChildren, "Role") == "viewport")
                    {
                        var panel = ((AccessibleContextNode)vpChildren).FetchChildNode(0);
                        if (FindNodePropertyValue(panel, "Role") == "panel" && ((AccessibleContextNode)panel).GetInfo().childrenCount >= 7 )
                        {
                            var label = ((AccessibleContextNode)panel).FetchChildNode(6);
                            string name = FindNodePropertyValue(label, "Name");
                            if (FindNodePropertyValue(label, "Name") == "Ye hold")
                            {
                                isInventory = true;
                            }
                        }
                    }
                }
            }
             
            int incr = 6;
            if (isInventory)
                incr = 7;
            
            JavaObjectHandle jHandle = _commodityNode.AccessibleContextHandle;
            int JvmId = jHandle.JvmId;
            
            int m = _commodityNode.GetInfo().childrenCount - incr;
            for (int i = 0; i < m; i+= incr)
            {

                commodityMarket.Rows.Add(new MarketRow()
                {
                    Commodity = GetValueOfName(_accessBridge.Functions.GetAccessibleChildFromContext(JvmId, jHandle, i)),
                    Outlet = GetValueOfName(_accessBridge.Functions.GetAccessibleChildFromContext(JvmId, jHandle, i+1)),
                    BuyPrice = GetIntValueOfName(_accessBridge.Functions.GetAccessibleChildFromContext(JvmId, jHandle, i+2)),
                    WillBuy = GetIntValueOfName(_accessBridge.Functions.GetAccessibleChildFromContext(JvmId, jHandle, i + 3)),
                    SellPrice = GetIntValueOfName(_accessBridge.Functions.GetAccessibleChildFromContext(JvmId, jHandle, i + 4)),
                    WillSell = GetIntValueOfName(_accessBridge.Functions.GetAccessibleChildFromContext(JvmId, jHandle, i + 5)),
                    //ye hold would be i+6

                });
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
        private string FindNodePropertyValue(AccessibleNode node, string name)
        {
            var properties = node.GetProperties(PropertyOptions.AccessibleContextInfo);
            foreach (var property in properties)
            {
                if (property.Name == name)
                    return (string)property.Value;
            }
            return null;
            
        }
        private void FindCommodityMarketNode(AccessibleNode node)
        {
            var properties = node.GetProperties(PropertyOptions.AccessibleContextInfo);

            foreach (var property in properties)
            {
                 if (property.Name == "Role" && (string) property.Value == "table" && ((AccessibleContextNode)node).GetInfo().childrenCount > 100)
                    _commodityNode = (AccessibleContextNode)node;
            }

            var children = node.GetChildren();
            foreach (var child in children)
                FindCommodityMarketNode(child);
        }
        public string GetValueOfName(JavaObjectHandle handle)
        {
            AccessibleContextInfo info;
            if (_accessBridge.Functions.GetAccessibleContextInfo(handle.JvmId, handle, out info))
            {
                return info.name;

            }
            return null;
        }

        public string GetValueOfName(AccessibleContextNode node)
        {
            AccessibleContextInfo info;
            if (_accessBridge.Functions.GetAccessibleContextInfo(node.AccessibleContextHandle.JvmId, node.AccessibleContextHandle, out info))
            {
                return info.name;

            }
            return null;
        }

        public int GetIntValueOfName(JavaObjectHandle handle)
        {
            string value = GetValueOfName(handle);
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            if (value == ">1000")
                return 1000;

            return int.Parse(value);
        }
        public int GetIntValueOfName(AccessibleContextNode node)
        {
            string value = GetValueOfName(node);
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            if (value == ">1000")
                return 1000;

            return int.Parse(value);
        }
    }    
}
