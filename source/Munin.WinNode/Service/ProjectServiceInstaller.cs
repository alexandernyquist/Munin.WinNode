using System.ComponentModel;
using System.ServiceProcess;

namespace Munin.WinNode
{
    [RunInstaller(true)]
    public sealed class ProjectServiceInstaller : ServiceInstaller
    {
        public const string ProjectServiceName = "Munin.WinNode";

        public ProjectServiceInstaller()
        {
            Description = ProjectServiceName;
            DisplayName = ProjectServiceName;
            ServiceName = ProjectServiceName;
            StartType = ServiceStartMode.Automatic;
        }
    }

    [RunInstaller(true)]
    public sealed class ProjectServiceProcessInstaller : ServiceProcessInstaller
    {
        public ProjectServiceProcessInstaller()
        {
            Account = ServiceAccount.LocalSystem;
        }
    }
}