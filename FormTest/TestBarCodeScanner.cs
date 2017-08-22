using System;
using System.Configuration;
using System.Windows.Forms;
using Merit.BarCodeScanner.Helpers;
using Merit.BarCodeScanner.Logging;
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

        private ILogger _logService = new FileLogManager(typeof(Form1));

        private void Import()
        {

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection _appSettings = configuration.AppSettings;

            ImportRequest importRequest = new ImportRequest
            {
                FileName = _appSettings.Settings["FileName"]?.Value,
                PathFolder = _appSettings.Settings["PathFolder"]?.Value,
                PathFileTest = txtPath.Text
            };

            var checkCsv = FileHelper.CheckFile(importRequest);
            ResultRespose resul = null;
            if (checkCsv.ToString().Equals("EXTENSION"))
            {
                resul = service.Import(importRequest, _appSettings);

                if (resul.Status)
                {
                    FileHelper.CopyFile(importRequest);
                    MessageBox.Show("Import to successfully");
                }
                else
                {
                    if (!string.IsNullOrEmpty(resul.Message))
                    {

                        var sendMail = EmailHelper.SendMail(_appSettings, new EmailContent
                        {
                            Body = resul.Message,
                            Subject = "Run batch have exception and stop"
                        }, resul.StartDate);

                        _logService.LogError("Send mail is error " + sendMail.Message);
                    }

                    MessageBox.Show("Import to unsuccessfully \r\n Plz check file log");
                }
            }
            else
            {
                MessageBox.Show("Can not find Raw data file ");
            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            btnImport.Enabled = true;
            if (string.IsNullOrEmpty(txtPath.Text))
            {
                MessageBox.Show("Plz input path file to test");
            }
            else
            {
                this.progressBar1.Value = 0;
                this.timer1.Interval = 50;
                this.timer1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Increment(1);
            lbMess.Text = "Import is progress " + progressBar1.Value.ToString() + "%";
            if (progressBar1.Value == progressBar1.Maximum)
            {
                timer1.Stop();
                Import();
                timer1.Stop();
            }
        }
    }
}
