﻿// Copyright (C) Microsoft. All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.CommandLineUtils;
using System.IO;
using System;

namespace Microsoft.DevSkim.CLI.Commands
{
    public class TestCommand : ICommand
    {
        public static void Configure(CommandLineApplication command)
        {            
            command.Description = "Run tests for rules";
            command.HelpOption("-?|-h|--help");
            
            var locationArgument = command.Argument("[path]",
                                                    "Path to rules");

            command.OnExecute(() => {
                return (new TestCommand(locationArgument.Value)).Run();
            });
        }

        public TestCommand(string path)            
        {
            _path = path;
        }

        public int Run()
        {
            if (!Directory.Exists(_path))
            {
                Console.Error.WriteLine("Error: Not a valid file or directory {0}", _path);
                return (int)ExitCode.CriticalError;
            }

            Verifier verifier = new Verifier(_path);
            if (!verifier.Verify())
                return (int)ExitCode.IssuesExists;

            Tester tester = new Tester(verifier.CompiledRuleset);
            tester.Run(_path);

            return (int)ExitCode.NoIssues;
        }

        private string _path;
    }
}