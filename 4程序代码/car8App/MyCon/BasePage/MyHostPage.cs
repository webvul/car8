// 
//  MyHostPage.cs
//  
//  Author:
//       newsea <iamnewsea@yahoo.com.cn>
// 
//  Copyright (c) 2010 newsea

using System;
using MyCon;
using MyCmn;
using MyBiz;
using MyOql;
using System.Collections.Generic;
using DbEnt;
using System.Web;

namespace MyCon
{
 

    public class MyHostPage : MyMvcPage
    {
        protected override void OnPreInit(EventArgs e)
        {

            var mod = dbr.Dept.SelectWhere(o => o.Id == 1).SkipPower().ToEntity(o => o._);
            if (mod != null)
            {
                this.Theme = mod.MySkin.AsString(null) ?? "Host";

                var controller = HttpContext.Current.CurrentHandler as MyMvcController;
                //controller.RegisterUsingFile("__myskinvar", "~/App_Themes/Host/SkinVar.js");

                this.MetaNodes.AddRange(PatchHeader(mod));
            }
            base.OnPreInit(e);
        }
    }
}
