using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCmn;

namespace DbEnt
{
    public enum ExtAnnexKeyEnum
    {
        [MyDesc("首页简介")]
        Home,
        [MyDesc("信任文件")]
        Profile,
        [MyDesc("关于我们")]
        AboutUs,
        [MyDesc("联系我们")]
        Contact,

        [MyDesc("产品图片")]
        Img ,
        [MyDesc("产品小图")]
        MinImg ,

        [MyDesc("我的照片")]
        MyPic ,
    }
}
