using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Netstat_ano.Messages;
using Netstat_ano.Models;
using System.Diagnostics;

namespace Netstat_ano.ViewModels
{
    public partial class ConnectionInfoWindowViewModel : ObservableObject
    {
        public ConnectionInfo? ConnInfo { get; set; }

        [RelayCommand]
        private async Task TryKillProcess()
        {
            CMessage message = await Task.Run(() =>
              {
                  CMessage cMessage = new(this);
                  try
                  {
                      if (int.TryParse(ConnInfo?.PID, out int pid))
                      {
                          Process process = Process.GetProcessById(pid) ?? throw new Exception("Process Not Exist");
                          process?.Kill();
                          cMessage.Tag = "KillProcessSuccess";
                      }
                      else
                      {
                          throw new Exception("Bad PID");
                      }
                  }
                  catch (Exception e)
                  {
                      Debug.WriteLine(e);
                      cMessage.Tag = "KillProcessFailed";
                      cMessage.Content = e.Message;
                  }
                  return cMessage;
              });
            WeakReferenceMessenger.Default.Send(message);
        }
    }
}

