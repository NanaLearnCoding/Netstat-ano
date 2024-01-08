using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Netstat_ano.Messages;
using Netstat_ano.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Data;

namespace Netstat_ano
{
    partial class MainWindowViewModel : ObservableRecipient
    {
        public static string Title => "Netstat-ano";

        private readonly List<ConnectionInfo?>? connectionInfoList;

        public MainWindowViewModel()
        {
            connectionInfoList = new List<ConnectionInfo?>();
            connectionInfoList = QueryConnectionInfo();
            ConnectionsCount = connectionInfoList.Count;
            GridCollectionView = CollectionViewSource.GetDefaultView(connectionInfoList);
            GridCollectionView.Filter = (item) =>
            {
                if (string.IsNullOrEmpty(FilterText))
                {
                    return true;
                }
                else
                {
                    if (item is ConnectionInfo temp && temp != null)
                    {
                        return temp.FilterString().Contains(FilterText.Trim().ToLower());
                    }
                    else
                    {
                        return false;
                    }
                }
            };
        }

        protected override void OnActivated()
        {
            Messenger.Register<CMessage>(this, (recipient, message) =>
            {
                if (message != null)
                {
                    switch (message.Tag)
                    {
                        case "KillProcessSuccess":
                            Debug.WriteLine("KillProcessSuccess -> RefreshConnections");
                            _ = RefreshConnections();
                            break;

                        default:
                            break;
                    }
                }
            });
        }

        [ObservableProperty]
        public int connectionsCount = 0;

        [ObservableProperty]
        public string? filterText;

        partial void OnFilterTextChanged(string? value)
        {
            GridCollectionView?.Refresh();
        }

        [ObservableProperty]
        public int queryCount = 0;

        [ObservableProperty]
        public ICollectionView? gridCollectionView;

        [RelayCommand]
        private async Task RefreshConnections()
        {
            await Task.Run(() =>
                {
                    List<ConnectionInfo?> connections = QueryConnectionInfo();
                    int lastCount = QueryCount;
                    connectionInfoList?.Clear();
                    connectionInfoList?.AddRange(connections);
                    QueryCount = Interlocked.Increment(ref lastCount);
                });

            GridCollectionView?.Refresh();
            if (connectionInfoList != null)
            {
                ConnectionsCount = connectionInfoList.Count;
            }
        }

        [RelayCommand]
        private void CloseWindow()
        {
            CMessage cMessage = new(this, "CloseWindow");
            WeakReferenceMessenger.Default.Send(cMessage);
        }

        [RelayCommand]
        private void ExitApp()
        {
            CMessage cMessage = new(this, "ExitApp");
            WeakReferenceMessenger.Default.Send(cMessage);
        }

        [RelayCommand]
        private void ShowAbout()
        {
            CMessage cMessage = new(this, "ShowAbout");
            WeakReferenceMessenger.Default.Send(cMessage);
        }

        [RelayCommand]
        private void ShowSettings()
        {
            CMessage cMessage = new(this, "ShowSettings");
            WeakReferenceMessenger.Default.Send(cMessage);
        }

        [RelayCommand]
        private void ShowConnectionInfo(ConnectionInfo connectionInfo)
        {
            if (connectionInfo != null)
            {
                CMessage cMessage = new(this, "ShowConnectionInfo")
                {
                    Content = connectionInfo
                };
                WeakReferenceMessenger.Default.Send(cMessage);
            }
        }

        [RelayCommand]
        private async Task TryKillProcess(ConnectionInfo connectionInfo)
        {
            if (connectionInfo != null)
            {
                Tuple<bool, string> ret = await Task.Run(() =>
                {
                    bool flag = true;
                    string errorMsg = "";
                    try
                    {
                        if (int.TryParse(connectionInfo.PID, out int pid))
                        {
                            Process process = Process.GetProcessById(pid) ?? throw new Exception("Process Not Exist");
                            process?.Kill();
                        }
                        else
                        {
                            throw new Exception("Bad PID");
                        }
                    }
                    catch (Exception e)
                    {
                        flag = false;
                        errorMsg = e.Message;
                    }
                    return Tuple.Create(flag, errorMsg);
                });
                if (ret.Item1 == true)
                {
                    _ = RefreshConnections();
                }
                else
                {
                    CMessage cMessage = new(this, "KillProcessFailed")
                    {
                        Content = ret.Item2
                    };
                    WeakReferenceMessenger.Default.Send(cMessage);
                }
            }

        }

