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

        public UserCompanyInfoVM GetUserComponyInfoByMobile(string mobile)
        {
            using (var ctx = new ShtxSms2008Entities())
            {
                var cb = ctx.CustomerBases.FirstOrDefault(o => o.Tel.Contains(mobile) && o.SendInterFace == 102);
                if (cb != null)
                {
                    return new UserCompanyInfoVM()
                    {
                        AppAddressDesc = cb.AppAddressDesc,
                        AppBusinessDesc = cb.AppBusinessDesc,
                        AppCity = cb.AppCity,
                        AppCompanyName = cb.AppCompanyName,
                        AppCompanyPics = cb.AppCompanyPics,
                        AppCustomerName = cb.AppCustomerName,
                        AppIndustry = cb.AppIndustry,
                        AppProduct = cb.AppProduct,
                        AppProvince = cb.AppProvince,
                        AppTel = cb.AppTel,
                        IsOpenMsg = (cb.IsOpenMsg ?? false)
                    };
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
