using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Service.ViewModel;
using System.Transactions;
using System.IO;

namespace Service
{
    public class TradeService
    {
        public bool saveTrade(TradeVM tradeVm, string tradePics)
        {
            bool flag = false;

            using (var tran = new TransactionScope())
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    try
                    {
                        Supply supply = new Supply();
                        supply.buissnes = tradeVm.Buissnes;//所属行业
                        supply.Product = tradeVm.Product;//名称
                        supply.Quantity = tradeVm.Quantity;
                        supply.Price = tradeVm.Price;
                        supply.Contact = tradeVm.Contact;
                        supply.Mobile = tradeVm.ContactTel;
                        supply.Description = tradeVm.Description;
                        supply.DocumentType = tradeVm.DocumentType;
                        supply.provinceName = tradeVm.Province;
                        supply.cityName = tradeVm.City;
                        supply.Creater = tradeVm.Mobile;
                        supply.CreateDate = DateTime.Now;

                        ctx.Supplies.Add(supply);

                        if (!string.IsNullOrWhiteSpace(tradePics))
                        {
                            string[] pics = tradePics.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var pic in pics)
                            {
                                Image image = new Image();
                                image.SupplyID = supply.ID;
                                image.Name = pic;
                                var path = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase,"upload");
                                var picPath = Path.Combine(path, pic);
                                var path1 = Path.GetFileNameWithoutExtension(picPath) + "-1" + Path.GetExtension(picPath);

                                System.Drawing.Image imgOutput = System.Drawing.Bitmap.FromFile(picPath);
                                System.Drawing.Image imgOutput2 = imgOutput.GetThumbnailImage(60, 60, null, IntPtr.Zero);
                                imgOutput2.Save(path1);

                                ctx.Images.Add(image);
                            }
                        }

                        ctx.SaveChanges();

                        tran.Complete();
                        flag = true;
                    }
                    catch (Exception)
                    {

                    }

                }

            }

            return flag;
        }

        public List<TradeVM> GetMoreTradesById(int id, int documentType, string searchKey, string area, string business,string action)
        {
            var list = new List<TradeVM>();
            using (var ctx = new ShtxSms2008Entities())
            {
                List<Supply> supplies = new List<Supply>();
                var query = ctx.Supplies.AsQueryable();
                query = query.Where(o => o.isChecked.Value && o.DocumentType == documentType);
                if (searchKey.Trim() != "")
                {
                    query = query.Where(o => o.Product.Contains(searchKey));
                }
                if (area != "地区")
                {
                    query = query.Where(o => o.provinceName == area);
                }
                if (business != "分类")
                {
                    query = query.Where(o => o.buissnes == business);
                }
                if (action.ToLower() == "down")
                {
                    if(id == 0)
                    {
                        query = query.OrderByDescending(o => o.ID).Take(30);
                    }
                    else
                    {
                        query = query.Where(o => o.ID > id).OrderByDescending(o => o.ID).Take(30);
                    }
                }
                else
                {
                    if (id == 0)
                    {
                        query = query.OrderByDescending(o => o.ID).Take(30);
                    }
                    else
                    {
                        query = query.Where(o => o.ID < id).OrderByDescending(o => o.ID).Take(30);
                    }
                }
                supplies = query.ToList();
                foreach (var supply in supplies)
                {
                    TradeVM vm = new TradeVM();
                    vm.Id = supply.ID;
                    vm.Buissnes = supply.buissnes;
                    vm.City = supply.cityName;
                    vm.Product = supply.Product;
                    vm.Quantity = supply.Quantity;
                    vm.Price = supply.Price;
                    vm.Contact = supply.Contact;
                    vm.ContactTel = supply.Mobile;
                    vm.Description = supply.Description;
                    vm.Province = supply.provinceName;
                    vm.Description = supply.Description;
                    vm.Date = supply.CreateDate.ToString("MM-dd");
                    var picNames = ctx.Images.Where(o => o.SupplyID == supply.ID).Select(o => o.Name).ToList();
                    vm.Pics = picNames;
                    list.Add(vm);
                }
            }

            return list;
        }

    }
}
