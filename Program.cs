//MAIN RUNTIME
using System.Diagnostics;
using System.Runtime.InteropServices;
//EXECUTION
namespace FileOrFolderInUse {
    internal class Program {
        static void Main(string[] args) {
            string? filePath = args.Length > 0 ? args[0] : null;
            bool isEmpty = string.IsNullOrWhiteSpace(filePath);
            bool notExists = !File.Exists(filePath) && !Directory.Exists(filePath);
            if (isEmpty || notExists) {
                return;
            }
            else {
                Handle.Scan(filePath!);
            }
        }
    }
    internal class Handle {
        public static void Scan(string path) {
            string baseDir = AppContext.BaseDirectory;
            string handlePath = RuntimeInformation.OSArchitecture switch {
                                Architecture.X64 => Path.Combine(baseDir, "handle64.exe"),
                                Architecture.X86 => Path.Combine(baseDir, "handle.exe"),
                                Architecture.Arm64 => Path.Combine(baseDir, "handle64a.exe"),
                                _ => throw new NotSupportedException($"Unsupported architecture: {RuntimeInformation.OSArchitecture}"),
                            };
            ProcessStartInfo startInfo = new() {
                FileName = handlePath,
                Arguments = $"\"{path}\" -u",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using Process? process = Process.Start(startInfo);
            bool notStarted = process == null;
            if (notStarted) {
                return;
            }
            else {
                string output = process!.StandardOutput.ReadToEnd();
                process.WaitForExit();
                Dictionary<int, (string Name, int Count)> processes = Parse(output);
                MergeRunningProcesses(path, processes);
                bool noneIsRunning = processes.Count == 0;
                if (noneIsRunning) {
                    bool isSysFile = Path.GetExtension(path).Equals(".sys", StringComparison.OrdinalIgnoreCase);
                    if (isSysFile) {
                        MessageBox.Show(
                            "One or more .sys drivers are present!\nThey may still be in use...",
                            "Unsure If Available Or Not",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                    else{
                        MessageBox.Show(
                            "File or folder is not currently in use.",
                            "Already Available",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
                else {
                    Form form = new() {
                        Text = "Confirmation",
                        Width = 670,
                        Height = 420,
                        StartPosition = FormStartPosition.CenterScreen
                    };
                    ListView listView = new() {
                        View = View.Details,
                        FullRowSelect = true,
                        GridLines = true,
                        Dock = DockStyle.Fill,
                        Font = new Font("Cascadia Code", 10f)
                    };
                    listView.Columns.Add("PID", 80);
                    listView.Columns.Add("Name", 450);
                    listView.Columns.Add("Open Handles", 100, HorizontalAlignment.Center);
                    foreach (KeyValuePair<int, (string Name, int Count)> list in processes) {
                        ListViewItem item = new(list.Key.ToString());
                        item.SubItems.Add(list.Value.Name);
                        item.SubItems.Add(list.Value.Count.ToString());
                        listView.Items.Add(item);
                    }
                    Label titleLabel = new() {
                        Text = "Terminate these processes?",
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Cascadia Code", 16f)
                    };
                    Panel buttonPanel = new() {
                        Dock = DockStyle.Bottom,
                        Height = 50
                    };
                    Button noButton = new() {
                        Font = new Font("Cascadia Code", 12f),
                        Text = "No",
                        DialogResult = DialogResult.No,
                        Width = 100,
                        Height = 30,
                        FlatStyle = FlatStyle.Flat,
                        Dock = DockStyle.Left
                    };
                    Button yesButton = new() {
                        Font = new Font("Cascadia Code", 12f),
                        Text = "Yes",
                        DialogResult = DialogResult.Yes,
                        Width = 100,
                        Height = 30,
                        FlatStyle = FlatStyle.Flat,
                        Dock = DockStyle.Right
                    };
                    void AddHover(Button press) {
                        Color hover = Color.LightGray;
                        press.FlatAppearance.BorderSize = 1;
                        press.MouseEnter += (s, e) => press.BackColor = hover;
                        press.MouseLeave += (s, e) => press.BackColor = SystemColors.Control;
                    }
                    AddHover(yesButton);
                    AddHover(noButton);
                    buttonPanel.Controls.Add(titleLabel);
                    buttonPanel.Controls.Add(noButton);
                    buttonPanel.Controls.Add(yesButton);
                    form.Controls.Add(listView);
                    form.Controls.Add(buttonPanel);
                    yesButton.Click += (s, e) => form.DialogResult = DialogResult.Yes;
                    noButton.Click += (s, e) => form.DialogResult = DialogResult.No;
                    DialogResult result = form.ShowDialog();
                    bool pressedYes = result == DialogResult.Yes;
                    if (pressedYes) {
                        List<string> failures = [];
                        foreach (int pid in processes.Keys) {
                            try {
                                Process.GetProcessById(pid).Kill();
                            }
                            catch (Exception ERROR) {
                                failures.Add($"PID {pid}: {ERROR.Message}");
                            }
                        }
                        bool Success = failures.Count == 0;
                        if (Success) {
                            MessageBox.Show("All processes closed successfully.",
                                            "Success",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information
                                        );
                        }
                        else {
                            MessageBox.Show("One or more processes could not be closed:\n\n" + string.Join("\n", failures),
                                            "Failure",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error
                                        );
                        }
                    }
                }
            }
        }
        private enum ProcessType {
            User,
            System,
            ServiceHost
        }
        private static ProcessType ClassifyProcess(string name) {
            name = name.ToLowerInvariant();
            if (name == "svchost") {
                return ProcessType.ServiceHost;
            }
            else if ( name == "smss"
                   || name == "csrss"
                   || name == "wininit"
                   || name == "services"
                   || name == "lsass"
                   || name == "MsMpEng"
                   || name == "wsccommunicator" ) {
                return ProcessType.System;
            }
            else {
                return ProcessType.User;
            }
        }
        private static void MergeRunningProcesses(
            string targetPath,
            Dictionary<int, (string Name, int Count)> result) {
            foreach (Process running in Process.GetProcesses()) {
                string name = running.ProcessName;
                try {
                    ProcessType type = ClassifyProcess(name);
                    string? exe = running.MainModule?.FileName;
                    bool notExecutable = exe == null;
                    bool notUser = type != ProcessType.User;
                    bool outsideTargetPath = !exe!.StartsWith(targetPath, StringComparison.OrdinalIgnoreCase);
                    if (notExecutable || outsideTargetPath || notUser) {
                        throw new Exception();
                    }
                    else {
                        bool pidExists = result.TryGetValue(running.Id, out (string Name, int Count) existing);
                        if (pidExists) {
                            result[running.Id] = (name, existing.Count + 1);
                        }
                        else {
                            result[running.Id] = (name, 1);
                        }
                    }
                }
                catch {
                    continue;
                }
            }
        }
        private static Dictionary<int, (string Name, int Count)> Parse(string output) {
            Dictionary<int, (string Name, int Count)> result = [];
            string[] lines = output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines) {
                try {
                    bool notPid = !line.Contains("pid:");
                    if (notPid) {
                        continue;
                    }
                    else {
                        throw new Exception();
                    }
                }
                catch {
                    string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string processName = parts.Length > 0 ? parts[0] : "unknown";
                    for (int i = 0; i < parts.Length; i++) {
                        bool hasPidValue = parts[i] == "pid:" && i + 1 < parts.Length;
                        if (hasPidValue) {
                            bool validPid = int.TryParse(parts[i + 1], out int pid);
                            if (validPid) {
                                bool foundPidEntry = result.TryGetValue(pid, out (string Name, int Count) value);
                                if (foundPidEntry) {
                                    result[pid] = (processName, value.Count + 1);
                                }
                                else {
                                    result[pid] = (processName, 1);
                                }
                            }
                            else {
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}