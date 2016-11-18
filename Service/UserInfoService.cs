using Model;
using Service.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserInfoService
    {
        public UserInfoVM GetUserInfoByMobile(string mobile)
        {
            using (var ctx = new ShtxSms2008Entities())
            {
                CustomerExtend ce = ctx.CustomerExtend.FirstOrDefault(o => o.SendInterFace == 102 && o.Tel.Contains(mobile));
                UserInfoVM userInfo = new UserInfoVM() { 
                    EndDate = ce.EndDate.Value.ToString("yyyy-MM-dd")
                };
                var appCustomer = ctx.AppCustomerTokens.FirstOrDefault(o => o.tel == mobile);
                if (appCustomer != null)
                {
                    userInfo.IsSound = appCustomer.isSound ?? false;
                }
                return userInfo;
            }
        }

        public bool SetUserInfoSound(string mobile, bool isSound)
        {
            bool flag = false;
            try
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var appCustomer = ctx.AppCustomerTokens.FirstOrDefault(o => o.tel == mobile);
                    if (appCustomer != null)
                    {
                        appCustomer.isSound = isSound;
                        ctx.SaveChanges();
                    }
                }
                flag = true;
            }
            catch (Exception)
            {
                
            }
           
            return flag;
        }
    }
}
