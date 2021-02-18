using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using ICSharpCode;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace Zip
{
    public sealed class Zip
    {
        public string Directory { get; private set; }
        public string ArchivePath { get; private set; }
        public bool CanOverwrite { get; private set; } = false;
        public string Output { get; private set; }

        public Zip(string[] args)
        {
            switch (args[0])
            {
                case "archive":
                    string directory = args[1];
                    string archive = args[2];
                    if (!System.IO.Directory.Exists(directory))
                    {
                        throw new ArgumentException("Directory doesn't exist", directory);
                    }
                    if (!System.IO.Directory.Exists(Path.GetDirectoryName(archive)))
                    {
                        throw new ArgumentException("Archive output directory doesn't exist", directory);
                    }
                    if (File.Exists(archive))
                    {
                        Console.WriteLine("Do you wish to overwrite this file?\ny/N\n");
                        string input = Console.ReadLine();
                        switch (input)
                        {
                            case "Y":
                            case "y":
                                CanOverwrite = true;
                                break;
                            default:
                                CanOverwrite = false;
                                break;
                        }
                    }
                    Directory = directory;
                    ArchivePath = archive;
                    break;
                case "update":
                    archive = args[1];
                    if (!File.Exists(archive))
                    {
                        throw new ArgumentException("Archive file doesn't exist", archive);
                    }
                    ArchivePath = archive;
                    break;
                case "extract":
                    archive = args[1];
                    string output = args[2];
                    if (!System.IO.File.Exists(archive))
                    {
                        throw new ArgumentException("Archive file doesn't exist", archive);
                    }
                    if (!System.IO.Directory.Exists(output))
                    {
                        Console.WriteLine("Output directory doesn't exist. Do you wish to create output directory?\nY/n\n");
                        string input = Console.ReadLine();
                        switch (input)
                        {
                            case "N":
                            case "n":
                                throw new ArgumentException("Output directory doesn't exist", output);
                            default:
                                try
                                {
                                    System.IO.Directory.CreateDirectory(output);
                                }
                                catch
                                {
                                    throw new ArgumentException("Output directory can't be created, due to access restrictions. Please check output directory path.", output);
                                }
                                break;
                        }
                        
                    }
                    ArchivePath = archive;
                    Output = output;
                    break;
                case "help":
                    Console.WriteLine("archive <path to folder to archive> <path to output .tar.gz file with extension>");
                    Console.WriteLine("update <path to output .tar.gz file with extension>");
                    Console.WriteLine("extract <path to output .tar.gz file with extension> <path to output folder>");
                    Environment.Exit(0);
                    break;
                default:
                    throw new ArgumentException("Command no found", args[0]);
            }
        }

        public void Archive()
        {
            var prepared = GetPreparedFiles(Directory);
            using (var fs = new FileStream(ArchivePath, FileMode.Create))
            {
                using(var compres=new GZipOutputStream(fs))
                {
                    using(var archive = TarArchive.CreateOutputTarArchive(compres))
                    {
                        archive.RootPath = Directory;
                        foreach(var p in prepared)
                        {
                            archive.WriteEntry(TarEntry.CreateEntryFromFile(p), true);
                        }
                    }
                }
            }

        }

        private List<string> GetRecurFiles(string directory)
        {
            var files = System.IO.Directory.GetFiles(directory);
            var directories = System.IO.Directory.GetDirectories(directory);

            var list = new List<string>(files);

            foreach(var d in directories)
            {
                list.AddRange(GetRecurFiles(d));
            }

            return list;
        }

        private List<string> GetPreparedFiles(string directory)
        {
            var files = GetRecurFiles(directory);
            if (files.Count == 0)
            {
                throw new Exception("Directory is empty");
            }
            var prepared = new List<string>();

            foreach (var f in files)
            {
                if (f.EndsWith(".c") || f.EndsWith(".h"))
                {
                    prepared.Add(f);
                }
            }
            if (prepared.Count == 0)
            {
                throw new Exception("Directory haven't .c or .h files");
            }
            return prepared;
        }

        public void Update()
        {
            
           if(!File.Exists(ArchivePath))
           {
                throw new Exception("Archive doesn't exist");
           }
            var tmp = System.IO.Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(ArchivePath), "tmp"));

            using (var fs =File.OpenRead(ArchivePath))
            {
                using (var decompresed=new GZipInputStream(fs))
                {
                    using (var tararchive = TarArchive.CreateInputTarArchive(decompresed, Encoding.UTF8))
                    {
                        tararchive.ExtractContents(tmp.FullName);
                    }
                }
            }
            File.Create(Path.Combine(tmp.FullName, "foo.c")).Close();
            File.Create(Path.Combine(tmp.FullName, "bar.c")).Close();
            File.Delete(ArchivePath); //Was possible error
            var prepared = GetPreparedFiles(tmp.FullName);
            using (var fs = new FileStream(ArchivePath, FileMode.Create))
            {
                using (var compres = new GZipOutputStream(fs))
                {
                    using (var archive = TarArchive.CreateOutputTarArchive(compres))
                    {
                        archive.RootPath =tmp.FullName;
                        foreach (var p in prepared) //Was error
                        {
                            archive.WriteEntry(TarEntry.CreateEntryFromFile(p), true);
                        }
                    }
                }
            }
            System.IO.Directory.Delete(tmp.FullName, true); //Was error, recursive
        }

        public void Extract()
        {
            using (var fs = File.OpenRead(ArchivePath))
            {
                using (var decompresed = new GZipInputStream(fs))
                {
                    using (var tararchive = TarArchive.CreateInputTarArchive(decompresed, Encoding.UTF8))
                    {
                        tararchive.ExtractContents(Output);
                    }
                }
            }
        }
    }
}
