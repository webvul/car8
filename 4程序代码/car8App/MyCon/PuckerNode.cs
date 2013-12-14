using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using MyCmn;
using MyOql;
using MyBiz.Sys;
using System.Web.Mvc;
using DbEnt;
using MyBiz;

namespace MyCon
{
    /// <summary>
    /// 仅返回 li 元素.
    /// </summary>
    public class PuckerNode
    {
        public string Css { get; set; }
        public string Uid { get; set; }
        public string Text { get; set; }
        public string Tooltip { get; set; }
        public string Href { get; set; }
        public List<PuckerNode> SubNode { get; set; }

        public PuckerNode()
        {
            this.SubNode = new List<PuckerNode>();
        }

        public PuckerNode(MenuRule.Entity MenuEntity)
            : this()
        {
            //默认CSS
            //this.Css = "one";
            this.Uid = MenuEntity.Id.ToString();
            this.Text = MenuEntity.Text;

            if (MenuEntity.Url.AsString().Trim().HasValue())
            {
                this.Href = MenuEntity.Url.GetUrlFull();
            }

            if (this.Text == "-")
            {
                this.Css = "split";
            }
        }

        /// <summary>
        /// 根据 MenuID 加载 菜单树. 如果MenuID 是0 , 则 SubNode 有效.
        /// </summary>
        /// <param name="MenuID"></param>
        public static PuckerNode LoadMenu(int MenuID)
        {
            WhereClip where = null;

            //if (MySession.IsSysAdmin() == false)
            //{
            //    if (MySessionKey.CommType.Get().ToEnum(CommunityTypeEnum.Normal) != CommunityTypeEnum.Club)
            //    {
            //        where = ("," + dbr.Menu.Name + ",").StringIndex("Pm") > 0;
            //    }
            //    else
            //    {
            //        where =  ("," + dbr.Menu.Name + ",").StringIndex("Club") > 0;
            //    }
            //}
            using (var scope = new MyOqlConfigScope(MySession.IsSysAdmin() ? ReConfigEnum.SkipPower : 0))
            {
                var menus = dbr.Menu
                    .SelectWhere(o => (("," + o.Name + ",").Contains(",Shop,") | o.Name == null) & ("," + o.Wbs + ",").Contains("," + MenuID.ToString() + ",") & o.Status != IsAbleEnum.Disable & where)
                    .OrderBy(o => o.SortID.Asc)
                    .ToEntityList(o => o._);

                //本身菜单项及子项

                Func<int, PuckerNode> _LoadPucks = null;
                Func<int, PuckerNode> LoadPucks = (Id) =>
                {
                    var Node = new PuckerNode();
                    var menuItem = menus.FirstOrDefault(o => o.Id == Id);
                    if (menuItem != null)
                    {
                        Node = new PuckerNode(menuItem);
                    }

                    var subItems = menus.Where(o => o.Pid == Id);

                    subItems.All(o =>
                    {
                        var subOne = _LoadPucks(o.Id);
                        Node.SubNode.Add(subOne);
                        return true;
                    });

                    //if (Node.SubNode.Count > 0) Node.Css = "hero";

                    return Node;

                };
                _LoadPucks = LoadPucks;

                return LoadPucks(MenuID);
            }
        }

        /// <summary>
        /// 根据 MenuID 加载 菜单树. 如果MenuID 是0 , 则 SubNode 有效.
        /// </summary>
        /// <param name="MenuID"></param>
        public static PuckerNode LoadMenuByStandardRole(int roleId)
        {
            var row = new PowerJson(dbr.Role.FindById(roleId).Power).Row;//
            //.Select(o => o.Power)
            //.Where(o => o.Id == roleId)
            //.ToEntity(string.Empty)).Row;

            MyBigInt myMenus = new MyBigInt();
            if (row != null && row.View != null && row.View.ContainsKey("Menu"))
            {
                myMenus = row.View["Menu"];
            }


            var menus = dbr.Menu
                .SelectWhere(o => o.Id.In(myMenus.ToPositions().ToArray()) & o.Status != IsAbleEnum.Disable)
                .OrderBy(o => o.SortID.Asc)
                .SkipPower().SkipLog()
                .ToEntityList(o => o._);

            //本身菜单项及子项

            Func<int, PuckerNode> _LoadPucks = null;
            Func<int, PuckerNode> LoadPucks = (Id) =>
            {
                var Node = new PuckerNode();
                var menuItem = menus.FirstOrDefault(o => o.Id == Id);
                if (menuItem != null)
                {
                    Node = new PuckerNode(menuItem);
                }

                var subItems = menus.Where(o => o.Pid == Id);

                subItems.All(o =>
                {
                    var subOne = _LoadPucks(o.Id);
                    Node.SubNode.Add(subOne);
                    return true;
                });

                //if (Node.SubNode.Count > 0) Node.Css = "hero";

                return Node;

            };
            _LoadPucks = LoadPucks;

            return LoadPucks(0);
        }


        public HtmlGenericControl ToLi(bool useHref)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");

            if (this.Text == "-")
            {
                li.Attributes["class"] = this.Css;
                return li;
            }
            HtmlGenericControl div1 = new HtmlGenericControl("div");
            HtmlAnchor a1 = new HtmlAnchor();
            HtmlGenericControl span1 = new HtmlGenericControl("span");

            if (Css.HasValue())
            {
                span1.Attributes["class"] = Css;
            }

            li.Controls.Add(div1);
            div1.Controls.Add(a1);
            a1.Controls.Add(span1);

            a1.Controls.Add(new System.Web.UI.WebControls.Literal() { Text = this.Text });
            if (Href.HasValue())
            {
                if (useHref)
                {

                    a1.HRef = Href;
                }
                else
                {
                    a1.HRef = "javascript:void(0);";
                    a1.Attributes["url"] = Href;
                }
            }

            a1.Title = Tooltip;
            a1.Attributes["uid"] = Uid;

            if (SubNode.Count > 0)
            {
                HtmlGenericControl ul = new HtmlGenericControl("ul");

                foreach (var item in SubNode)
                {
                    ul.Controls.Add(item.ToLi(useHref));
                }

                li.Controls.Add(ul);
            }

            return li;
        }

        public override string ToString() { return ToString(true); }

        public string ToString(bool useHref)
        {
            return ToLi(useHref).ToRenderString();
        }
    }
}

