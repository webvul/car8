using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using MyBiz;

namespace MyCon
{
    public partial class MyMvcVuc : ViewUserControl
    {
        private RenderResponse _ronse = null;
        public RenderResponse Ronse
        {
            get
            {
                if (_ronse == null) _ronse = new RenderResponse();
                return _ronse;
            }
            set { _ronse = value; }
        }

        public string GetRes(params string[] res)
        {
            return res[0];
        }
    }
}
