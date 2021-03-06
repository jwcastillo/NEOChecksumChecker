using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neo.SmartContract.Framework;
using NeoChecksumChecker.Contracts.Models;
using NeoChecksumChecker.Website.Models;
using NeoLux;

namespace NeoChecksumChecker.Website.Controllers
{
  public class HomeController : Controller
  {
    private readonly string _scriptHash = "bd8959714f1c9f83b0fb481c3c5edaa6af003a45";

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Demo(string checksum)
    {
      var model = new ChecksumViewModel();
      model.Checksum = checksum;
      model.ChecksumInfo = new ChecksumInfo(new byte[] { 1,2,3}, 8451235, "neon.exe");
      model.AddressInfo = new AddressInfo(54234, 107426, 5);
      return View("Check", model);

    }


    public IActionResult Check(string checksum)
    {
      var model = new ChecksumViewModel();
      model.Checksum = checksum;

      //Get checksum info from storage
      var checksumStorage = NeoAPI.getStorage(NeoAPI.Net.Test, _scriptHash, checksum);
      if (checksumStorage != null && checksumStorage.Value != null)
      {
        string checksumValue = checksumStorage.Value;
        ChecksumInfo checksumInfo = ChecksumInfo.FromBytes(Encoding.ASCII.GetBytes(checksumValue));
        model.ChecksumInfo = checksumInfo;

        //Get address info from storage (based on address from checksum info)
        var addressStorage = NeoAPI.getStorage(NeoAPI.Net.Test, _scriptHash, checksumInfo.Address.AsString());
        if (addressStorage != null && addressStorage.Value != null)
        {
          string value = addressStorage.Value;
          AddressInfo addressInfo = AddressInfo.FromBytes(Encoding.ASCII.GetBytes(value));
          model.AddressInfo = addressInfo;

        }
      }


      return View(model);
    }

    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
