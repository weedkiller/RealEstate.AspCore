﻿using RealEstate.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace RealEstate.Runner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            IsAllowed(false);
            _mode = Modules.Mode.Release;
            _admin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            _configuration = Assembly.GetEntryAssembly().ReadConfiguration();
        }

        private delegate void SetTextCallback(string text);

        private readonly Modules.Mode _mode;
        private readonly bool _admin;
        private readonly R8Config _configuration;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogToFile();
        }

        private void LogToFile()
        {
            var logs = txtLog.Text;
            if (string.IsNullOrEmpty(logs))
                return;

            var path = Path.GetDirectoryName($"{Assembly.GetEntryAssembly().Location}");

            var logFolderPath = $"{path}\\logs";
            var isFolderExists = Directory.Exists(logFolderPath);
            if (isFolderExists)
            {
                var fileName = $"[{DateTime.Now.Date:yy-MM-dd} {DateTime.Now.ToString("t").Replace(":", "-")}] {Guid.NewGuid()}.txt";
                var currentLogFile = $"{logFolderPath}\\{fileName}";

                using (var fs = new FileStream(currentLogFile, FileMode.CreateNew))
                {
                    var info = new UTF8Encoding(true).GetBytes(logs);
                    fs.Write(info, 0, info.Length);
                }
            }
            else
            {
                var createDir = Directory.CreateDirectory(logFolderPath);
                if (createDir.Exists)
                {
                    LogToFile();
                }
            }
        }

        private void Log(string text)
        {
            if (this.txtConsole.InvokeRequired)
            {
                var d = new SetTextCallback(Log);
                Invoke(d, text);
            }
            else
            {
                txtConsole.Text += $"[{DateTime.Now}] {text}\r\n";
            }
        }

        private void IsAllowed(bool state)
        {
            btnFunc.Enabled = state;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log(_mode.ToString());
            Log(_admin ? "Ran as Administrator" : "Not ran as Administrator");

            var assembliesVersion = Modules.CheckAssembliesVersion();
            foreach (var ass in assembliesVersion)
            {
                if (ass.Value)
                {
                    Log(ass.Key);
                    IsAllowed(true);
                }
                else
                {
                    Log(ass.Key);
                    IsAllowed(false);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(_configuration.DbPath))
            {
                var file = File.Exists(_configuration.DbPath);
                if (file)
                {
                    Log($"Database path: {_configuration.DbPath}");
                    IsAllowed(true);
                }
                else
                {
                    Log("Unable to find database file.");
                    IsAllowed(false);
                    return;
                }
            }
            else
            {
                Log("Unable to find database file path.");
                IsAllowed(false);
                return;
            }

            var firewallActivation = Modules.CheckFirewall(MainAssembly, Port);
            if (!firewallActivation)
            {
                Log("Unable to open needed port.");
                IsAllowed(false);
                return;
            }
            else
            {
                Log("Port is already opened.");
                IsAllowed(true);
            }

            var ipCheck = Modules.CheckIp();
            if (ipCheck?.Any() == true)
            {
                Url = $"http://{ipCheck}:{Port}";
                Log($"Server IP: {Url}");
                IsAllowed(true);
            }
            else
            {
                Log("Unable to find server's ip.");
                IsAllowed(false);
                return;
            }

            var localDbCheck = Modules.CheckLocalDbInstalled();
            if (localDbCheck)
            {
                // msiexec /i SqlLocalDB.msi /qn IACCEPTSQLLOCALDBLICENSETERMS=YES
                Log("Microsoft SQL Server LocalDB 13.0 is installed");
                IsAllowed(true);
            }
            else
            {
                Log("Microsoft SQL Server LocalDB 13.0 not installed");
                IsAllowed(false);
                return;
            }

            // Microsoft .net core 2.2.103
            var dotNetCheck = Modules.CheckDotNetCoreRuntimesInstalled();
            Log(dotNetCheck);

            //var sql = Modules.GetLocalDB();
            //if (sql == null)
            //{
            //    AddInfo("Unable to create Sql Instance");
            //    IsAllowed(false);
            //    return;
            //}
            //else
            //{
            //    AddInfo("Database Instance has been created.");
            //    IsAllowed(true);
            //}

            var instanceFound = Modules.DbInstance("info", "Name:");
            if (instanceFound)
            {
                Log("Instance found.");
            }
            else
            {
                Log("Instance not found or not valid.");

                var instanceDeleted = Modules.DbInstance("d", "LocalDB instance \"MSSQLLocalDB\" deleted.");
                Log(instanceDeleted ? "Tried to delete former instance." : "Unable to delete former instance");

                var instanceCreated = Modules.DbInstance("c", "LocalDB instance \"MSSQLLocalDB\" created with");
                if (instanceCreated)
                {
                    Log("Instance created successfully.");
                }
                else
                {
                    Log("Unable to start created instance");
                    return;
                }

                var instanceStarted = Modules.DbInstance("start", "LocalDB instance \"MSSQLLocalDB\" started.");
                if (instanceStarted)
                {
                    Log("Instance started successfully.");
                }
                else
                {
                    Log("Unable to start instance");
                    return;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private Process Process { get; set; }
        private string Url { get; set; }
        private const ushort Port = 5566;
        private const string Prefix = "RealEstate";
        private static string MainAssembly => $"{Prefix}.Web";
        private static string MainAssemblyFile => $"{MainAssembly}.dll";

        private void button1_Click(object sender, EventArgs e)
        {
            if (btnFunc.Text.Equals("Stop"))
            {
                Program.KillDotNet();

                Log("Application stopped.");
                SetState("Start");
            }
            else if (btnFunc.Text.Equals("Start"))
            {
                var sync = Modules.CreateSynchronizer(Url);
                if (string.IsNullOrEmpty(sync))
                {
                    Log("Unable to Create Synchronizer.");
                    return;
                }
                else
                {
                    Log($"Synchronizer created in : {sync}");
                }

                foreach (var process in Process.GetProcesses())
                {
                    if (!process.ProcessName.Contains("dotnet"))
                        continue;

                    Log("DOTNET Process is currently running and will be closed soon.");
                    process.Kill();
                }

                Process = new Process
                {
                    EnableRaisingEvents = true,
                    StartInfo =
                    {
                        FileName = "dotnet",
                        Arguments = MainAssemblyFile,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        Verb = "runas"
                    }
                };

                Process.OutputDataReceived += Process_OutputDataReceived;
                Process.ErrorDataReceived += Process_ErrorDataReceived;
                Process.Exited += Process_Exited;

                var startState = Process.Start();
                if (!startState)
                {
                    Log("Unable to start app.");
                    return;
                }
                else
                {
                    Log("Process is preparing to run.");
                    SetState("Stop");
                }
                Process.BeginErrorReadLine();
                Process.BeginOutputReadLine();
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Log($"Application exited.");
            Program.KillDotNet();
            SetState("Start");
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_mode == Modules.Mode.Debug)
            {
                Log(e.Data);
            }
            else
            {
                LogDotnet(e.Data);
                Log($"Application got error.");
            }
        }

        private void SetState(string text)
        {
            if (this.btnFunc.InvokeRequired)
            {
                var d = new SetTextCallback(SetState);
                Invoke(d, text);
            }
            else
            {
                btnFunc.Text = text;
            }
        }

        private void LogDotnet(string text)
        {
            if (this.txtLog.InvokeRequired)
            {
                var d = new SetTextCallback(LogDotnet);
                Invoke(d, text);
            }
            else
            {
                txtLog.Text += $"[{DateTime.Now}] {text}\r\n";
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_mode == Modules.Mode.Debug)
            {
                Log(e.Data);
                if (e.Data?.Contains("Unable to start Kestrel.") == true)
                {
                    Log("Unable to start Application.");
                    Program.KillDotNet();
                    SetState("Stop");
                    return;
                }

                if (e.Data?.Contains("Application started. Press Ctrl+C to shut down.") == true)
                {
                    Log("Application running successfully.");
                    if (string.IsNullOrEmpty(Url))
                        return;

                    Process.Start(new ProcessStartInfo(Url));
                }
            }
            else
            {
                LogDotnet(e.Data);
                if (e.Data?.Contains("Unable to start Kestrel.") == true)
                {
                    Log("Unable to start Background process.");
                }
                else if (e.Data?.Contains("http://*:5566 is already in use.") == true)
                {
                    Log("Failed to bind to address http://[::]:5566: address already in use.");
                }
                else if (e.Data?.Contains("Application started. Press Ctrl+C to shut down.") == true)
                {
                    Log("Application running successfully.");
                    if (string.IsNullOrEmpty(Url))
                        return;

                    Process.Start(new ProcessStartInfo(Url));
                }
            }
        }
    }
}