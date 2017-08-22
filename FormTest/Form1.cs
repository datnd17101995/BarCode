using System;
using System.Configuration;
using System.Windows.Forms;
using Merit.BarCodeScanner.Models;
using Merit.BarCodeScanner.Services;

namespace FormTest
{
    public partial class Form1 : Form
    {
        private IBarCodeScannerService service;
        public Form1()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
            service = new BarCodeScannerService();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection _appSettings = configuration.AppSettings;

            ImportRequest importRequest = new ImportRequest
            {
                FileName = _appSettings.Settings["FileName"]?.Value,
                PathFolder = _appSettings.Settings["PathFolder"]?.Value
            };

            service.Import(importRequest, _appSettings);
        }
    }
}
