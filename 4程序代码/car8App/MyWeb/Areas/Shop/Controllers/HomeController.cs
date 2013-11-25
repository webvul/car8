using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyOql;
using MyWeb.Areas.Shop.Models;
using MyCmn;
using MyCon;
using DbEnt;
using MyBiz;

namespace MyWeb.Areas.Shop.Controllers
{
    public class HomeController : MyMvcController
    {
        //
        // GET: /Shop/Home/

        public ActionResult Index(string WebName)
        {
            IndexModel index = new IndexModel();
            index.Dept = dbr.Dept.FindFirst(o => o.WebName == WebName, o => o._);

            //index.Imgs = dbr.DeptAnnex.Select(o => dbr.Annex.FullName)
            //    .Join(dbr.Annex, (a, b) => a.AnnexID == b.Id)
            //    .Where(o => o.DeptID == index.Dept.Id & o.Key == DeptAnnexKeyEnum.Home)
            //    .ToEntityList("")
            //    .Where(o => o.HasValue())
            //    .Select(o => o.ResolveUrl())
            //    .ToArray();

            index.Imgs = dbr.Annex.Select(o => o.FullName).Where(o => o.Id == index.Dept.Title).ToEntityList("").ToArray();

            //var detail = index.Dept.GetDeptDetails().FirstOrDefault(o => o.GroupKey == ShopMenu.Home && o.Trait == DeptDetailTraitEnum.Default);
            //if (detail != null)
            //{
            //    index.Detail = detail.Value;// HttpUtility.HtmlDecode(dbr.DeptDetail.Value.GetRes(detail.Value));
            //}

            index.ShowCases = dbr.EnterpriseShowCase.Select()
              .Join(dbr.ProductInfo, (a, b) => (a.ProductID == b.Id), o => new ColumnClip[] { o.Name })
              .LeftJoin(dbr.Annex, (a, b) => (dbr.ProductInfo.Logo == b.Id), o => new ColumnClip[] { o.FullName.As("Img") })
              .Where(o => o.DeptID == index.Dept.Id)
              .ToEntityList<IndexModel.ShowCaseModel>()
              .ToArray();

            index.NoticeShowCases = dbr.NoticeShowCase.Select()
  .Join(dbr.NoticeInfo, (a, b) => (a.NoticeID == b.Id), o => new ColumnClip[] { o.Name })
  .LeftJoin(dbr.NoticeType, (a, b) => b.Id == dbr.NoticeInfo.NoticeTypeID, o => o.Name.As("NoticeTypeName"))
  .LeftJoin(dbr.Annex, (a, b) => (dbr.NoticeInfo.Logo == b.Id), o => new ColumnClip[] { o.FullName.As("Img") })
  .Where(o => o.DeptID == index.Dept.Id)
  .ToEntityList<IndexModel.NoticeShowCaseModel>()
  .ToArray();


            return View(index);
        }

        public ActionResult Test(string WebName)
        {
            var ent = new DeptRule.Entity();
            ent =
                dbr.Dept.Select(o => o.GetColumns())
                .Where(o => o.WebName == WebName)
                .ToEntity(ent);

            return View(ent);
        }


        [HttpPost]
        public ActionResult SendMsg(string WebName, FormCollection collection)
        {
            JsonMsg jm = new JsonMsg();

            int deptID = dbr.Dept.FindScalar(o => o.Id, o => o.WebName == WebName).AsInt();
            if (deptID > 0)
            {
                var msg = dbr.ContactMsg.FindFirst(
                    o => o.DeptID == deptID & o.SenderName == collection["Contract"] & o.Subject == "信息" & o.Msg == collection["Msg"]
                    , o => o._);

                if (msg != null)
                {
                    msg.AddTime = DateTime.Now;
                    dbr.ContactMsg.Update(o => o.AddTime == DateTime.Now, o => o.Id == msg.Id).Execute();
                }
                else
                {
                    int res = dbr.ContactMsg
                        .Insert(o => o.DeptID == deptID &
                            o.AddTime == DateTime.Now.ToString() &
                            o.SenderName == collection["Contract"] &
                            o.Subject == "信息" &
                            o.Msg == collection["Msg"] & o.Url == Request.UrlReferrer.ToString())
                        .Execute();

                    if (res != 1)
                    {
                        jm.msg = "消息发送失败，请稍后重试。";
                    }
                }
            }
            else
            {
                jm.msg = "请检查Url是否正确，该网站不存在。";
            }
            return Json(jm);
        }

