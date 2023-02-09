﻿using System;
using System.Globalization;
using System.Threading;

namespace FEZRepacker.Interface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // showoff
            Console.WriteLine(
                "==============================\n" +
                "= FEZRepacker 0.1 by Krzyhau =\n" +
                "==============================\n"
            );

            // keep number decimals consistent
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

            if (args.Length > 0)
            {
                CommandLine.ParseCommandLine(args);
            }
            else
            {
                // run user interface here
            }
        }
    }
}