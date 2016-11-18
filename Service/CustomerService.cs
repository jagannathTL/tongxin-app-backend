using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Transactions;
using System.Text.RegularExpressions;

namespace Service
{
    public enum TrialError
    {
        OK = 0,
        Registered = 1,
        Invalid = 2,
        TooManyTrial = 3,
        SystemError = 4
    }
    public class CustomerService
    {
        static object obj = new object();
        public TrialError canotAdd(string mobile)
        {
            //手机号是否合法
            string dianxin = @"^1[3578][01379]\d{8}$"; 
            Regex dReg = new Regex(dianxin);        
            //联通手机号正则        
            string liantong = @"^1[34578][01256]\d{8}$";        
            Regex tReg = new Regex(liantong);        
            //移动手机号正则        
            string yidong = @"^(134[012345678]\d{7}|1[34578][012356789]\d{8})$";        
            Regex yReg = new Regex(yidong);

            if (mobile.Length < 11)
                return TrialError.Invalid;

            if (!dReg.IsMatch(mobile.Substring(0, 11)) && !tReg.IsMatch(mobile.Substring(0, 11)) && !yReg.IsMatch(mobile.Substring(0, 11)))
            {
                return TrialError.Invalid;
            }

            using (var ctx = new ShtxSms2008Entities())
            {
                //var count = ctx.CustomerBases.Count(o => o.Tel.Contains(mobile) && o.SendInterFace == 102);
                //if (count > 0)
                //    return TrialError.Registered;
                App_user user = ctx.App_user.FirstOrDefault(o => o.Tel == mobile);
                if (user != null)
                {
                    if (user.applyCount >= 1)
                        return TrialError.TooManyTrial;
                    else
                        return TrialError.OK;
                }
                else
                {
                    return TrialError.OK;
                }
            }
        }

        public TrialError addTrial(string mobile)
        {
            string tel;

            if (mobile.Length != 11)
            {
                return TrialError.Invalid;
            }

            lock (obj)
            {
                TrialError error = canotAdd(mobile);
                if (error == TrialError.OK)
                {
                    try
                    {
                        string pwd = createPwd();
                        using (TransactionScope tran = new TransactionScope())
                        {
                            using (var ctx = new ShtxSms2008Entities())
                            {
                                var cbs = ctx.CustomerBases.Where(o => o.Tel.Contains(mobile)).OrderByDescending(o => o.Tel).ToList();
                                CustomerBase maxCb = cbs.FirstOrDefault();
                                if (cbs.Count() > 0)
                                {
                                    var maxTel = maxCb.Tel;
                                    if (maxTel == mobile)
                                    {
                                        tel = mobile + "-01";
                                    }
                                    else
                                    {
                                        var suf = int.Parse(maxTel.Split(new char[] { '-' })[1]);
                                        tel = mobile + "-" + ((suf + 1).ToString().PadLeft(2, '0'));
                                    }
                                }
                                else
                                {
                                    tel = mobile;
                                }

                                CustomerExtend maxCe = null;
                                if (maxCb != null)
                                {
                                    maxCe = ctx.CustomerExtend.FirstOrDefault(o => o.Tel == maxCb.Tel);
                                }

                                CustomerBase cb = new CustomerBase();
                                cb.Tel = tel;
                                cb.Name = (maxCb == null ? "app试用用户" : maxCb.Name);
                                cb.CompanyName = (maxCb == null ? "app试用用户" : maxCb.CompanyName);
                                cb.SendInterFace = 102;
                                cb.BargainID = 0;
                                cb.Province = (maxCb == null ? 3 : maxCb.Province);
                                cb.Sort = 0;
                                
                                cb.Appsecret = pwd;
                                cb.ProductLine = 1;

                                CustomerExtend ce = new CustomerExtend();
                                ce.Tel = tel;
                                DateTime today = DateTime.Today;
                                ce.FirstDate = today;
                                ce.UpdateDate = DateTime.Now;
                                ce.EndDate = today.AddDays(14);
                                ce.IsPayment = false;
                                ce.CusTerm = 0;
                                ce.UnitPrice = 0;
                                ce.TotalCon = 0;
                                ce.CusKind = 2;
                                ce.Valid = true;
                                ce.Mid = (maxCe == null ? "admin" : maxCe.Mid);
                                ce.Defer = 0;
                                ce.SendInterFace = 102;
                                ce.BargainID = 0;
                                ce.RoleID = 15;
                                ce.EnFlag = 1;
                                ce.CusSendAttribute = 1;
                                ce.SendRank = 10;
                                ce.ExtendID = Guid.NewGuid();

                                App_user user = ctx.App_user.Where(u => u.Tel == mobile).FirstOrDefault();
                                if (user != null){
                                    user.applyCount++;
                                }
                                else
                                {
                                    user = new App_user();
                                    user.Tel = mobile;
                                    user.applyCount = 1;
                                    ctx.App_user.Add(user);
                                }

                                ctx.CustomerBases.Add(cb);
                                ctx.CustomerExtend.Add(ce);
                         
                                ctx.SaveChanges();
                                tran.Complete();
                            }
                        }
                        sendSms(mobile, getMsg(mobile, pwd));
                        return TrialError.OK;
                    }
                    catch
                    {
                        return TrialError.SystemError;
                    }
                }
                else { return error; }
            }
        }

        private string createPwd()
        {
            Random rad = new Random();
            int value = rad.Next(1000, 10000);
            return value.ToString();
        }

        private void sendSms(string mobile, string msg)
        {
            using (var ctx = new MetalSmsSendEntities())
            {
                ProvideSm sms = new ProvideSm();
                sms.Tel = mobile;
                sms.Message = msg;
                sms.AddDate = DateTime.Now;
                sms.SendInt = 0;
                sms.Mid = "admin";
                sms.SMSTitle = "上海同鑫";
                sms.st = false;
                sms.Stop_Flag = false;
                ctx.ProvideSms.Add(sms);
                ctx.SaveChanges();
            }
        }

        private string getMsg(string mobile, string pwd)
        {
            using (var ctx = new ShtxSms2008Entities())
            {
                var smsRoles = ctx.SmsRoles.FirstOrDefault(o => o.RoleType == 106);
                if (smsRoles != null)
                {
                    var str = smsRoles.RoleDesc;
                    str = str.Replace("CustomerMobile", mobile).Replace("AppPassWord",pwd);
                    return str;
                }
                else
                {
                    return "您好，欢迎使用同鑫资讯移动客户端, 用户名：" + mobile + " 密码：" + pwd;
                }
            }
        }

        public bool sendPwd(string mobile)
        {
            bool flag = false;
            if (mobile.Length != 11)
                return flag;

            using (var ctx = new ShtxSms2008Entities())
            {
                var cb = ctx.CustomerBases.FirstOrDefault(o => o.Tel.Contains(mobile) && o.SendInterFace == 102);
                if (cb != null)
                {
                    if (!string.IsNullOrWhiteSpace(cb.Appsecret))
                    {
                        sendSms(mobile, "您同鑫资讯移动客户端的登陆密码为:" + cb.Appsecret);
                        flag = true;
                    }
                }
            }
            return flag;
        }
    }
}