        public ActionResult About(string WebName)
        {
            var ent = new AboutModel();
            ent.Dept = dbr.Dept.FindFirst(o => o.WebName == WebName, o => o._);

            //ent.Detail = string.Join(
            //    @"<br />", ent.Dept
            //    .GetDeptDetails()
            //    .Where(o => o.Trait == DeptDetailTraitEnum.Default &&
            //        o.GroupKey == ShopMenu.AboutUs)
            //     .OrderBy(o => o.SortID)
            //    .Select(o => o.Value)
            //    .ToArray()
            //  );


            ent.Imgs = ent.Dept.GetDeptAnnexs()
                .Where(o => o.Key == DeptAnnexKeyEnum.AboutUs)
                .OrderBy(o => o.SortID)
                .Select(o => o.GetAnnex().FullName.ResolveUrl())

                .ToArray();

            //ent.Info = ent.Dept.GetDeptDetails()
            //    .Where(o => o.GroupKey == ShopMenu.AboutUs)
            //    .OrderBy(o => o.SortID)
            //    .ToArray();

            return View(ent);
        }

        public ActionResult ShowCase(string WebName, string page)
        {
            var ent = new ProductsModel();
            ent.Dept = dbr.Dept.FindFirst(o => o.WebName == WebName, o => o._);

            WhereClip where = dbr.ProductType.DeptID == ent.Dept.Id &
                 dbr.ProductInfo.Id.In(dbr.EnterpriseShowCase.Select(o => o.ProductID).Where(o => o.DeptID == ent.Dept.Id));

            if (Request.QueryString["Type"].HasValue())
            {
                where &= dbr.ProductType.Id == Request.QueryString["Type"];
            }

            ent.Products = dbr.ProductInfo.Select(o => new ColumnClip[] { o.Name, o.Descr, o.Id })
                .Join(dbr.ProductType, (a, b) => a.ProductTypeID == b.Id, o => new ColumnClip[] { o.Name.As("ProductType") })
                //.LeftJoin(dbr.ProductAnnex, (a, b) => a.Id == b.ProductID & b.Key == ProductAnnexKeyEnum.MinImg)
                .LeftJoin(dbr.Annex, (a, b) => a.Logo == b.Id, o => new ColumnClip[] { o.FullName.As("Img") })
                .Where(where)
                .Take(12)
                .Skip((page.AsInt() - 1) * 12)
                .OrderBy(o => o.SortID.Asc)
                .ToEntityList<ProductsModel.ProductsDetail>();


            ent.Count =
                dbr.ProductInfo.Select(o => new ColumnClip[] { o.Count() })
                .Join(dbr.ProductType, (a, b) => a.ProductTypeID == b.Id)
                .Where(where)
                .ToScalar()
                .AsInt();
            ent.Count = ent.Count % 12 == 0 ? ent.Count / 12 : ent.Count / 12 + 1;

            return View("Products", ent);
        }

        public ActionResult Products(string WebName, string page)
        {
            var ent = new ProductsModel();
            ent.Dept = dbr.Dept.FindFirst(o => o.WebName == WebName, o => o._);

            WhereClip where = dbr.ProductType.DeptID == ent.Dept.Id;

            if (Request.QueryString["Type"].HasValue())
            {
                where &= dbr.ProductType.Id == Request.QueryString["Type"];
            }

            ent.Products = dbr.ProductInfo.Select(o => new ColumnClip[] { o.Name, o.Descr, o.Id })
                .LeftJoin(dbr.ProductType, (a, b) => a.ProductTypeID == b.Id, o => new ColumnClip[] { o.Name.As("ProductType") })
                .LeftJoin(dbr.Annex, (a, b) => a.Logo == b.Id, o => new ColumnClip[] { o.FullName.As("Img") })
                .Where(where)
                .Take(12)
                .Skip((page.AsInt() - 1) * 12)
                .OrderBy(o => o.SortID.Asc)
                .ToEntityList<ProductsModel.ProductsDetail>();


            ent.Count =
                dbr.ProductInfo.Select(o => new ColumnClip[] { o.Count() })
                .Join(dbr.ProductType, (a, b) => a.ProductTypeID == b.Id)
                .Where(where)
                .ToScalar()
                .AsInt();
            ent.Count = ent.Count % 12 == 0 ? ent.Count / 12 : ent.Count / 12 + 1;


            RenderOriJs("webPager", string.Format("$('.WebPager').MyPager({{total:{0},page:{1},href:'{2}'}});"
            , ent.Count
            , MyHelper.RequestContext.RouteData.Values["page"].AsInt()
            , MyHelper.GetHtmlUrl(@"~/Shop/Products/" + ent.Dept.WebName + "/{0}.aspx" +
                (Request.QueryString["Type"].HasValue() ? "?Type=" + Request.QueryString["Type"] : ""),
                Request.QueryString["Html"].AsBool() ? MyMvcPage.MyHtmlTranslater : null)
            ));

            return View(ent);
        }

