using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagiCore
{
    public static class ConfigNodeExtensions
    {
        /// <summary>
        /// Gets the value associated with the provided key, or returns the default if not found
        /// </summary>
        /// <param name="node">The node to check</param>
        /// <param name="key">The key to check for</param>
        /// <param name="def">The default value if the key is not found</param>
        /// <returns>The value or default</returns>
        public static string GetValueOrDefault(this ConfigNode node, string key, string def = "")
        {
            if (node.HasValue(key))
                return node.GetValue(key);
            return def;
        }

        /// <summary>
        /// Replaces all instances of the provided variables with their given values
        /// </summary>
        /// <param name="node">The node to act on</param>
        /// <param name="variables">The variables and their values to use for replacement</param>
        public static void ReplaceValues(this ConfigNode node, Dictionary<string, string> variables)
        {
            foreach (ConfigNode.Value val in node.values)
            {
                val.value = MathParsing.ReplaceMathVariables(node.name, val.value, variables); //TODO: Should this use the StringTranslation version instead?
            }

            for (int i = 0; i < node.nodes.Count; i++)
            {
                node.nodes[i].ReplaceValues(variables); //recurse through all attached nodes
            }
        }
    }
}
