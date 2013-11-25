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
using System.Diagnostics.CodeAnalysis;

namespace MyCon
{
    public class MyShopPage<TModel> : MyShopPage
    {
        private ViewDataDictionary<TModel> _viewData;

        public new AjaxHelper<TModel> Ajax
        {
            get;
            set;
        }

        public new HtmlHelper<TModel> Html
        {
            get;
            set;
        }

        public new TModel Model
        {
            get
            {
                return ViewData.Model;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public new ViewDataDictionary<TModel> ViewData
        {
            get
            {
                if (_viewData == null)
                {
                    SetViewData(new ViewDataDictionary<TModel>());
                }
                return _viewData;
            }
            set
            {
                SetViewData(value);
            }
        }

        public override void InitHelpers()
        {
            base.InitHelpers();

            Ajax = new AjaxHelper<TModel>(ViewContext, this);
            Html = new HtmlHelper<TModel>(ViewContext, this);
        }

        protected override void SetViewData(ViewDataDictionary viewData)
        {
            _viewData = new ViewDataDictionary<TModel>(viewData);

            base.SetViewData(_viewData);
        }
    }
}
