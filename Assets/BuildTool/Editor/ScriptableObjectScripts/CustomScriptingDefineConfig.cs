using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildTool
{
    public class CustomScriptingDefineConfig : ScriptableObject
    {
        public List<string> allCustomMacros = new List<string>();

        [Flags]
        public enum CustomScriptingDefine
        {
            ORBIT_GM = 1 << 0,
        }
    }
}
