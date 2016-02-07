using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagiCore
{
    public class ConfigNodeUtils
    {
        public static string GetValueOrDefault(ConfigNode node, string value, string def = "")
        {
            if (node.HasValue(value))
                return node.GetValue(value);
            return def;
        }

        public static void ReplaceValuesInNode(ConfigNode source, Dictionary<string, string> variables)
        {
            foreach (ConfigNode.Value val in source.values)
            {
                val.value = MathParsing.ReplaceMathVariables(val.value, variables);
            }

            for (int i = 0; i < source.nodes.Count; i++)
            {
                ReplaceValuesInNode(source.nodes[i], variables); //recurse through all attached nodes
            }
        }
    }
}
