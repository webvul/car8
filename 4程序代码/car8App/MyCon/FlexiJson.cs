using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MyCmn;
using System.Data;
using System.Collections;
using System.Web.Mvc;
using System.Web;
using MyOql;

namespace System.Web.Mvc
{
    public class FlexiJsonBase<T> : ActionResult
        where T : class
    {
        public bool IsTree { get; set; }
        /// <summary>
        /// 总条数.
        /// </summary>
        public long Totle { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 行数据
        /// </summary>
        public List<T> Rows { get; set; }

        /// <summary>
        /// 主要实体，用于权限。
        /// </summary>
        public string Entity { get; set; }

        /// <summary>
        /// 标题.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 错误消息.
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 客户端定义列
        /// </summary>
        public string[] PostColumns { get; private set; }

        /// <summary>
        /// 额外的Js
        /// </summary>
        public string ExtraJs { get; set; }

        /// <summary>
        /// （默认）分组列
        /// </summary>
        public string Sort { get; set; }

        internal ControllerContext Context { get; set; }

        public FlexiJsonBase(bool IsTree, string Entity, long Totle)
        {
            this.IsTree = IsTree;
            this.Entity = Entity;
            this.Totle = Totle;
            this.PostColumns = System.Mvc.Model["FlexiGrid_Cols"].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            this.Rows = new List<T>();
        }

        public override string ToString()
        {
            List<string> extraJs = new List<string>();
            if (this.Context != null)
            {
                var controller = this.Context.Controller as MyCon.MyMvcController;
                if (controller != null)
                {
                    controller.RenderJss.All(o =>
                    {
                        if (o.Key == "_") return true;
                        extraJs.Add(o.Value.Replace(@"""", @"\"""));
                        return true;
                    });
                }
            }
            return string.Format(@"{{""total"":{0},""title"":""{3}"",""entity"":""{4}"",""ErrorMsg"":""{5}"",""isTree"":{1},""ExtraJs"":""{6}"",""sort"":""{7}"",""rows"":[{2}]}}",
                    Totle,
                    this.IsTree ? "true" : "false",
                    string.Join(@",", Rows.Select(o => o.ToString()).ToArray()),
                    Title.GetSafeValue(),
                    Entity,
                    ErrorMsg.GetSafeValue(),
                    string.Join("", extraJs.ToArray()),
                    this.Sort
                    )
                    .Replace(Environment.NewLine, " ").Replace("\n", " ")
                    ;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            this.Context = context;
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.Write(this.ToString());
        }
    }

    /// <summary>
    /// FlexiGrid 专用数据格式.
    /// </summary>
    public class FlexiJson : FlexiJsonBase<FlexiJson.FlexiRow>
    {
        public FlexiRow NewRow(string Id, params string[] Cell)
        {
            return new FlexiRow(this.RawRender, Id, Cell);
        }
        /// <summary>
        /// 哪些列是原始输出
        /// </summary>
        public int[] RawRender { get; set; }

        /// <summary>
        /// FlexiGrid 的每一行数据
        /// </summary>
        public class FlexiRow
        {
            /// <summary>
            /// 哪些列是原始输出
            /// </summary>
            public int[] RawRender { get; set; }

            public string Id { get; set; }
            public string[] Cell { get; set; }

            internal FlexiRow(int[] rawRender, string Id, params string[] Cell)
            {
                this.RawRender = rawRender;

                this.Id = Id.Trim();

                if (Cell != null)
                {
                    this.Cell = Cell.Select(o => string.IsNullOrEmpty(o) ? string.Empty : o.Trim()).ToArray();
                }
            }

            //public FlexiRow(params string[] Cell)
            //{
            //    this.Id = Cell[0];
            //    this.Cell = Cell;
            //}

            public string RemoveBr(string val)
            {
                if (val.HasValue() == false) return val;

                var pos = 0;
                while (true)
                {
                    var brIndex = val.IndexOf("<br", pos, StringComparison.CurrentCultureIgnoreCase);
                    if (brIndex < 0) return val;
                    if (brIndex + 5 >= val.Length) return val;

                    if (val[brIndex + 3] == ' ' && val[brIndex + 4] == '/' && val[brIndex + 5] == '>')
                    {
                        val = val.Remove(brIndex, 6).Insert(brIndex, ",");
                    }
                    else if (val[brIndex + 3] == '/' && val[brIndex + 4] == '>')
                    {
                        val = val.Remove(brIndex, 5).Insert(brIndex, ",");
                    }
                    else if (val[brIndex + 3] == '>')
                    {
                        val = val.Remove(brIndex, 4).Insert(brIndex, ",");
                    }

                    pos = brIndex + 3;
                }
            }

            public override string ToString()
            {
                var cellIndex = -1;
                //GodError.Check(Id.HasValue() == false && (Cell[0].HasValue() == false), () => "FlexiRow 的Id,或第一个单元格 为空,无法确定其Id.");
                return string.Format(@"{{""id"":""{0}"",""cell"":[""{1}""]}}",
                    Id.Trim().AsString(Cell[0].Trim()).HasValue(o => o.GetSafeValue(), o => { throw new GodError("找不到FlexiRow的行Id"); })
                    , string.Join(@""",""", Cell.Select(
                        o =>
                        {
                            cellIndex++;

                            if (RawRender != null && RawRender.Contains(cellIndex)) return o;
                            return RemoveBr(System.Web.HttpUtility.HtmlEncode(RemoveBr(o.AsString())))
                                .GetSafeValue();
                        }
                        )
                        .ToArray()));
            }
        }

        public FlexiJson(string Entity, long Totle)
            : base(false, Entity, Totle)
        {
        }

    }

    /// <summary>
    /// 分页是分的根节点数.
    /// </summary>
    public class FlexiTreeJson : FlexiJsonBase<FlexiTreeJson.FlexiTreeRowData>
    {
        /// <summary>
        /// 哪些列是原始输出
        /// </summary>
        public string[] RawRender { get; set; }

        /// <summary>
        /// 树型表格的行数据.
        /// </summary>
        public class FlexiTreeRowData
        {
            internal FlexiTreeRowData(int level, string[] PostColumns, string[] rawRender)
            {
                this.RawRender = rawRender;
                this.Level = level;
                this.PostColumns = PostColumns;
                this.Rows = new List<FlexiTreeRowData>();
                this.Cell = new StringDict();
            }

            public FlexiTreeRowData AddNewRow()
            {
                var row = new FlexiTreeRowData(this.Level + 1, this.PostColumns, this.RawRender);
                this.Rows.Add(row);
                return row;
            }

            /// <summary>
            /// 哪些列是原始输出
            /// </summary>
            public string[] RawRender { get; set; }


            /// <summary>
            /// 当是Ajax的时候,是否有子节点.
            /// </summary>
            public bool AjaxHasChildren { get; set; }

            /// <summary>
            /// 客户端定义的列.
            /// </summary>
            private string[] PostColumns { get; set; }

            /// <summary>
            /// 行Id ,必须存在.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// 列数据
            /// </summary>
            public StringDict Cell { get; set; }

            /// <summary>
            /// 行数据
            /// </summary>
            public List<FlexiTreeRowData> Rows { get; set; }

            public int Level { get; set; }

            public override string ToString()
            {
                //{
                //    Func<FlexiTreeRowData, string> GenOneRow = row =>
                //        {

                //            var treeRowTemp = @"{{""id"":""{0}"",""cell"":[""{1}""],""level"":{2}{3}}}";
                //            return string.Format(treeRowTemp,
                //                        row.Id.Trim().AsString(row.Cell.First().Key).HasValue(o => o.GetSafeValue(), o => { throw new GodError("找不到FlexiRow的行Id"); }),
                //                        string.Join(@""",""",
                //                        PostColumns.Select(
                //                        o =>
                //                        {
                //                            if (row.Cell.ContainsKey(o))
                //                            {
                //                                return System.Web.HttpUtility.HtmlEncode(row.Cell[o]).GetSafeValue();
                //                            }
                //                            else return "";
                //                        }
                //                            )
                //                        .ToArray()),
                //                        row.Level,
                //                        row.Rows == null ? string.Empty : @",""rows"":" + row.Rows.Count.ToString())
                //                        ;
                //        };

                //    var rows = new List<string>();
                //    if (Rows != null && Rows.Count > 0)
                //    {
                //        new Recursion<FlexiTreeRowData>().Execute(new FlexiTreeRowData[] { this }, o => o.Rows, row =>
                //            {
                //                rows.Add(GenOneRow(row));

                //                return RecursionReturnEnum.Go;
                //            });
                //    }

                //    return string.Join(",", rows.ToArray());
                //}

                string rows = string.Empty;
                if (Rows != null && Rows.Count > 0)
                {
                    rows = string.Format(@",""rows"":[{0}]", string.Join(@",", Rows.Select(o => o.ToString()).ToArray()));
                }
                else if (AjaxHasChildren)
                {
                    rows = @",""rows"":true";
                }

                return string.Format(@"{{""id"":""{0}"",""cell"":[""{1}""]{2}}}",
                     Id.Trim().AsString(Cell.First().Key).HasValue(o => o.GetSafeValue(), o => { throw new GodError("找不到FlexiRow的行Id"); }),
                        string.Join(@""",""",
                            PostColumns.Select(
                            o =>
                            {
                                if (Cell.ContainsKey(o))
                                {
                                    if (RawRender != null && RawRender.Contains(o)) return Cell[o];

                                    return System.Web.HttpUtility.HtmlEncode(Cell[o]).GetSafeValue();
                                }
                                else return string.Empty;
                            }
                                )
                            .ToArray()),
                            rows
                        )
                        .Replace(Environment.NewLine, " ").Replace("\n", " ");
            }
        }

        public FlexiTreeJson(string Entity, long Totle)
            : base(true, Entity, Totle)
        {
        }

        public FlexiTreeRowData AddNewRootRow(int initLevel = 0)
        {
            var row = new FlexiTreeRowData(initLevel, this.PostColumns, this.RawRender);
            this.Rows.Add(row);
            return row;
        }
    }
}
