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

        public bool SaveUserCompany(string mobile, UserCompanyInfoVM company, List<string> pics)
        {
            using (var ctx = new ShtxSms2008Entities())
            {
                try
                {
                    var cb = ctx.CustomerBases.FirstOrDefault(o => o.SendInterFace == 102 && o.Tel.Contains(mobile));
                    if (cb != null)
                    {
                        cb.AppAddressDesc = company.AppAddressDesc;
                        cb.AppBusinessDesc = company.AppBusinessDesc;
                        cb.AppCity = company.AppCity;
                        cb.AppCompanyName = company.AppCompanyName;
                        if (pics.Count > 0)
                        {
                            var str = string.Join("|||", pics);
                            if (!string.IsNullOrWhiteSpace(cb.AppCompanyPics))
                            {
                                cb.AppCompanyPics += "|||";
                            }
                            cb.AppCompanyPics += str;
                        }
                        cb.AppCustomerName = company.AppCustomerName;
                        cb.AppIndustry = company.AppIndustry;
                        cb.AppProduct = company.AppProduct;
                        cb.AppProvince = company.AppProvince;
                        cb.IsOpenMsg = company.IsOpenMsg;
                        cb.AppTel = company.AppTel;
                        ctx.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
                
            }
        }
    }
}
