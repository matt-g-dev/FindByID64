using System.Collections.Generic;
using CommandLine.Text;
using CommandLine;

namespace FindByID64
{
    internal class CommandLineOptions
    {
        [Option('i', "input",
            Group = "id64s",
            HelpText = "File containing list of id64s (1 per line)")]
        public string? InputFile { get; set; }

        [Option('q', "id64s",
            Group = "id64s",
            Separator = ',',
            HelpText = "Comma-separated list of ID64s")]
        public ICollection<ulong>? ID64s { get; set; }

        [Option('j', "json",
            Required = true,
            HelpText = "Input JSON file to search (text or gzip)")]
        public string? JsonFile { get; set; }

        [Option('o', "output",
            Required = false,
            Default = "output.json",
            HelpText = "Output file")]
        public string? OutputFile { get; set; }

        [Option('m', "multiple",
            Required = false,
            Default = false,
            HelpText = "Match ID64s multiple times")]
        public bool MultipleMatches { get; set; }

        [Option('k', "key",
            Default = ID64Key.id64,
            HelpText = "Key for ID64 (id64, SystemAddress, systemId64)")]
        public ID64Key Key { get; set; }

        [Option('v', "verbose",
            Required = false,
            Default = false,
            HelpText = "Verbose output")]
        public bool Verbose { get; set; }


        [Usage(ApplicationAlias = "FindByID64.exe")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example("Find systems with a matching ID64 from a text file source", new CommandLineOptions
                        { InputFile = "id64list.txt", JsonFile="systemsWithCoordinates.json.gz", OutputFile="output.json" }),
                    new Example("Find systems with a matching ID64 from command-line", new CommandLineOptions
                        { ID64s=new List<ulong>{4374164130139, 3932277478106 }, JsonFile="systemsWithCoordinates.json.gz", OutputFile="output.json",  })                  
                };
            }
        }

    }
}
