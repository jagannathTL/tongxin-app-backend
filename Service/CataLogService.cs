using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Service.ViewModel;

namespace Service
{
    public class CataLogService
    {
        public List<CataLogVM> GetCataLogByChannelId(int channelId)
        {
            //change://
            //using (var ctx = new ShtxSms2008Entities())
            //{
                //return ctx.CataLogs.Where(o => o.ChannelId == channelId).Select(o => new CataLogVM {
                //    Id = o.ID,
                //    Name = o.KeyName,
                //    Desc = o.Description
                //}).ToList();
                
            //}
            return CacheService.GetCataLogByChannelId(channelId);
        }
    }
}
