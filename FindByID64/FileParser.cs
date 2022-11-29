using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using System.Text.Json;

using File = System.IO.File;

namespace FindByID64
{
    internal class FileParser
    {
        private HashSet<ulong> id64s;
        private CommandLineOptions opts;


        public FileParser(CommandLineOptions theOptions)
        {
            opts = theOptions;
            id64s = new();
        }


        public void Process()
        {
            verboseOutput($"Input ID64 list: {((String.IsNullOrEmpty(opts.InputFile)) ? "<none>" : opts.InputFile)}");
            verboseOutput($"Individual ID64s: {((opts.ID64s==null || opts.ID64s.Count==0) ? "<none>" : $"[{opts.ID64s.Count}] "+string.Join(',',opts.ID64s))}");
            verboseOutput($"Input JSON: {opts.JsonFile}");
            verboseOutput($"Output JSON: {opts.OutputFile}");
            verboseOutput($"Verbose: {opts.Verbose}");
            verboseOutput($"Multiple Matches: {opts.MultipleMatches}");
            verboseOutput($"ID64 Key: {opts.Key}");

            DateTime start = DateTime.UtcNow;
            if(opts.ID64s!=null && opts.ID64s.Count>0)
            {
                id64s.UnionWith(opts.ID64s);
            }

            if (!String.IsNullOrEmpty(opts.InputFile))
                readInputID64s();
            verboseOutput($"Unique ID64s: {id64s.Count}");
            parseJSON();
            DateTime end = DateTime.UtcNow;
            verboseOutput($"Time Taken: {(end - start).TotalSeconds.ToString("N2")} seconds");

        }


        private void readInputID64s()
        {
            string? line;
            ulong theid64;
            uint inputcnt = 0;
            uint linecnt = 0;
            if (File.Exists(opts.InputFile))
            {
                try
                {
                    using (FileStream fs = new FileStream(opts.InputFile, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader stream = new StreamReader(fs))
                        {
                            while ((line = stream.ReadLine()) != null)
                            {
                                if (line.Length > 0 && UInt64.TryParse(line, out theid64))
                                {
                                    id64s.Add(theid64);
                                    inputcnt++;
                                }
                                linecnt++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    showErrorAndExit($"Error reading {opts.InputFile}:\r\n{ex.Message}");
                }

                verboseOutput($"{linecnt} lines read, {inputcnt} id64s parsed");

            }
            else
            {
                showErrorAndExit($"{opts.InputFile} does \r\nnot exist");
            }
        }


        private void parseJSON()
        {
            if(opts.OutputFile==null || String.IsNullOrWhiteSpace(opts.OutputFile))
            {
                showErrorAndExit("output file is empty");
            }
            else if (File.Exists(opts.JsonFile))
            {
                try
                {
                    StreamReader stream;
                    GZipInputStream? zipStream = null;
                    string? line;
                    ulong outputcnt = 0;
                    ulong linecnt = 0;
                    FileStream fs = new FileStream(opts.JsonFile, FileMode.Open, FileAccess.Read);
                    if (opts.JsonFile.ToUpper().EndsWith(".GZ"))
                    {
                        zipStream = new GZipInputStream(fs);
                        stream = new StreamReader(zipStream);
                    }
                    else
                    {
                        stream = new StreamReader(fs);
                    }

                    bool stillmatching = true;
                    verboseOutput($"Writing {opts.OutputFile}");
                    using (StreamWriter osw = File.CreateText(opts.OutputFile))
                    {
                        while ((line = stream.ReadLine()) != null && stillmatching)
                        {
                            if (line.Length > 4)
                            {
                                line = line.TrimEnd(',').TrimStart(' ');
                                ISysID64? sys = (opts.Key == ID64Key.id64) ? JsonSerializer.Deserialize<SysID64>(line)
                                    : (opts.Key == ID64Key.SystemAddress) ? JsonSerializer.Deserialize<SysID64SystemAddress>(line)
                                    : JsonSerializer.Deserialize<SysID64SystemID64>(line);

                                if (sys != null && sys.ID64 != null && id64s.Contains((ulong)sys.ID64))
                                {
                                    osw.WriteLine(line);
                                    outputcnt++;
                                    if (!opts.MultipleMatches)
                                        id64s.Remove((ulong)sys.ID64);
                                    stillmatching = opts.MultipleMatches | (id64s.Count > 0);
                                }
                            }
                            linecnt++;
                        }
                    }
                    stream.Close();
                    stream.Dispose();
                    if (zipStream != null)
                        zipStream.Dispose();
                    fs.Dispose();

                    if (!opts.MultipleMatches && id64s.Count == 0)
                        verboseOutput("All ID64s matched");

                    verboseOutput($"{linecnt} lines read, {outputcnt} lines written");

                }
                catch (Exception ex)
                {
                    showErrorAndExit($"Error reading {opts.JsonFile}:\r\n{ex.Message}");
                }
            }
            else
            {
                showErrorAndExit($"{opts.JsonFile} does not exist");
            }

        }

        private void verboseOutput(string output)
        {
            if (opts.Verbose)
                Console.WriteLine(output);
        }

        private void showErrorAndExit(string output)
        {
            Console.WriteLine(output);
            Environment.Exit(-1);
        }

    }
}
