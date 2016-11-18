using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Service.ViewModel;

namespace Service
{
    public class InboxMsgService
    {
        #region 初始加载
        public List<InboxVM> GetInBoxMsgByMobile(string mobile)
        {
            var list = new List<InboxVM>();
            using (var ctx = new MetalSmsSendEntities())
            {
                //DateTime end = DateTime.Today.AddDays(1);
                //DateTime start = end.AddDays(-2);
                var log = ctx.App_Log.FirstOrDefault(o => o.mobile.Contains(mobile) && o.ErrCode == "1");
                if (log != null)
                {
                    var tel = log.mobile;
                    var count = ctx.App_Log.Count(o => o.mobile == tel && o.ErrCode == "1");
                    if (count > 0)
                        list = ctx.App_Log.Where(o => o.mobile == tel && o.ErrCode == "1").OrderByDescending(o => o.Date).Take(count < 30 ? count : 30)
                            .Select(o => new InboxVM { Id = o.ID, Date = o.Date, Msg = o.Msg, Url = o.Url }).ToList();
                }
            }
            return list;
        }

        #endregion

        #region 上拉下拉
        public List<InboxVM> GetInBoxMsgByAction(string mobile, string action, DateTime date)
        {
            var list = new List<InboxVM>();
            if (action == "pullDown")
            {
                list = GetInBoxMsgByPullDown(mobile, date);
            }
            else if (action == "pullUp")
            {
                list = GetInBoxMsgByPullUp(mobile, date);
            }
            return list;
        }


        //下拉，获取日期之后的所有
        private List<InboxVM> GetInBoxMsgByPullDown(string mobile, DateTime date)
        {
            var list = new List<InboxVM>();
            using (var ctx = new MetalSmsSendEntities())
            {
                var log = ctx.App_Log.FirstOrDefault(o => o.mobile.Contains(mobile) && o.ErrCode == "1");
                if (log != null)
                {
                    var tel = log.mobile;
                    list = ctx.App_Log.Where(o => o.mobile == tel && o.ErrCode == "1" && o.Date > date).OrderByDescending(o => o.Date)
                        .Select(o => new InboxVM { Id = o.ID, Date = o.Date, Msg = o.Msg, Url = o.Url }).ToList();
                }
            }
            return list;
        }

        //上拉，获取日期之前20条
        private List<InboxVM> GetInBoxMsgByPullUp(string mobile, DateTime date)
        {
            var list = new List<InboxVM>();
            using (var ctx = new MetalSmsSendEntities())
            {
                var log = ctx.App_Log.FirstOrDefault(o => o.mobile.Contains(mobile) && o.ErrCode == "1");
                if (log != null)
                {
                    var tel = log.mobile;
                    var count = ctx.App_Log.Count(o => o.mobile == tel && o.ErrCode == "1" && o.Date < date);
                    if (count > 0)
                    {
                        list = ctx.App_Log.Where(o => o.mobile == tel && o.ErrCode == "1" && o.Date < date).OrderByDescending(o => o.Date)
                            .Take(count > 20 ? 20 : count).Select(o => new InboxVM { Id = o.ID, Date = o.Date, Msg = o.Msg, Url = o.Url }).ToList();
                    }
                }
            }
            return list;
        }

        #endregion
    }
}
