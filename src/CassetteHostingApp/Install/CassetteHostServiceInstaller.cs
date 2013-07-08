using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using CassetteHostingEnvironment;

namespace CassetteHostingApp.Install
{
    [RunInstaller(true)]
    public class CassetteHostServiceInstaller : Installer
    {
        public CassetteHostServiceInstaller()
        { 
            var process = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };
            var service = new ServiceInstaller { ServiceName = CassetteHostingService.CassetteServiceName };
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
