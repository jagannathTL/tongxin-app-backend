using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Service
{
    public class MessageService
    {
        public bool ClearSaltByMobile(string mobile)
        {
            try
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var cbs = ctx.AppCustomerTokens.Where(o => o.tel.Contains(mobile));
                    foreach (var cb in cbs)
                    {
                        cb.salt = 0;
                    }
                    ctx.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}
