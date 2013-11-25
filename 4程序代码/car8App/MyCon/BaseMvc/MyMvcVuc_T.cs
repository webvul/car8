using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using MyBiz;

namespace MyCon
{
    public partial class MyMvcVuc<TModel> : MyMvcVuc
    {
        // Fields
        private AjaxHelper<TModel> _ajaxHelper;
        private HtmlHelper<TModel> _htmlHelper;
        private ViewDataDictionary<TModel> _viewData;

        // Methods
        protected override void SetViewData(ViewDataDictionary viewData)
        {
            this._viewData = new ViewDataDictionary<TModel>(viewData);
            base.SetViewData(this._viewData);
        }

        // Properties
        public new AjaxHelper<TModel> Ajax
        {
            get
            {
                if (this._ajaxHelper == null)
                {
                    this._ajaxHelper = new AjaxHelper<TModel>(base.ViewContext, this);
                }
                return this._ajaxHelper;
            }
        }

        public new HtmlHelper<TModel> Html
        {
            get
            {
                if (this._htmlHelper == null)
                {
                    this._htmlHelper = new HtmlHelper<TModel>(base.ViewContext, this);
                }
                return this._htmlHelper;
            }
        }

        public new TModel Model
        {
            get
            {
                return this.ViewData.Model;
            }
        }

        public new ViewDataDictionary<TModel> ViewData
        {
            get
            {
                base.EnsureViewData();
                return this._viewData;
            }
            set
            {
                this.SetViewData(value);
            }
        }
    }
}
