<%@ WebHandler Language="C#" Class="UpdateHandler" %>

using System;
using System.Web;

public class UpdateHandler : IHttpHandler
{


    UpdateMeta updateMeta = new UpdateMeta() { versionCode = 2, versionName = "1.1", description = "新增收件箱功能", download = "api.shtx.com.cn/Handlers/tongxin.apk" };

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        if (method == "checkversion")
        {
            context.Response.Write("{\"versioncode\":" + updateMeta.versionCode + ",\"versionname\":\"" + updateMeta.versionName + "\",\"description\":\"" + updateMeta.description + "\",\"download\":\"" + updateMeta.download + "\"}");
            context.Response.Flush();
            context.Response.End();
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}

class UpdateMeta
{
    public int versionCode { get; set; }
    public string versionName { get; set; }
    public string description { get; set; }
    public string download { get; set; }
}