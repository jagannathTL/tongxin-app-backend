using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Model;

namespace Service
{
    public class DownloadCountService
    {
        public void addCount()
        {
            using (var ctx = new ShtxSms2008Entities())
            {
                ApkDownload download = new ApkDownload()
                {
                    downloadtime = DateTime.Now
                };
                ctx.ApkDownloads.Add(download);
                ctx.SaveChanges();
            }
        }

    }
}
