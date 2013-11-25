using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyOql;
using MyCmn;
using System.Web.Mvc;
using DbEnt;

namespace MyBiz.Admin
{
    public class MyMenuNode
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public float SortID { get; set; }
        public string Wbs { get; set; }

        public string Name { get; set; }
        public YesNoEnum State { get; set; }
        public MenuNodeCollection Children { get; set; }

        public override string ToString()
        {
            if (this.Text == "-")
                return @"<li><div class=""menu-split"" /></li>";
            else
                return string.Format(@"<li><a href=""javascript:void(0);"" onclick=""jv.page().menuClick('{0}');"">{1}</a>{2}</li>",
                    Url.GetUrlFull(), Text,
                    Children != null ? Children.ToString() : ""
                );
        }

        public void Bind(List<MenuRule.Entity> entitys, int LevelID)
        {
            //children

            Children = new MenuNodeCollection();
            Children.Bind(entitys, LevelID);
        }
    }

    /// <summary>
    /// 入口
    /// </summary>
    public class MenuNodeCollection
    {
        public void Bind(List<MenuRule.Entity> entitys, int ParentID)
        {
            List<MyMenuNode> data = new List<MyMenuNode>();

            // 过滤级数。
            var myEntitys = entitys.Where(o => o.Pid == ParentID);
            //填充数据
            foreach (var item in myEntitys)
            {
                var menu = new MyMenuNode();
                menu.Text = item.Text;
                menu.Url = item.Url;
                menu.Wbs = item.Wbs;
                menu.SortID = item.SortID;
                menu.ID = item.Id;

                //调用子项  Bind
                menu.Bind(entitys, item.Id);

                data.Add(menu);
            }

            Children = data.ToArray();
        }
        public MyMenuNode[] Children { get; set; }


        public override string ToString()
        {
            if (Children != null && Children.Length > 0)
            {
                return string.Format(@"<ul>{0}</ul>",
                                   Children != null && Children.Length > 0 ? string.Join("", Children.Select(o => o.ToString()).ToArray()) : ""
                                   );
            }
            return string.Join("", Children.Select(o => o.ToString()).ToArray());

        }
    }
}
