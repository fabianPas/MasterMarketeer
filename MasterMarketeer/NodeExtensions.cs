using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsAccessBridgeInterop;

namespace MasterMarketeer
{
    public static class NodeExtensions
    {
        public static string GetValue(this AccessibleNode node)
        {
            var firstOrDefault = node.GetProperties(PropertyOptions.AccessibleContextInfo).FirstOrDefault(x => x.Name == "Name");
            if (firstOrDefault != null)
                return (string)firstOrDefault.Value;

            return null;
        }

        public static int GetInteger(this AccessibleNode node)
        {
            var value = node.GetValue();
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            if (value == ">1000")
                return 1000;

            return int.Parse(value);
        }
    }
}
