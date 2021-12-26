using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;

namespace NugetWatcher
{
    public class Watcher : IDisposable
    {
        private readonly List<FileSystemWatcher> _fileSystemWatchers = new List<FileSystemWatcher>();
        private readonly string _configFile = "dirs.txt";
        private readonly string _nugetFile = "nuget.exe";
        private readonly ConcurrentQueue<string> _raisedFilesQueue = new ConcurrentQueue<string>();
        private Timer _timer;
        public void Init()
        {
            if (!File.Exists(_configFile))
            {
                Console.WriteLine($"[{DateTime.Now}] Config file {_configFile} not found. Program exited");
                return;
            }
            if (!File.Exists(_nugetFile))
            {
                Console.WriteLine($"[{DateTime.Now}] Nuget file {_nugetFile} not found. Program exited");
                return;
            }
            List<string> dirs = GetMonitoredDirs();
            if(dirs.Count == 0)
            {
                return;
            }
            if (!RegisterWatchers(dirs))
            {
                return;
            }
            _timer = new Timer(5000)
            {
                Enabled = true,
                AutoReset = true
            };
            _timer.Elapsed += OnTimerElapsed;
            StartWatchers();

        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {

            while (!_raisedFilesQueue.IsEmpty)
            {
                if(_raisedFilesQueue.TryDequeue(out string fullPath))
                {
                    string trimPattern = @"bin\\Debug.*";
                    string nuspecFile = Regex.Replace(fullPath, trimPattern, "", RegexOptions.IgnoreCase);
                    nuspecFile = Path.Combine(nuspecFile, "local.nuspec");
                    if (!File.Exists(nuspecFile))
                    {
                        Console.WriteLine($"[{DateTime.Now}] Nuspec file '{nuspecFile}' not found!");
                    }
                    else
                    {
                        try
                        {
                            string targetOutput = Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                    "NugetWatcherRepo"
                                );
                            if (!Directory.Exists(targetOutput))
                            {
                                Directory.CreateDirectory(targetOutput);
                            }
                            Console.WriteLine($"[{DateTime.Now}] Generating nuget {nuspecFile}");
                            string version = $"{DateTime.Now.ToString("yyyy.MM.dd.HHmmss")}-local";
                            ProcessStartInfo processStartInfo = new ProcessStartInfo
                            {
                                FileName = ".\\nuget.exe",
                                Arguments = $"pack \"{nuspecFile}\" -Version {version} -OutputDirectory \"{targetOutput}\"",
                                RedirectStandardOutput = true,
                                UseShellExecute = false
                            };
                            Process process = new Process
                            {
                                StartInfo = processStartInfo
                            };
                            process.Start();
                            process.WaitForExit();
                            Console.WriteLine($"[{DateTime.Now}] {nuspecFile} generated");

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{DateTime.Now}] Error while generating new local nuget package.");
                            Console.WriteLine($"[{DateTime.Now}] Detail: {ex.Message}");
                        }
                    }

                }
            }
        }

        private List<string> GetMonitoredDirs()
        {
            try
            {
                return File.ReadAllLines(_configFile).Select(c => c.Trim()).ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] An error occured while reading the config file {_configFile}''. Detail: {ex.Message}");
                return new List<string>();
            }
        }
        private bool RegisterWatchers(List<string> dirs)
        {
            try
            {
                foreach(string dir in dirs)
                {
                    string debugFolder = Path.Combine(dir, "bin", "Debug");
                    if (!Directory.Exists(debugFolder))
                    {
                        Directory.CreateDirectory(debugFolder);
                    }
                    var watcher = new FileSystemWatcher
                    {
                        Path = debugFolder,
                        Filter = "*.dll*",
                        NotifyFilter = NotifyFilters.LastWrite,
                        IncludeSubdirectories = true,
                        EnableRaisingEvents = false
                    };
                    watcher.Changed += OnChanged;
                    _fileSystemWatchers.Add(watcher);
                }
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] An error occured while registering watchers. Detail: {ex.Message}");
                return false;
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (!_raisedFilesQueue.Contains(e.FullPath))
            {
                _raisedFilesQueue.Enqueue(e.FullPath);
            }
        }

        private void StartWatchers()
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now}] Starting Nuget Watcher...");
                foreach(FileSystemWatcher watcher in _fileSystemWatchers)
                {
                    watcher.EnableRaisingEvents = true;
                    Console.WriteLine($"[{DateTime.Now}] - {watcher.Path}");
                }
                Console.WriteLine($"[{DateTime.Now}] Nuget Watcher started...");
                Console.WriteLine($"[{DateTime.Now}] Starting timer...");
                _timer.Start();
                Console.WriteLine($"[{DateTime.Now}] Timer started...");


            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] An error occured while registering watchers. Detail: {ex.Message}");
            }
        }

        public void Dispose()
        {
            foreach(FileSystemWatcher watcher in _fileSystemWatchers)
            {
                try
                {
                    watcher.Dispose();
                }
                catch { }
            }
        }
    }
}
