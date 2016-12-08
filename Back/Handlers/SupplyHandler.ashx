<%@ WebHandler Language="C#" Class="SupplyHandler" %>

using System;
using System.Web;
using Service;
using Service.ViewModel;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SupplyHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        if (method == "create")
        {
            try
            {
                var catalogID = int.Parse(context.Request["catalogID"]);
                var quantity = context.Request["quantity"];
                var mobile = context.Request["mobile"];
                var contact = context.Request["contact"];
                var description = context.Request["description"];
                var deliveryType = int.Parse(context.Request["deliveryType"]);
                var province = context.Request["province"];
                var city = context.Request["city"];
                var type = context.Request["type"];
                var product = context.Request["product"];
                var creater = context.Request["createdBy"];
                var price = context.Request["price"];

                var vm = new SupplyVM();
                vm.CatalogId = catalogID;
                vm.Quantity = quantity;
                vm.Mobile = mobile;
                vm.Contact = contact;
                vm.Description = description;
                vm.DeliveryType = deliveryType == 1 ? true : false;
                vm.Provice = province;
                vm.City = city;
                vm.SupplyType = (type == "1" ? true : false);
                vm.Product = product;
                vm.Creater = creater;
                vm.Price = price;
                vm.Images = new List<string>();

                var path = context.Server.MapPath("~/upload");
                HttpFileCollection files = context.Request.Files;
                if (files.Count > 0)
                {
                    try
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFile f = files[i];
                            var name = Guid.NewGuid().ToString();
                            var imgName = name + Path.GetExtension(f.FileName);
                            var imgName2 = name + "-1" + Path.GetExtension(f.FileName);
                            var imgPath = Path.Combine(path, imgName);
                            var imgPath2 = Path.Combine(path, imgName2);
                            f.SaveAs(imgPath);
                            MemoryStream MemStream = new MemoryStream();
                            System.Drawing.Image imgOutput = System.Drawing.Bitmap.FromFile(imgPath);
                            System.Drawing.Image imgOutput2 = imgOutput.GetThumbnailImage(60, 60, null, IntPtr.Zero);
                            //imgOutput2.Save(System.Web.HttpContext.Current.Server.MapPath("image.png"), System.Drawing.Imaging.ImageFormat.Png);
                            imgOutput2.Save(imgPath2);

                            vm.Images.Add(imgName);
                        }
                    }
                    catch (Exception)
                    {
                        context.Response.Write("{\"result\":\"error\"}");
                    }
                }

                var supplySvc = new SupplyService();
                int ret = supplySvc.createSupply(vm);
                var result = ret == 0 ? "error" : "ok";
                context.Response.Write("{\"result\":\"" + result + "\",\"id\":\"" + ret + "\"}");
            }
            catch (Exception)
            {
                context.Response.Write("{\"result\":\"error\"}");
            }
        }
        else if (method == "getsupply")
        {
            var channel = int.Parse(context.Request["channel"]);
            var creater = context.Request["createdBy"];
            var supplySvc = new SupplyService();
            var list = supplySvc.getSupply(channel, creater);
            var str = GetJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
        else if (method == "getitem")
        {
            var id = int.Parse(context.Request["id"]);
            var supplySvc = new SupplyService();
            var list = supplySvc.getDeliver(id);

            var str = GetItemJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();


        }
        else if (method == "mysupply")
        {
            var mobile = context.Request["mobile"];
            var supplySvc = new SupplyService();
            var list = supplySvc.getMySupply(mobile);
            var str = GetJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();

        }
        else if (method == "deleteSupply")
        {
            var id = int.Parse(context.Request["id"]);
            var supplySvc = new SupplyService();
            var flag = supplySvc.deleteSupply(id);
            var result = flag ? "ok" : "error";
            context.Response.Write("{\"result\":\"" + result + "\"}");
        }
        else if (method == "addTrade")
        {
            //var mobile = context.Request["mobile"].ToString();
            //var documentType = Convert.ToInt32(context.Request["documentType"]);
            //var buissnes = context.Request["buissnes"].ToString();//所属行业
            //var product = context.Request["product"].ToString();//名称
            //var quantity = context.Request["quantity"].ToString();//数量
            //var price = context.Request["price"].ToString();//价格
            //var tel = context.Request["tel"].ToString();//联系方式
            //var contact = context.Request["contact"].ToString();//联系人
            //var province = context.Request["province"].ToString();//省份
            //var city = context.Request["city"].ToString();//城市
            //var description = context.Request["description"].ToString();//备注
            //var imgs = context.Request["imgs"] == null ? "" : context.Request["imgs"].ToString();//图片


            var mobile = "13524259846";
            var documentType = 0;
            var buissnes = "基本金属";//所属行业
            var product = "名称";//名称
            var quantity = "数量";//数量
            var price = "价格";//价格
            var tel = "联系方式";//联系方式
            var contact = "联系人";//联系人
            var province = "北京";//省份
            var city = "东城区";//城市
            var description = "详细描述";//备注
            var imgs = "";//图片
            
            TradeVM tradeVm = new TradeVM();
            tradeVm.Buissnes = buissnes;
            tradeVm.City = city;
            tradeVm.Contact = contact;
            tradeVm.ContactTel = tel;
            tradeVm.Description = description;
            tradeVm.Mobile = mobile;
            tradeVm.Price = price;
            tradeVm.Product = product;
            tradeVm.Province = province;
            tradeVm.Quantity = quantity;
            tradeVm.DocumentType = documentType;
            var tradeSvc = new TradeService();
            var flag = tradeSvc.saveTrade(tradeVm, imgs);
            var result = flag ? "ok" : "error";
            context.Response.Write("{\"result\":\"" + result + "\"}");
        }
        else if (method == "getTradeList")
        {
            var id = Convert.ToInt32(context.Request["id"]);
            var documentType = Convert.ToInt32(context.Request["documentType"]);
            var searchKey = context.Request["searchKey"].ToString();
            var area = context.Request["area"].ToString();
            var business = context.Request["business"].ToString();
            var action = context.Request["action"].ToString();//up or down
            var list = new TradeService().GetMoreTradesById(id, documentType,searchKey,area,business,action);
            var str = GetMoreTradeList(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
    }

    private string GetMoreTradeList(List<TradeVM> list)
    {
        StringWriter sw = new StringWriter();
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.None;
            writer.WriteStartArray();

            foreach (var trade in list)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("id");
                writer.WriteValue(trade.Id);
                writer.WritePropertyName("product");
                writer.WriteValue(trade.Product);
                writer.WritePropertyName("province");
                writer.WriteValue(trade.Province ?? "");
                writer.WritePropertyName("city");
                writer.WriteValue(trade.City ?? "");
                writer.WritePropertyName("contact");
                writer.WriteValue(trade.Contact);
                writer.WritePropertyName("date");
                writer.WriteValue(trade.Date);
                writer.WritePropertyName("buissnes");
                writer.WriteValue(trade.Buissnes ?? "");
                writer.WritePropertyName("quantity");
                writer.WriteValue(trade.Quantity);
                writer.WritePropertyName("price");
                writer.WriteValue(trade.Price);
                writer.WritePropertyName("tel");
                writer.WriteValue(trade.ContactTel);
                writer.WritePropertyName("description");
                writer.WriteValue(trade.Description);


                writer.WritePropertyName("pics");
                writer.WriteStartArray();
                if (trade.Pics.Count > 0)
                {
                    foreach (var pic in trade.Pics)
                    {
                        writer.WriteValue(pic);
                    }
                }
                writer.WriteEndArray();

                writer.WritePropertyName("pics1");
                writer.WriteStartArray();
                if (trade.Pics.Count > 0)
                {
                    foreach (var pic in trade.Pics)
                    {
                        writer.WriteValue(Path.GetFileNameWithoutExtension(pic) + "-1" + Path.GetExtension(pic));
                    }
                }
                writer.WriteEndArray();
                
                
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.Flush();
            sw.Close();
        }
        return sw.GetStringBuilder().ToString();
    }

    private string GetJson(List<SupplyViewVM> list)
    {
        StringWriter sw = new StringWriter();
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.None;
            writer.WriteStartArray();
            for (int i = 0; i < list.Count; i++)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("id");
                writer.WriteValue(list[i].Id);
                writer.WritePropertyName("avatar");
                writer.WriteValue(list[i].Avatar);
                writer.WritePropertyName("name");
                writer.WriteValue(list[i].Product);
                writer.WritePropertyName("location");
                writer.WriteValue(list[i].Address);
                writer.WritePropertyName("contact");
                writer.WriteValue(list[i].Contact);
                writer.WritePropertyName("date");
                writer.WriteValue(list[i].Date);
                writer.WritePropertyName("type");
                writer.WriteValue(list[i].SupplyType);
                writer.WritePropertyName("ischecked");
                if (list[i].isChecked.HasValue)
                {
                    writer.WriteValue(list[i].isChecked.Value);
                }
                else
                {
                    writer.WriteValue("");
                }
                writer.WritePropertyName("price");
                writer.WriteValue(list[i].Price);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.Flush();
            sw.Close();
        }
        return sw.GetStringBuilder().ToString();
    }

    private string GetItemJson(SupplyDetailVM supply)
    {
        StringWriter sw = new StringWriter();
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.None;

            writer.WriteStartObject();
            writer.WritePropertyName("id");
            writer.WriteValue(supply.Id);

            writer.WritePropertyName("name");
            writer.WriteValue(supply.Product);
            writer.WritePropertyName("location");
            writer.WriteValue(supply.Address);
            writer.WritePropertyName("contact");
            writer.WriteValue(supply.Contact);
            writer.WritePropertyName("type");
            writer.WriteValue(supply.SupplyType);
            writer.WritePropertyName("quantity");
            writer.WriteValue(supply.Quantity);
            writer.WritePropertyName("mobile");
            writer.WriteValue(supply.Mobile);
            writer.WritePropertyName("deliver");
            writer.WriteValue(supply.DeliveryType);
            writer.WritePropertyName("description");
            writer.WriteValue(supply.Description);
            writer.WritePropertyName("price");
            writer.WriteValue(supply.Price);
            writer.WritePropertyName("errorcode");
            writer.WriteValue(supply.ErrorCode);
            writer.WritePropertyName("avatars");
            writer.WriteStartArray();
            if (supply.Avatar != null)
            {
                foreach (var img in supply.Avatar)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("avatar");
                    writer.WriteValue(img);
                    writer.WriteEndObject();
                }
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Flush();
            sw.Close();
        }
        return sw.GetStringBuilder().ToString();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}