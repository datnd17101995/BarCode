using System;
using System.Configuration;
using System.ServiceProcess;
using Merit.BarCodeScanner.Logging;
using Merit.BarCodeScanner.Models;
using Merit.BarCodeScanner.Services;
using Merit.BarCodeScanner.Helpers;
using System.Timers;

namespace Merit.BarCodeScanner.WindowsService2
{
    public partial class BarCodeScanner : ServiceBase
    {
        private ILogger _logService = new FileLogManager(typeof(BarCodeScanner));
        private Timer timer;
        private AppSettingsSection _appSettings;
        private BarCodeScannerService service;
        private ImportRequest importRequest;
        private int reScanBarCode = 0;
        private int interval = 0;
        DateTime lastRun = DateTime.Now;
        int minutes = 60 * 1000;
        public BarCodeScanner()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _appSettings = configuration.AppSettings;
            service = new BarCodeScannerService();
        }

        protected override void OnStart(string[] args)
        {
            _logService.LogInfo("Service is Start");
            var intervalValue = _appSettings.Settings["Interval"];
            interval = intervalValue == null ? 30000 : int.Parse(intervalValue.Value);
            timer = new Timer(minutes);
            this.timer.Interval = minutes; //10000 ~ 1s
            this.timer.Elapsed += WorkProcess;
            timer.Start();
        }

        private void WorkProcess(object sender, ElapsedEventArgs e)
        {
            var currentDate = DateTime.Parse(DateTime.Now.ToString("HH:mm:ss"));
            var timeValue = _appSettings.Settings["TimeRun"];
            var timeRescan = _appSettings.Settings["TimeReScan"] == null ? 60000 : int.Parse(_appSettings.Settings["TimeReScan"].Value);
            DateTime time = DateTime.Parse(timeValue == null ? "18:30:00" : timeValue.Value.ToString());
            DateTime timeRun = DateTime.Today.AddHours(time.Hour).AddMinutes(time.Minute);
            //_logService.LogError("timeRun: " + timeRun);
            //_logService.LogError("currentDate: " + currentDate);
            //_logService.LogError("lastRun: " + lastRun);
            if (currentDate >= timeRun && lastRun < timeRun)
            {
                timer.Stop();

                importRequest = new ImportRequest
                {
                    FileName = _appSettings.Settings["FileName"]?.Value,
                    PathFolder = _appSettings.Settings["PathFolder"]?.Value,
                    FileNameNew =_appSettings.Settings["FileNew"]?.Value
                };
                var checkFile = FileHelper.CheckFile(importRequest);

                //check if file exits
                if (checkFile.ToString().Equals("EXTENSION"))
                {
                    reScanBarCode = 0;
                    try
                    {
                        var results = service.Import(importRequest, _appSettings);

                        //timer.Interval = 3 * 60 * 1000;
                        if (results.Status)
                        {
                            FileHelper.CopyFile(importRequest);
                            //_logService.LogError("importRequest: " + importRequest);
                        }
                        else
                        {
                            //if Barcode Scanner Batch run got Error 
                            if (!string.IsNullOrEmpty(results.Message))
                            {

                                var sendMail = EmailHelper.SendMail(_appSettings, new EmailContent
                                {
                                    Body = results.Message,
                                    Subject = "Barcode Scanner Batch run got Error and Stop"
                                }, results.StartDate);
                                if (sendMail.Message == "")
                                {
                                    _logService.LogError("Send mail is successfully ");
                                }
                                else
                                {
                                    _logService.LogError("Send mail is error " + sendMail.Message.ToString());
                                }
                                _logService.LogInfo("End: " + DateTime.Now.ToString());
                                _logService.LogInfo("##############################################");
                            }
                        }
                        lastRun = DateTime.Now;
                    }
                    catch (Exception)
                    {

                    }

                }
                else if (checkFile.ToString().Equals("NOTEXSIT"))
                {
                    reScanBarCode++;
                    //auto re-scan after 1 minutes if could not fould
                    //_logService.LogError("reScanBarCode" + reScanBarCode);
                    timer.Interval = timeRescan;
                    if (reScanBarCode == 3)
                    {
                        var sendMail = EmailHelper.SendMail(_appSettings, new EmailContent
                        {
                            Body = "Can not find Raw data file",
                            Subject = "Barcode Scanner Batch run got Error and Stop"
                        }, DateTime.Now);
                        reScanBarCode = 0;
                        _logService.LogError("Can not find Raw data file");
                        //_logService.LogError(sendMail.Message);
                        if (sendMail.Message == "")
                        {
                            _logService.LogError("Send mail successfully ");
                        }
                        else
                        {
                            _logService.LogError("Send mail is error " + sendMail.Message.ToString());
                        }
                        lastRun = DateTime.Now;
                        _logService.LogInfo("End: " + DateTime.Now.ToString());
                        _logService.LogInfo("##############################################");
                    }
                }
                
                if (reScanBarCode == 0)
                {
                    //auto scan after interval minutes after success
                    timer.Interval = minutes;
                    //_logService.LogError("Interval " + timer.Interval);
                }
                timer.Start();
            }
        }
        protected override void OnStop()
        {
            _logService.LogInfo("Service is Stoped");
            timer.Enabled = false;
            barCodeTimer.Enabled = false;
        }
    }
}