        public ActionResult Notice(string WebName, string page)
        {
            var ent = new NoticeModel();
            ent.Dept = dbr.Dept.FindFirst(o => o.WebName == WebName, o => o._);

            WhereClip where = dbr.NoticeType.DeptID == ent.Dept.Id;

            if (Request.QueryString["Type"].HasValue())
            {
                where &= dbr.NoticeType.Id == Request.QueryString["Type"];
            }

            ent.Notices = dbr.NoticeInfo.Select(o => new ColumnClip[] { o.Name, o.Descr, o.Id })
                .LeftJoin(dbr.NoticeType, (a, b) => a.NoticeTypeID == b.Id, o => new ColumnClip[] { o.Name.As("NoticeType") })
                .LeftJoin(dbr.Annex, (a, b) => a.Logo == b.Id, o => new ColumnClip[] { o.FullName.As("Img") })
                .Where(where)
                .Take(12)
                .Skip((page.AsInt() - 1) * 12)
                .OrderBy(o => o.SortID.Asc)
                .ToEntityList<NoticeModel.NoticeDetail>();


            ent.Count =
                dbr.NoticeInfo.Select(o => new ColumnClip[] { o.Count() })
                .Join(dbr.NoticeType, (a, b) => a.NoticeTypeID == b.Id)
                .Where(where)
                .ToScalar()
                .AsInt();
            ent.Count = ent.Count % 12 == 0 ? ent.Count / 12 : ent.Count / 12 + 1;


            RenderOriJs("webPager", string.Format("$('.WebPager').MyPager({{total:{0},page:{1},href:'{2}'}});"
            , ent.Count
            , MyHelper.RequestContext.RouteData.Values["page"].AsInt()
            , MyHelper.GetHtmlUrl(@"~/Shop/Notice/" + ent.Dept.WebName + "/{0}.aspx" +
                (Request.QueryString["Type"].HasValue() ? "?Type=" + Request.QueryString["Type"] : ""),
                Request.QueryString["Html"].AsBool() ? MyMvcPage.MyHtmlTranslater : null)
            ));

            return View(ent);
        }

        public ActionResult ProductInfo(string WebName, string id)
        {
            var ent = new ProductInfoModel();
            var dt = DateTime.Today;
            var clicks = dbr.ProductClicks.SelectWhere(o => o.Year == dt.Year & o.Month == dt.Month & o.ProductID == id.AsInt());
            ent.Dept = dbr.Dept.FindFirst(o => o.WebName == WebName, o => o._);

            ent.ProductInfo = dbr.ProductInfo.FindById(id.AsInt());
            ent.Imgs = ent.ProductInfo.GetProductAnnexs().Select(o => o.GetAnnex().FullName.ResolveUrl()).ToArray();
            return View(ent);
        }
        public ActionResult NoticeInfo(string WebName, string page)
        {
            var ent = new NoticeInfoModel();
            var dt = DateTime.Today;
            var clicks = dbr.ProductClicks.SelectWhere(o => o.Year == dt.Year & o.Month == dt.Month & o.ProductID == page.AsInt());
            ent.Dept = dbr.Dept.FindFirst(o => o.WebName == WebName, o => o._);

            ent.NoticeInfo = dbr.NoticeInfo.FindById(page.AsInt());
            //ent.Imgs = ent.NoticeInfo.GetNoticeAnnexs().Select(o => o.GetAnnex().FullName.ResolveUrl()).ToArray();
            return View(ent);
        }
    }
}
