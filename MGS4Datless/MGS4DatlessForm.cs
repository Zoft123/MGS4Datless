using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MGSDatless
{
    public partial class MGS4DatlessForm : Form
    {
        private readonly string binPath;
        private readonly string dictionaryPath;
        private readonly string solideyePath;
        private readonly string masterCnfPath;

        public MGS4DatlessForm()
        {
            InitializeComponent();
            binPath = Path.Combine(Application.StartupPath, "Resources");
            dictionaryPath = Path.Combine(binPath, "dictionary.txt");
            solideyePath = Path.Combine(binPath, "solideye.exe");
            masterCnfPath = Path.Combine(binPath, "master.cnf");
            ValidateEssentialFiles();
            Log("Application started.");
        }

        private void ValidateEssentialFiles()
        {
            bool isValid = File.Exists(dictionaryPath) && File.Exists(solideyePath) && File.Exists(masterCnfPath);
            if (!isValid)
            {
                MessageBox.Show("One or more essential files are missing in the bin directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnUndat.Enabled = false;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog { Description = "Select the main MGS folder." };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtMGSPath.Text = fbd.SelectedPath;
                Log($"Selected MGS Path: {fbd.SelectedPath}");
            }
        }

        private async void btnUndat_Click(object sender, EventArgs e)
        {
            string mgsPath = txtMGSPath.Text.Trim();

            if (string.IsNullOrEmpty(mgsPath) || !Directory.Exists(mgsPath))
            {
                MessageBox.Show("Please select a valid MGS folder path.", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnUndat.Enabled = btnBrowse.Enabled = false;
            progressBar.Value = 0;
            Log("Undat operation started.");

            try
            {
                await Task.Run(() => ProcessMGS(mgsPath));
                progressBar.Value = 100;
                Log("Undat operation completed successfully.");
                MessageBox.Show("Process completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnUndat.Enabled = btnBrowse.Enabled = true;
                progressBar.Value = 100;
            }
        }

        private void ProcessMGS(string mgsPath)
        {
            mgsPath = mgsPath.TrimEnd('\\');
            string stageDir = Path.Combine(mgsPath, "stage");

            if (!Directory.Exists(stageDir))
                throw new Exception("The 'stage' folder does not exist in the selected MGS directory.");

            string stageBackupDir = Path.Combine(mgsPath, "_stage");
            if (!Directory.Exists(stageBackupDir))
            {
                Directory.Move(stageDir, stageBackupDir);
                Log("Renamed 'stage' to '_stage'.");
            }
            else
            {
                Log("'_stage' folder already exists. Skipping rename.");
            }

            int totalDatFiles = 6;
            double datIncrement = 49.0 / totalDatFiles;
            for (int i = 0; i < totalDatFiles; i++)
            {
                string datFile = Path.Combine(mgsPath, $"stage0{i}.dat");
                string backupDatFile = Path.Combine(mgsPath, $"_stage0{i}.dat");

                if (File.Exists(datFile))
                {
                    UpdateProgress(datIncrement, $"Extracting {Path.GetFileName(datFile)}...");
                    ExecuteSolideyeExtract(datFile, mgsPath);
                    MoveFileWithOverwrite(datFile, backupDatFile);
                    Log($"Renamed {Path.GetFileName(datFile)} to {Path.GetFileName(backupDatFile)}.");
                }
                else
                {
                    Log($"File {Path.GetFileName(datFile)} does not exist. Skipping.");
                }
            }

            string newStageDir = Path.Combine(mgsPath, "stage");
            if (!Directory.Exists(newStageDir))
                throw new Exception("Expected 'stage' folder was not created after extraction.");

            Log("'stage' folder successfully recreated.");

            double darQarTotalProgress = 20.0;
            var stageSubFolders = Directory.GetDirectories(newStageDir).OrderBy(Path.GetFileName).ToArray();
            double darQarIncrement = darQarTotalProgress / (2 * stageSubFolders.Length);

            foreach (var subStage in stageSubFolders)
            {
                string subStageName = Path.GetFileName(subStage);
                Log($"Processing sub-stage: {subStageName}");

                string darDir = Path.Combine(subStage, "Dar");
                string qarDir = Path.Combine(subStage, "Qar");

                Directory.CreateDirectory(darDir);
                Directory.CreateDirectory(qarDir);

                MoveFilesByExtension(subStage, darDir, "DAR");
                MoveFilesByExtension(subStage, qarDir, "QAR");

                UpdateProgress(darQarIncrement, $"Packing Dar in {subStageName}...");
                ExecuteSolideyePack(darDir, subStage, "dar");

                UpdateProgress(darQarIncrement, $"Packing Qar in {subStageName}...");
                ExecuteSolideyePack(qarDir, subStage, "qar");

                if (subStageName.StartsWith("r_", StringComparison.OrdinalIgnoreCase))
                    RenameCacheFiles(subStage);
            }

            double cnfTotalProgress = 20.0;
            AssignConfigurationFiles(masterCnfPath, newStageDir);
            UpdateProgress(cnfTotalProgress, "Moved configuration files.");

            double mergeProgress = 10.0;
            UpdateProgress(mergeProgress, "Merging '_stage' back into 'stage'...");
            MergeStageFolders(stageBackupDir, newStageDir);

            UpdateProgress(0.5, "Finalizing...");
        }

        private void ExecuteSolideyeExtract(string datFilePath, string outputPath)
        {
            var psi = new ProcessStartInfo
            {
                FileName = solideyePath,
                Arguments = $"-e \"{datFilePath}\" -dict \"{dictionaryPath}\" -o \"{outputPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            proc.WaitForExit();

            if (proc.ExitCode != 0)
                throw new Exception($"Solideye failed to extract {Path.GetFileName(datFilePath)}.\nError: {proc.StandardError.ReadToEnd()}");

            Log($"Extracted {Path.GetFileName(datFilePath)} successfully.");
        }

        private void ExecuteSolideyePack(string folderPath, string outputPath, string packType)
        {
            var psi = new ProcessStartInfo
            {
                FileName = solideyePath,
                Arguments = $"-p \"{folderPath}\" -f {packType} -o \"{outputPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            proc.WaitForExit();

            if (proc.ExitCode != 0)
                throw new Exception($"Solideye failed to pack {packType} in {Path.GetFileName(outputPath)}.\nError: {proc.StandardError.ReadToEnd()}");

            Log($"Packed {packType} successfully in {Path.GetFileName(outputPath)}.");
            Directory.Delete(folderPath, true);
            Log($"Removed folder {folderPath} after packing.");
        }

        private void MoveFilesByExtension(string sourceDir, string destDir, string type)
        {
            var extensions = type switch
            {
                "DAR" => new[] { "cnp", "cpef", "cvd", "eqpp", "img", "jpg", "la2", "lt2", "lt3", "mdn", "mtar", "mtsq", "octl", "octt", "pdl", "phs", "raw", "rdv", "rvb", "sds", "sfp", "vab", "van", "var", "vib", "vlm" },
                "QAR" => new[] { "txn" },
                _ => Array.Empty<string>()
            };

            foreach (var ext in extensions)
            {
                foreach (var file in Directory.GetFiles(sourceDir, $"*.{ext}"))
                {
                    string destPath = Path.Combine(destDir, Path.GetFileName(file));
                    MoveFileWithOverwrite(file, destPath);
                    Log($"Moved {Path.GetFileName(file)} to {type} folder.");
                }
            }
        }

        private void RenameCacheFiles(string subStageDir)
        {
            var cacheFiles = new Dictionary<string, string>
    {
        { "cache.dar", "resident.dar" },
        { "cache.qar", "resident.qar" }
    };

            foreach (var kvp in cacheFiles) // Explicitly iterate over KeyValuePair
            {
                string sourcePath = Path.Combine(subStageDir, kvp.Key);
                string targetPath = Path.Combine(subStageDir, kvp.Value);
                if (File.Exists(sourcePath))
                {
                    MoveFileWithOverwrite(sourcePath, targetPath);
                    Log($"Renamed {kvp.Key} to {kvp.Value} in {Path.GetFileName(subStageDir)}.");
                }
            }
        }

        private void MergeStageFolders(string sourceDir, string destDir)
        {
            foreach (var sourceSubFolder in Directory.GetDirectories(sourceDir))
            {
                string subFolderName = Path.GetFileName(sourceSubFolder);
                string targetSubFolder = Path.Combine(destDir, subFolderName);
                Directory.CreateDirectory(targetSubFolder);

                foreach (var file in Directory.GetFiles(sourceSubFolder))
                {
                    string targetFilePath = Path.Combine(targetSubFolder, Path.GetFileName(file));
                    File.Copy(file, targetFilePath, true);
                    Log($"Copied {Path.GetFileName(file)} to 'stage' folder.");
                }
            }
        }

        private void AssignConfigurationFiles(string masterCnfFilePath, string stageDir)
        {
            var configMapping = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            string currentSubFolder = null;

            foreach (var line in File.ReadAllLines(masterCnfFilePath))
            {
                if (line.StartsWith("#"))
                {
                    currentSubFolder = line.Substring(1).Trim();
                    if (!configMapping.ContainsKey(currentSubFolder))
                        configMapping[currentSubFolder] = new List<string>();
                }
                else if (!string.IsNullOrWhiteSpace(line) && currentSubFolder != null)
                {
                    configMapping[currentSubFolder].Add(line);
                }
            }

            var stageSubFolders = Directory.GetDirectories(stageDir);
            double progressPerSubFolder = 20.0 / stageSubFolders.Length;

            foreach (var subStage in stageSubFolders)
            {
                string subStageName = Path.GetFileName(subStage);
                if (configMapping.TryGetValue(subStageName, out var cnfLines))
                {
                    File.WriteAllLines(Path.Combine(subStage, "data.cnf"), cnfLines);
                    Log($"Written data.cnf for {subStageName}.");
                }

                UpdateProgress(progressPerSubFolder, $"Configured {subStageName}.");
            }
        }

        private void UpdateProgress(double incrementPercentage, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(incrementPercentage, message)));
                return;
            }

            int newProgress = Math.Min(progressBar.Value + (int)Math.Round(incrementPercentage), 100);
            progressBar.Value = newProgress;
            Log(message);
        }

        private void Log(string message)
        {
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [INFO] {message}{Environment.NewLine}";
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => txtLog.AppendText(logMessage)));
            }
            else
            {
                txtLog.AppendText(logMessage);
            }

            string externalLogPath = Path.Combine(Application.StartupPath, "process.log");
            try
            {
                File.AppendAllText(externalLogPath, logMessage);
            }
            catch
            {
                // Silent catch
            }
        }

        private void MoveFileWithOverwrite(string sourcePath, string destinationPath)
        {
            if (File.Exists(destinationPath))
                File.Delete(destinationPath);

            File.Move(sourcePath, destinationPath);
            Log($"Moved file from {Path.GetFileName(sourcePath)} to {Path.GetFileName(destinationPath)}.");
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
