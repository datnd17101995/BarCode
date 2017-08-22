using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Configuration;

namespace Merit.BarCodeScanner.WindowService
{
    public partial class BarCodeScanner : ServiceBase
    {
        //private ILogger _logService = new FileLogManager(typeof(BarCodeScannerService));
        public BarCodeScanner()
        {
            InitializeComponent();
            //log4net.Config.XmlConfigurator.Configure();
            //service = new BarCodeScannerService();

            //Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //AppSettingsSection _appSettings = configuration.AppSettings;            

            //var intervalValue = _appSettings.Settings["Interval"];
            //var timeValue = _appSettings.Settings["TimeRun"];

            //int interval = intervalValue == null ? 30000 : int.Parse(intervalValue.Value);
            //var time = timeValue == null ? "1830" : timeValue.Value;

            //ImportRequest importRequest = new ImportRequest
            //{
            //    FileName = _appSettings.Settings["FileName"]?.Value,
            //    PathFolder = _appSettings.Settings["PathFolder"]?.Value,
            //    PathFileTest = txtPath.Text
            //};

            //var resul = service.Import(importRequest, _appSettings);

            //var currentDate = DateTime.Now.ToString("HHmm");

            ////if (currentDate.Equals(time))
            ////{
            ////    timer = new Timer();
            ////    this.timer.Interval = interval; //10000 ~ 1s
            ////    this.timer.Elapsed += new System.Timers.ElapsedEventHandler(WorkProcess);
            ////}

            //timer = new Timer();
            //this.timer.Interval = interval; //10000 ~ 1s
            //this.timer.Elapsed += new System.Timers.ElapsedEventHandler(WorkProcess);
        }

        protected override void OnStart(string[] args)
        {
            //_logService.LogInfo("Service is Stoped");
            barCodeTimer.Enabled = false;
        }

        protected override void OnStop()
        {
            //_logService.LogInfo("Service is Stoped");
            barCodeTimer.Enabled = false;
        }        
    }
}
