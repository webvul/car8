using System;
using MyCon;
using System.Web.Mvc;
using DbEnt;
using MyOql;
using MyCmn;
using MyWeb.Areas.Admin.Models;
using MyBiz.Sys;

namespace MyWeb.Areas.Admin.Controllers
{

    /// <summary>
    /// 在URL 传递时, 采用  Type=User&Value=SysAdmin 的方式 , 这样 Key 是固定的. 
    /// 参数:
    /// Type=User,Dept,Role
    /// Value=SysAdmin,Guid,Id
    /// Data=True    启用数据权限
    /// Mode=Add,Minus 在批量处理时, 指定Post上来的数据是 批量添加权限还是批量减少权限.
    /// </summary>
    public partial class PowerController : MyMvcController
    {
        public ActionResult Detail(string uid)
        {
            return Update(uid);
        }

        public ActionResult Add(string uid)
        {
            PowerModel model = new PowerModel(new PowerJson(""));

            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Add(int uid, FormCollection collection)
        {
            var query = collection.ToDictionary();

            JsonMsg jm = new JsonMsg();
            try
            {
                if (new DictRule().Update(query).Execute() == 1)
                {

                }
                else { jm.msg = "未更新记录"; }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }



        public ViewResult Update(string uid)
        {
            PowerOwnerEnum updateType = Request.QueryString["Type"].ToEnum<PowerOwnerEnum>();
            string updateValue = Request.QueryString["Value"];

            return View("Edit");
        }

        public ActionResult UpdateMain(string uid)
        {
            PowerOwnerEnum updateType = Request.QueryString["Type"].ToEnum<PowerOwnerEnum>();
            string updateValue = Request.QueryString["Value"];

            return View("Edit", "Main");
        }

        [HttpPost]
        public ActionResult Update(int uid, FormCollection collection)
        {
            var query = collection.ToDictionary();

            JsonMsg jm = new JsonMsg();
            try
            {
                if (new DictRule().Update(query).Execute() == 1)
                { }
                else { jm.msg = "未更新记录"; }
            }
            catch (Exception ee)
            {
                jm.msg = ee.Message.GetSafeValue();
            }

            return jm;
        }


    }
}