using System.Configuration;
using Merit.BarCodeScanner.Models;

namespace Merit.BarCodeScanner.Services
{
    public interface IBarCodeScannerService
    {
        ResultRespose Import(ImportRequest request, AppSettingsSection _appSetting);
    }
}
