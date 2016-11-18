using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Service.ViewModel;
using System.Transactions;
using System.Data.Entity.Validation;

namespace Service
{
    public class SupplyService
    {
        public int createSupply(SupplyVM vm)
        {
            int ret = 0;


            using (var tran = new TransactionScope())
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    try
                    {
                        var supply = new Supply();
                        supply.CatalogID = vm.CatalogId;
                        supply.Quantity = vm.Quantity;
                        supply.Mobile = vm.Mobile;
                        supply.Contact = vm.Contact;
                        supply.Description = vm.Description;
                        supply.DeliveryType = vm.DeliveryType;
                        supply.SupplyType = vm.SupplyType;
                        supply.Product = vm.Product;
                        supply.Creater = vm.Creater;
                        supply.Price = vm.Price;
                        //supply.isChecked = false;初始为空，0拒绝，1同意

                        var provice = ctx.Provinces.FirstOrDefault(o => o.Name == vm.Provice);
                        if (provice != null)
                        {
                            var city = ctx.Provinces.FirstOrDefault(o => o.ParentID == provice.ID && o.Name == vm.City);
                            if (city != null)
                                supply.ProviceID = city.ID;
                        }
                        supply.CreateDate = DateTime.Now;
                        ctx.Supplies.Add(supply);
                        ctx.SaveChanges();

                        foreach (var imgName in vm.Images)
                        {
                            Image img = new Image();
                            img.Name = imgName;
                            img.SupplyID = supply.ID;
                            ctx.Images.Add(img);
                        }
                        ctx.SaveChanges();
                        tran.Complete();
                        ret = supply.ID;
                    }
                    catch (DbEntityValidationException)
                    {

                    }
                    catch (Exception)
                    {

                    }

                }
            }

