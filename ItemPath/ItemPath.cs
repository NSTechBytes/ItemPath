using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Rainmeter;

namespace PluginItemPath
{
    internal class Measure
    {
        private API _api;
        public string MeasureType { get; set; }
        public string FileType { get; set; }
        public string VarName { get; set; }
        public string VarPath { get; set; }
        public string CopyPathVar { get; set; }
        public string CopyPath { get; set; }
        public string IniPath { get; set; }

        public Measure()
        {
            MeasureType = string.Empty;
            FileType = string.Empty;
            VarName = string.Empty;
            VarPath = string.Empty;
            CopyPathVar = string.Empty;
            CopyPath = string.Empty;
            IniPath = string.Empty;
        }

        public void Reload(API api, ref double maxValue)
        {
            _api = api; 
        }

        public void Execute(string args)
        {
           
            ParseArguments(args);

            if (string.IsNullOrEmpty(IniPath))
            {
                _api.Log(API.LogType.Error, "ItemPath.dll: Missing IniPath in the command.");
                return;
            }

            try
            {
                if (MeasureType == "ChooseFile")
                {
                    ExecuteChooseFile();
                }
                else if (MeasureType == "ChooseFolder")
                {
                    ExecuteChooseFolder();
                }
                else
                {
                    _api.Log(API.LogType.Error, $"ItemPath.dll: Unsupported MeasureType '{MeasureType}'.");
                }
            }
            catch (Exception ex)
            {
                _api.Log(API.LogType.Error, $"ItemPath.dll: Error - {ex.Message}");
            }
        }

        private void ExecuteChooseFile()
        {
           
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = CreateFilter(FileType);
                dialog.Title = "Select a File";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = dialog.FileName;
                    string fileName = Path.GetFileName(selectedFile);

                    
                    string copiedFilePath = CopyFileToPath(selectedFile);

                  
                    WriteVariablesToIni(fileName, selectedFile, copiedFilePath);

                    _api.Log(API.LogType.Debug, $"ItemPath.dll: File '{fileName}' processed and copied successfully.");
                }
            }
        }

        private void ExecuteChooseFolder()
        {
           
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a Folder";
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolder = dialog.SelectedPath;
                    string folderName = new DirectoryInfo(selectedFolder).Name;

                   
                    WriteVariablesToIni(folderName, selectedFolder, null);

                    _api.Log(API.LogType.Debug, $"ItemPath.dll: Folder '{folderName}' selected successfully.");
                }
            }
        }

        private string CopyFileToPath(string sourceFile)
        {
            if (string.IsNullOrEmpty(CopyPath))
                return null;

            if (!Directory.Exists(CopyPath))
            {
                Directory.CreateDirectory(CopyPath);
            }

            string destinationFile = Path.Combine(CopyPath, Path.GetFileName(sourceFile));
            File.Copy(sourceFile, destinationFile, overwrite: true);

            return destinationFile; 
        }

        private void WriteVariablesToIni(string name, string path, string copiedFilePath)
        {
            var iniLines = File.Exists(IniPath) ? File.ReadAllLines(IniPath) : new string[0];
            var iniContent = new List<string>(iniLines);

            
            UpdateOrAddVariable(iniContent, VarName, name);
            UpdateOrAddVariable(iniContent, VarPath, path);

        
            if (!string.IsNullOrEmpty(CopyPathVar) && copiedFilePath != null)
            {
                UpdateOrAddVariable(iniContent, CopyPathVar, copiedFilePath);
            }

            File.WriteAllLines(IniPath, iniContent);
        }

        private void UpdateOrAddVariable(List<string> iniContent, string variable, string value)
        {
            if (string.IsNullOrEmpty(variable) || value == null)
                return;

            int index = iniContent.FindIndex(line => line.StartsWith($"{variable}="));
            if (index >= 0)
            {
                iniContent[index] = $"{variable}={value}";
            }
            else
            {
                iniContent.Add($"{variable}={value}");
            }
        }

        private void ParseArguments(string args)
        {
            var arguments = args.Split('|');
            foreach (var argument in arguments)
            {
                var keyValue = argument.Split('=');
                if (keyValue.Length != 2) continue;

                string key = keyValue[0].Trim();
                string value = keyValue[1].Trim();

                switch (key)
                {
                    case "MeasureType":
                        MeasureType = value;
                        break;
                    case "FileType":
                        FileType = value;
                        break;
                    case "VarName":
                        VarName = value;
                        break;
                    case "VarPath":
                        VarPath = value;
                        break;
                    case "CopyPathVar":
                        CopyPathVar = value;
                        break;
                    case "CopyPath":
                        CopyPath = value;
                        break;
                    case "IniPath":
                        IniPath = value;
                        break;
                }
            }
        }

        private string CreateFilter(string fileType)
        {
            if (string.IsNullOrEmpty(fileType))
                return "All Files|*.*";

            string[] extensions = fileType.Split(',');
            string filter = string.Join(";", extensions.Select(ext => $"*{ext.Trim()}"));
            return $"Supported Files ({filter})|{filter}|All Files|*.*";
        }
    }

    public static class Plugin
    {
        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            GCHandle.FromIntPtr(data).Free();
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.Reload(new API(rm), ref maxValue);
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            return 0.0;
        }

        [DllExport]
        public static void ExecuteBang(IntPtr data, [MarshalAs(UnmanagedType.LPWStr)] string args)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.Execute(args);
        }
    }
}
