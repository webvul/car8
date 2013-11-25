// 
//  MyShopPage.cs
//  
//  Author:
//       newsea <iamnewsea@yahoo.com.cn>
// 
//  Copyright (c) 2010 newsea

using System;
using MyCon;
using System.Collections.Generic;
using MyCmn;
using System.Web.Mvc;
using MyBiz;
using MyOql;
using DbEnt;
using System.Web;
using System.IO;

namespace MyCon
{
    public interface IShopModel
    {
        DeptRule.Entity Dept { get; set; }
    }

    /// <summary>
    /// 对于要求比较高的精细页面，每个页面，只引用一个Js和Css 是必要的。
    /// </summary>
    public class MyShopPage : MyMvcPage
    {
        public override string Theme
        {
            get
            {
                return string.Empty;
            }
            set
            {
                this.Skin = value;
            }
        }

        public string Skin { get; set; }

        public override List<HtmlNode> GetInitJs()
        {
            List<HtmlNode> retVal = new List<HtmlNode>();
            retVal.AddRange(GetUsingFile("~/Res/MyJs.js"));
            return retVal;
        }

        protected override void OnPreInit(EventArgs e)
        {
            var mod = Model as IShopModel;
            if (mod != null)
            {
                this.Skin = mod.Dept.MySkin;

                //加载自定义皮肤
                var path = Server.MapPath("~/Areas/" + MyHelper.Area + "/Skin/" + this.Skin);
                if (Directory.Exists(path))
                {
                    var cssBasePath = path + Path.DirectorySeparatorChar + Controller + "." + Action + ".css";
                    var jsBasePath = path + Path.DirectorySeparatorChar + Controller + "." + Action + ".js";
                    if (File.Exists(cssBasePath))
                    {
                        this.RegisterUsingFile("__MySkinBaseCss__", "~/Areas/" + MyHelper.Area + "/Skin/" + this.Skin + "/" + Controller + ".css");
                    }
                    if (File.Exists(jsBasePath))
                    {
                        this.RegisterUsingFile("__MySkinBaseJs__", "~/Areas/" + MyHelper.Area + "/Skin/" + this.Skin + "/" + Controller + ".js");
                    }


                    var cssPath = path + Path.DirectorySeparatorChar + Controller + "." + Action + ".css";
                    var jsPath = path + Path.DirectorySeparatorChar + Controller + "." + Action + ".js";
                    if (File.Exists(cssPath))
                    {
                        this.RegisterUsingFile("__MySkinCss__", "~/Areas/" + MyHelper.Area + "/Skin/" + this.Skin + "/" + Controller + "." + Action + ".css");
                    }
                    if (File.Exists(jsPath))
                    {
                        this.RegisterUsingFile("__MySkinJs__", "~/Areas/" + MyHelper.Area + "/Skin/" + this.Skin + "/" + Controller + "." + Action + ".js");
                    }
                }

                this.MetaNodes.AddRange(PatchHeader(mod.Dept));
            }
            base.OnPreInit(e);
        }
    }
}
