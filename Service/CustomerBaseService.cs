using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Transactions;

namespace Service
{
    public class CustomerBaseService
    {
        public bool Login(string mobile, string pwd, string token, int phoneType)
        {
            bool loginSuccess = false;
            var flag = false;
            var tokenTel = "";
            var tokenToken = "";
            var exitPhoneType = "0";
            if (mobile.Length != 11)
            {
                //电话号码不正确
                return false;
            }
            using (TransactionScope tran = new TransactionScope())
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    try
                    {
                        loginSuccess = CheckUser(mobile, pwd);
                        if (loginSuccess)
                        {
                            var list = ctx.CustomerBases.Where(o => o.Tel.Contains(mobile));
                            var tokens = ctx.AppCustomerTokens.Where(o => o.tel == mobile).ToList();
                            int count = 0;
                            for (int i = 0; i < tokens.Count(); i++)
                            {
                                if (!string.IsNullOrWhiteSpace(tokens[i].devicetoken))
                                {
                                    count++;
                                }
                            }
                            if (count == 0)
                            {
                                var appCustomerToken = ctx.AppCustomerTokens.FirstOrDefault(o => o.tel == mobile);
                                if (appCustomerToken == null)
                                {
                                    var appToken = new AppCustomerToken();
                                    appToken.tel = mobile;
                                    appToken.salt = 0;
                                    appToken.devicetoken = token;
                                    appToken.updatedate = DateTime.Now;
                                    appToken.PhoneType = phoneType;
                                    ctx.AppCustomerTokens.Add(appToken);
                                    //Log.WriteLog("Login,count=0", "tel=" + mobile + ",token=" + token);
                                }
                                else
                                {
                                    appCustomerToken.tel = mobile;
                                    appCustomerToken.salt = 0;
                                    appCustomerToken.devicetoken = token;
                                    appCustomerToken.PhoneType = phoneType;
                                    appCustomerToken.updatedate = DateTime.Now;
                                }
                            }
                            else
                            {
                                var appToken = ctx.AppCustomerTokens.FirstOrDefault(o => o.tel.Contains(mobile));
                                if (appToken.devicetoken != token)
                                {
                                    ////写一条记录到app——sms
                                    //AddMagForAPPSMS(appToken.tel,appToken.devicetoken);
                                    tokenTel = appToken.tel;
                                    tokenToken = appToken.devicetoken;
                                    appToken.salt = 0;
                                    appToken.devicetoken = token;
                                    appToken.PrePhoneType = appToken.PhoneType;
                                    exitPhoneType = appToken.PrePhoneType.Value.ToString();
                                    flag = true;
                                }
                                appToken.PhoneType = phoneType;
                                appToken.updatedate = DateTime.Now;
                            }
                            ctx.SaveChanges();

                            tran.Complete();



                        }
                        //return loginSuccess;
                    }
                    catch
                    {

                    }
                }
            }

            if (flag)
            {
                //写一条记录到app——sms
                AddMagForAPPSMS(tokenTel, tokenToken, exitPhoneType);
            }
            return loginSuccess;
        }

        public void AddMagForAPPSMS(string tel, string token, string phoneType)
        {
            using (var ctx = new MetalSmsSendEntities())
            {
                var sms = new app_sms();
                sms.mobile = tel;
                sms.SMSTitle = "退出";
                sms.content = token;
                sms.Mid = "admin";
                sms.st = false;
                sms.timestamp = DateTime.Now;
                sms.Url = phoneType;
                ctx.app_sms.Add(sms);
                ctx.SaveChanges();
                //Log.WriteLog("Login,count>0", "tel=" + tel + ",token=" + token);
            }
        }

        public bool CheckTokenByMobile(string tel, string token)
        {
            using (var ctx = new ShtxSms2008Entities())
            {
                var cb = ctx.CustomerBases.FirstOrDefault(o => o.SendInterFace == 102 && o.Tel.Contains(tel));
                if (cb == null)
                {
                    return false;
                }
                else
                {
                    var ce = ctx.CustomerExtend.FirstOrDefault(o => o.Tel == cb.Tel && o.SendInterFace == 102 && (o.Valid ?? false));
                    if (ce == null)
                    {
                        return false;
                    }
                    else
                    {
                        var appToken = ctx.AppCustomerTokens.FirstOrDefault(o => o.tel.Contains(tel));
                        if (appToken.devicetoken != token)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
        }

        public bool CheckUser(string mobile, string pwd)
        {
            using (var ctx = new ShtxSms2008Entities())
            {
                var cb = ctx.CustomerBases.FirstOrDefault(o => o.SendInterFace == 102 && o.Tel.Contains(mobile) && o.Appsecret == pwd);
                if (cb == null)
                {
                    return false;
                }
                else
                {
                    var ce = ctx.CustomerExtend.FirstOrDefault(o => o.Tel == cb.Tel && o.SendInterFace == 102 && (o.Valid ?? false));
                    if (ce == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        public bool CheckUser(string mobile, string pwd, string token)
        {
            var flag = CheckUser(mobile, pwd);
            if (flag)
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    int count = ctx.AppCustomerTokens.Count(o => o.devicetoken == token && o.tel == mobile);
                    //Log.WriteLog("CheckUser", "tel=" + mobile + ",token=" + token);
                    return count > 0;
                }
            }
            else
            {
                return flag;
            }
        }
    }
}