            return ret;
        }

        public List<SupplyViewVM> getSupply(int channelId,string creater)
        {
            var list = new List<SupplyViewVM>();
            using (var ctx = new ShtxSms2008Entities())
            {
                var today = DateTime.Today;
                DateTime tom = today.AddDays(1);
                //DateTime start = today.AddDays(-6);
                DateTime start = today.AddMonths(-6);
                //var supplies = ctx.Supplies.Where(o => o.CatalogID == channelId && o.CreateDate >= start && o.CreateDate < tom && o.isChecked == true).OrderByDescending(o => o.CreateDate);
                List<int> catalogIds = ctx.CataLogs.Where(o => o.ChannelId == channelId).Select(o => o.ID).ToList();
                var supplies = ctx.Supplies.Where(o => catalogIds.Contains(o.CatalogID) && o.CreateDate >= start && o.CreateDate < tom && o.isChecked == true).OrderByDescending(o => o.CreateDate);
                foreach (var supply in supplies)
                {
                    SupplyViewVM vm = new SupplyViewVM();
                    vm.Id = supply.ID;
                    vm.Product = supply.Product;
                    vm.SupplyType = supply.SupplyType;
                    vm.Contact = supply.Contact;
                    vm.isChecked = supply.isChecked;
                    vm.Price = supply.Price;
                    
                    var img = ctx.Images.FirstOrDefault(o => o.SupplyID == supply.ID);
                    if (img != null)
                    {
                        vm.Avatar = "http://api.shtx.com.cn/Upload/" + getAvatarName(img.Name);
                    }
                    else
                    {
                        vm.Avatar = "http://api.shtx.com.cn/Upload/default.jpg";
                    }
                    var city = ctx.Provinces.FirstOrDefault(o => o.ID == supply.ProviceID);
                    var province = ctx.Provinces.FirstOrDefault(o => o.ID == city.ParentID);
                    var address = province.Name + city.Name;
                    vm.Address = address;
                    if (supply.CreateDate >= today)
                    {
                        vm.Date = supply.CreateDate.ToString("HH:mm");
                    }
                    else
                    {
                        vm.Date = supply.CreateDate.ToString("MM-dd");
                    }
                    list.Add(vm);
                }
            }
            return list;
        }

        private string getAvatarName(string name)
        {
            string fileName = "default.jpg";
            if (!string.IsNullOrEmpty(name))
            {
                int index = name.LastIndexOf(".");
                if (index != -1)
                {
                    string start = name.Substring(0, index);
                    string end = name.Substring(index);
                    start += "-1";
                    fileName = start + end;
                }
            }
            return fileName;
        }

        public SupplyDetailVM getDeliver(int id)
        {
            using (var ctx = new ShtxSms2008Entities())
            {
                Supply supply = ctx.Supplies.FirstOrDefault(o => o.ID == id);
                SupplyDetailVM vm = new SupplyDetailVM();
                if (supply == null)
                {
                    vm.ErrorCode = "error";
                    return vm;
                }
                else
                {
                    vm.ErrorCode = "ok";
                }
                
                vm.Id = supply.ID;
                vm.Product = supply.Product;
                vm.SupplyType = supply.SupplyType;
                vm.Contact = supply.Contact;
                var city = ctx.Provinces.FirstOrDefault(o => o.ID == supply.ProviceID);
                var province = ctx.Provinces.FirstOrDefault(o => o.ID == city.ParentID);
                var address = province.Name + city.Name;
                vm.Address = address;
                vm.Quantity = supply.Quantity;
                vm.Mobile = supply.Mobile;
                vm.Description = supply.Description;
                vm.DeliveryType = supply.DeliveryType;
                vm.Price = supply.Price;
                var imgs = ctx.Images.Where(o => o.SupplyID == supply.ID).ToList();
                var list = new List<string>();
                if (imgs.Count > 0)
                {
                    foreach (var img in imgs)
                    {
                        list.Add("http://api.shtx.com.cn/Upload/" + img.Name);
                    }
                }
                else
                {
                    list.Add("http://api.shtx.com.cn/Upload/default.jpg");
                }
                vm.Date = supply.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
                vm.Avatar = list;
                return vm;
            }
        }

        public List<SupplyViewVM> getMySupply(string mobile)
        {
            var list = new List<SupplyViewVM>();
            var today = DateTime.Today;
            using (var ctx = new ShtxSms2008Entities())
            {
                var supplies = ctx.Supplies.Where(o => o.Creater == mobile).OrderByDescending(o=>o.CreateDate).ToList();
                foreach (var supply in supplies)
                {
                    SupplyViewVM vm = new SupplyViewVM();
                    vm.Id = supply.ID;
                    vm.Product = supply.Product;
                    vm.SupplyType = supply.SupplyType;
                    vm.Contact = supply.Contact;
                    vm.isChecked = supply.isChecked;
                    vm.Price = supply.Price;

                    var img = ctx.Images.FirstOrDefault(o => o.SupplyID == supply.ID);
                    if (img != null)
                    {
                        vm.Avatar = "http://api.shtx.com.cn/Upload/" + getAvatarName(img.Name);
                    }
                    else
                    {
                        vm.Avatar = "http://api.shtx.com.cn/Upload/default.jpg";
                    }
                    var city = ctx.Provinces.FirstOrDefault(o => o.ID == supply.ProviceID);
                    var province = ctx.Provinces.FirstOrDefault(o => o.ID == city.ParentID);
                    var address = province.Name + city.Name;
                    vm.Address = address;
                    if (supply.CreateDate >= today)
                    {
                        vm.Date = supply.CreateDate.ToString("HH:mm");
                    }
                    else
                    {
                        vm.Date = supply.CreateDate.ToString("MM-dd");
                    }
                    list.Add(vm);
                }
            }
            return list;
        }

        public bool deleteSupply(int id)
        {
            bool flag = false;

            using (TransactionScope tran = new TransactionScope())
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var images = ctx.Images.Where(o => o.SupplyID == id).ToList();
                    foreach (var image in images)
                    {
                        ctx.Images.Remove(image);
                    }
                    var supply = ctx.Supplies.FirstOrDefault(o => o.ID == id);
                    if (supply != null)
                    {
                        ctx.Supplies.Remove(supply);
                    }
                    ctx.SaveChanges();
                    tran.Complete();
                    flag = true;
                }
            }

            return flag;
        }
    }
}
