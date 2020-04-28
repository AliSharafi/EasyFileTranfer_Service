using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using EFTService.Utils;

namespace EFTService
{
    public partial class EFTService : ServiceBase
    {
        FileTransfer ft;

        public EFTService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            base.OnStart(args);
            if (args.Length > 0)
            {
                string _configPath = args[0];
                ft = new FileTransfer(true, _configPath);
            }
        }
        protected override void OnStop()
        {
            base.OnStop();
            try
            {
                ft.Stop();
            }
            catch(Exception ex)
            {
#if DEBUG
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "EFTService";
                    eventLog.WriteEntry("Error in Stop Listening " + ex.Message, EventLogEntryType.Information, 101, 1);
                }
#endif
            }

        }
    }
}
