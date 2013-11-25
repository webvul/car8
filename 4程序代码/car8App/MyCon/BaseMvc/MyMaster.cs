
using System.Web.Mvc;

namespace MyCon
{
    public class MyMaster<TModel> : ViewMasterPage<TModel>
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


        public string GetRes(string ResKey)
        {
            return ResKey.GetRes();
        }
    }

    public class MyMaster : ViewMasterPage
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


        public string GetRes(string ResKey)
        {
            return ResKey.GetRes();
        }
    }
}
