//系统自动生成的实体，不能修改。 By: UDI-PC.于新海  At:2013-12-15 11:12:12
using System;
using MyOql;
using MyCmn;
using System.Linq;
using System.Data ;
using System.Data.Common ;
using System.Collections.Generic;
using System.Runtime.Serialization ;
using DbEnt;

namespace DbEnt
{

public class ShopGroupClass
{
    public ProductTypeRule ProductType { get { return new ProductTypeRule(); } }
    public NoticeTypeRule NoticeType { get { return new NoticeTypeRule(); } }
    public NoticeInfoRule NoticeInfo { get { return new NoticeInfoRule(); } }
    public ProductClicksRule ProductClicks { get { return new ProductClicksRule(); } }
    public ContactMsgRule ContactMsg { get { return new ContactMsgRule(); } }
    public ProductAnnexRule ProductAnnex { get { return new ProductAnnexRule(); } }
    public ProductInfoRule ProductInfo { get { return new ProductInfoRule(); } }
    public ProductDetailRule ProductDetail { get { return new ProductDetailRule(); } }
    public SplitRule Split(System.String val,System.String split)
    {
        return new SplitRule(val,split);
    }
    public ShopGroupClass()
    {
    }
}

public class ViewGroupClass
{
    public VPowerActionRule VPowerAction { get { return new VPowerActionRule(); } }
    public VTxtResRule VTxtRes { get { return new VTxtResRule(); } }
    public ViewGroupClass()
    {
    }
}

}