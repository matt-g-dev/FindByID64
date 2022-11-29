using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using FindByID64;


CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
    .WithParsed(RunFileParser)
    .WithNotParsed(HandleParseError);

 void RunFileParser(CommandLineOptions opts)
{
    FileParser fp = new FileParser(opts);
    
    fp.Process();
}
 void HandleParseError(IEnumerable<Error> errs)
{
    Environment.Exit(-1);
}






