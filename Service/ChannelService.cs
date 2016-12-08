using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Service.ViewModel;

namespace Service
{
    public class ChannelService
    {
        public List<ChannelVM> GetAllChannels()
        {
            //change://
            //using (var ctx = new ShtxSms2008Entities())
            //{
            //    return ctx.Channels.Select(o => new ChannelVM { 
            //        Id = o.ID,
            //        Name = o.Name
            //    }).ToList();
            //}
            return CacheService.GetChannel();
        }
    }
}
