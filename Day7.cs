using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(7)]
    public class Day7 : Day
    {
        public override string RunPart1()
        {
            var lines = File.ReadAllLines(InputFile);

            var rootDir = ReadFileSystem(lines);
            rootDir.Print();


            var allFolders = rootDir.GetAllSubFolders();
            var smallFolders = allFolders.Where(dir => dir.Size < 100000);

            return smallFolders.Sum(dir => dir.Size).ToString();
        }


        public override string RunPart2()
        {
            var lines = File.ReadAllLines(InputFile);

            var rootDir = ReadFileSystem(lines);
            rootDir.Print();

            var totalSpace = 70000000;
            var updateSpace = 30000000;
            var unusedSpace = totalSpace - rootDir.Size;

            var requiredSpace = updateSpace - unusedSpace;
            Console.WriteLine($"needed space {requiredSpace}");

            var allFolders = rootDir.GetAllSubFolders();
            var possibleFolders = allFolders.Where(dir => dir.Size > requiredSpace).OrderBy(dir => dir.Size).ToList();

            var result = possibleFolders.First();
            return result.Size.ToString();
        }

        private AOCDirectory ReadFileSystem(IEnumerable<string> instructions)
        {
            AOCDirectory rootDir = null;
            AOCDirectory currentDir = null;
            foreach (var line in instructions)
            {
                var parts = line.Split(' ');
                if (line.StartsWith("$ cd"))
                {
                    var dirName = parts.Last();
                    if (rootDir == null)
                        rootDir = currentDir = new AOCDirectory(dirName);
                    else if (dirName == "..")
                        currentDir = currentDir.Parent;
                    else
                        currentDir = currentDir.GetDirectory(dirName);
                }
                else if (line.StartsWith("$ ls"))
                {
                    // nop
                }
                else
                {
                    if (line.StartsWith("dir"))
                    {
                        currentDir.Add(new AOCDirectory(parts.Last()));
                    }
                    else
                    {
                        currentDir.Add(new AOCFile(parts[1], int.Parse(parts[0])));
                    }
                }
            }
            return rootDir;
        }

        private class AOCDirectory
        {
            private readonly List<AOCFile> files = new List<AOCFile>();
            private readonly List<AOCDirectory> directories = new List<AOCDirectory>();

            public string Name { get; private set; }
            public AOCDirectory Parent { get; protected set; }

            public IEnumerable<AOCFile> Files => files;
            public IEnumerable<AOCDirectory> Directories => directories;

            public AOCDirectory(string name)
            {
                Name = name;
            }

            public void Add(AOCDirectory dir)
            {
                directories.Add(dir);
                dir.Parent = this;
            }

            public void Add(AOCFile file)
            {
                files.Add(file);
            }

            public AOCDirectory GetDirectory(string name)
            {
                Console.WriteLine($"get directory {name}");
                var subdir = directories.FirstOrDefault(d => d.Name == name);
                if (subdir == null) throw new Exception("Error: directory does not exist");
                return subdir;
            }

            public void Print(int indent = 0)
            {
                var prefix = new String(' ', indent);
                Console.WriteLine($"{prefix}- {Name} (dir)");
                foreach (var dir in directories)
                    dir.Print(indent + 2);
                foreach (var file in files)
                    file.Print(indent + 2);
            }

            public int Size
            {
                get { return Files.Sum(f => f.Size) + Directories.Sum(d => d.Size); }
            }

            public IEnumerable<AOCDirectory> GetAllSubFolders()
            {
                var list = new List<AOCDirectory>();
                list.Add(this);
                foreach (var dir in directories)
                    list.AddRange(dir.GetAllSubFolders());
                return list;
            }
        }

        private class AOCFile
        {
            public string Name { get; private set; }
            public int Size { get; private set; }

            public AOCFile(string name, int size)
            {
                Name = name;
                Size = size;
            }

            public void Print(int indent = 0)
            {
                var prefix = new String(' ', indent);
                Console.WriteLine($"{prefix}- {Name} (file, {Size})");
            }
        }
    }
}