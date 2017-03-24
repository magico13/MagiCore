using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MagiCore
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class EventBehaviour : MonoBehaviour
    {
        //Called when variables are about to be replaced, so other mods can also replace variables
        public static EventData<string, Dictionary<string, string>> onMCVariableReplacing;

        private void Start()
        {
            onMCVariableReplacing = new EventData<string, Dictionary<string, string>>(nameof(onMCVariableReplacing));
        }
    }
}
