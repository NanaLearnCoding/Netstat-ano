using System.Text;

namespace Netstat_ano.Models
{
    public class ConnectionInfo
    {
        public string? Protocol { get; set; }
        public string? LocalAddress { get; set; }
        public string? RemoteAddress { get; set; }
        public string? State { get; set; }
        public string? PID { get; set; }

        public string? LocalPort { get; set; }
        public string? RemotePort { get; set; }

        public string? ProcessName { get; set; }
    }

    public static class ConnectionInfoExtension
    {
        public static string FilterString(this ConnectionInfo connectionInfo)
        {
            StringBuilder sb = new();
            sb.Append(
                string.IsNullOrEmpty(connectionInfo.LocalAddress)
                    ? ""
                    : connectionInfo.LocalAddress.ToLower()
            );
            sb.Append(' ');
            sb.Append(
                string.IsNullOrEmpty(connectionInfo.RemoteAddress)
                    ? ""
                    : connectionInfo.RemoteAddress.ToLower()
            );
            sb.Append(' ');
            sb.Append(string.IsNullOrEmpty(connectionInfo.PID) ? "" : connectionInfo.PID.ToLower());
            sb.Append(' ');
            sb.Append(
                string.IsNullOrEmpty(connectionInfo.ProcessName)
                    ? ""
                    : connectionInfo.ProcessName.ToLower()
            );
            sb.Append(' ');
            return sb.ToString();
        }
    }
}
