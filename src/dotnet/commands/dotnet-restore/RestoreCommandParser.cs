// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.Cli.CommandLine;
using LocalizableStrings = Microsoft.DotNet.Tools.Restore.LocalizableStrings;

namespace Microsoft.DotNet.Cli
{
    internal static class RestoreCommandParser
    {
        public static Command Restore() =>
            Create.Command(
                "restore",
                LocalizableStrings.AppFullName,
                Accept.ZeroOrMoreArguments(),
                FullRestoreOptions());

        private static Option[] FullRestoreOptions()
        {
            var fullRestoreOptions = AddImplicitRestoreOptions(new Option[] { CommonOptions.HelpOption() }, true, true);

            return fullRestoreOptions.Concat(new Option[] { CommonOptions.VerbosityOption() }).ToArray();
        }

        public static Option[] AddImplicitRestoreOptions(
            IEnumerable<Option> commandOptions)
        {
            return AddImplicitRestoreOptions(commandOptions, false, false).ToArray();
        }

        private static IEnumerable<Option> AddImplicitRestoreOptions(
            IEnumerable<Option> commandOptions,
            bool showHelp,
            bool useShortOptions)
        {
            return commandOptions.Concat(ImplicitRestoreOptions(showHelp, useShortOptions)
                .Where(o => !commandOptions.Any(c => c.Name == o.Name)));
        }

        private static Option[] ImplicitRestoreOptions(bool showHelp = false, bool useShortOptions = false)
        {
            return new Option[] {
                Create.Option(
                    useShortOptions ? "-s|--source" : "--source",
                    showHelp ? LocalizableStrings.CmdSourceOptionDescription : string.Empty,
                    Accept.OneOrMoreArguments()
                          .With(name: LocalizableStrings.CmdSourceOption)
                          .ForwardAsSingle(o => $"/p:RestoreSources={string.Join("%3B", o.Arguments)}")),
                Create.Option(
                    useShortOptions ? "-r|--runtime" : "--runtime" ,
                    LocalizableStrings.CmdRuntimeOptionDescription,
                    Accept.OneOrMoreArguments()
                          .WithSuggestionsFrom(_ => Suggest.RunTimesFromProjectFile())
                          .With(name: LocalizableStrings.CmdRuntimeOption)
                          .ForwardAsSingle(o => $"/p:RuntimeIdentifiers={string.Join("%3B", o.Arguments)}")),
                Create.Option(
                    "--packages",
                    showHelp ? LocalizableStrings.CmdPackagesOptionDescription : string.Empty,
                    Accept.ExactlyOneArgument()
                          .With(name: LocalizableStrings.CmdPackagesOption)
                          .ForwardAsSingle(o => $"/p:RestorePackagesPath={o.Arguments.Single()}")),
                Create.Option(
                    "--disable-parallel",
                    showHelp ? LocalizableStrings.CmdDisableParallelOptionDescription : string.Empty,
                    Accept.NoArguments()
                          .ForwardAs("/p:RestoreDisableParallel=true")),
                Create.Option(
                    "--configfile",
                    showHelp ? LocalizableStrings.CmdConfigFileOptionDescription : string.Empty,
                    Accept.ExactlyOneArgument()
                          .With(name: LocalizableStrings.CmdConfigFileOption)
                          .ForwardAsSingle(o => $"/p:RestoreConfigFile={o.Arguments.Single()}")),
                Create.Option(
                    "--no-cache",
                    showHelp ? LocalizableStrings.CmdNoCacheOptionDescription : string.Empty,
                    Accept.NoArguments()
                          .ForwardAs("/p:RestoreNoCache=true")),
                Create.Option(
                    "--ignore-failed-sources",
                    showHelp ? LocalizableStrings.CmdIgnoreFailedSourcesOptionDescription : string.Empty,
                    Accept.NoArguments()
                          .ForwardAs("/p:RestoreIgnoreFailedSources=true")),
                Create.Option(
                    "--no-dependencies",
                    LocalizableStrings.CmdNoDependenciesOptionDescription,
                    Accept.NoArguments()
                          .ForwardAs("/p:RestoreRecursive=false")),
                Create.Option(
                    useShortOptions ? "-f|--force" : "--force",
                    LocalizableStrings.CmdForceRestoreOptionDescription,
                    Accept.NoArguments()
                          .ForwardAs("/p:RestoreForce=true"))
            };
        }
    }
}