        #region private funcs
        private static List<ConnectionInfo?> QueryConnectionInfo()
        {
            Stopwatch sw = Stopwatch.StartNew();
            string netstatOutput = RunNetStatAnoCommand();
            List<ConnectionInfo?> connections = ParseNetStatOutput(netstatOutput);
            sw.Stop();
            Debug.WriteLine($"QueryConnectionInfo Cost {sw.ElapsedMilliseconds}");
            return connections;
        }

        private static string RunNetStatAnoCommand()
        {
            Process process = new();
            ProcessStartInfo startInfo = new()
            {
                FileName = "netstat.exe",
                Arguments = "-ano",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        private static List<ConnectionInfo?> ParseNetStatOutput(string output)
        {
            List<ConnectionInfo?> connectionInfoList = new();
            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Debug.WriteLine($"Lines Length {lines.Length}");
            connectionInfoList = lines.Skip(2).AsParallel().AsOrdered().Select(line =>
                        {
                            try
                            {

                                string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length == 5 && parts[0].Equals("TCP"))
                                {
                                    Process? processIfExist = null;
                                    string? processName = null;
                                    try
                                    {
                                        if (int.TryParse(parts[4], out int pid))
                                        {
                                            processIfExist = Process.GetProcessById(pid);
                                        }
                                        if (processIfExist != null)
                                        {
                                            processName = processIfExist.ProcessName;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    StringBuilder sb = new();
                                    sb.Append(string.IsNullOrEmpty(parts[1]) ? "" : parts[1].ToLower());
                                    sb.Append(' ');
                                    sb.Append(string.IsNullOrEmpty(parts[2]) ? "" : parts[2].ToLower());
                                    sb.Append(' ');
                                    sb.Append(string.IsNullOrEmpty(parts[4]) ? "" : parts[4].ToLower());
                                    sb.Append(' ');
                                    sb.Append(processIfExist == null ? "" : processIfExist.ProcessName.ToLower());
                                    sb.Append(' ');
                                    ConnectionInfo connectionInfo = new()
                                    {
                                        Protocol = parts[0],
                                        LocalAddress = parts[1],
                                        RemoteAddress = parts[2],
                                        State = parts[3],
                                        PID = parts[4],

                                        LocalPort = parts[1].Split(':').Last(),
                                        RemotePort = parts[2].Split(':').Last(),

                                        ProcessName = processName
                                    };

                                    return connectionInfo;
                                }
                                else if (parts.Length == 4 && parts[0].Equals("UDP"))
                                {
                                    Process? processIfExist = null;
                                    string? processName = null;
                                    try
                                    {
                                        if (int.TryParse(parts[3], out int pid))
                                        {
                                            processIfExist = Process.GetProcessById(pid);
                                        }
                                        if (processIfExist != null)
                                        {
                                            processName = processIfExist.ProcessName;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    StringBuilder sb = new();
                                    sb.Append(string.IsNullOrEmpty(parts[1]) ? "" : parts[1].ToLower());
                                    sb.Append(' ');
                                    sb.Append(string.IsNullOrEmpty(parts[2]) ? "" : parts[2].ToLower());
                                    sb.Append(' ');
                                    sb.Append(string.IsNullOrEmpty(parts[3]) ? "" : parts[3].ToLower());
                                    sb.Append(' ');
                                    sb.Append(processIfExist == null ? "" : processIfExist.ProcessName.ToLower());
                                    sb.Append(' ');

                                    ConnectionInfo connectionInfo = new()
                                    {
                                        Protocol = parts[0],
                                        LocalAddress = parts[1],
                                        RemoteAddress = parts[2],
                                        PID = parts[3],

                                        LocalPort = parts[1].Split(':').Last(),
                                        RemotePort = parts[2].Split(':').Last(),

                                        ProcessName = processName
                                    };
                                    return connectionInfo;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e);
                                return null;
                            }
                        }).Where(x => x != null).ToList();
            return connectionInfoList;
        }
        #endregion
    }
}
