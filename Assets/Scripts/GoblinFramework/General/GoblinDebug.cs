using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General
{
    public static class GoblinDebug
    {
        public static void Log(string message) 
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(message);
#else
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
#endif
        }

        public static void LogError(string message) 
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(message);
#else
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
            System.Diagnostics.Debug.Assert(false);
#endif
        }
    }
}
