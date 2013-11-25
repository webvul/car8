using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
using System.Web.UI;
using System.Text;
using MyCmn;
using MyCon;
using System.Xml;
using MyOql;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using MyBiz;
using System.Collections;
using System.Reflection;

namespace System.Web.Mvc
{
    [Flags]
    public enum DisplayStyle
    {
        MyDate = 1,
        DateTime = 2,
    }

    public static partial class MyHtmlHelper
    {
        //三种数据源: List<XmlDictionary<string,string> , MyOqlSet , IEnumerable<S>

        public static FlexiJson LoadFlexiGrid<T, S>(this T Entity, IEnumerable<S> Source, int Totle)
            where T : RuleBase
        {
            if (Totle == 0)
            {
                Totle = Source.Count();
            }

            FlexiJson fr = new FlexiJson(Entity.GetName(), Totle);


            if (Totle == 0)
            {
                return ReturnNoSession(fr);
            }

            string[] Columns = System.Mvc.Model["FlexiGrid_Cols"].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


            string idCol = System.Mvc.Model["FlexiGrid_Id"];


            bool IsDict = false;
            Type type = typeof(S);
            if (type.GetInterface("System.Collections.IDictionary") != null)
            {
                IsDict = true;
            }

            IEnumerable<string> ps = null;
            Dictionary<string, MethodInfo> MethodInfoGetDict = null;

            foreach (S item in Source)
            {
                string idValue = string.Empty;
                List<string> lstValue = new List<string>();
                if (IsDict)
                {
                    var dict = item as IDictionary;
                    idValue = dict[idCol].AsString();
                    Columns.All(o =>
                    {
                        if (dict.Contains(o))
                        {
                            lstValue.Add(dict[o].AsString());
                        }
                        else lstValue.Add("");
                        return true;
                    });
                }
                else
                {
                    var entity = item as IEntity;
                    if (entity != null)
                    {
                        Columns.All(o =>
                        {
                            if (entity.GetProperties().Contains(o))
                            {
                                var propertyValue = entity.GetPropertyValue(o).AsString();
                                if (o == idCol)
                                {
                                    idValue = propertyValue;
                                }

                                lstValue.Add(propertyValue);
                            }
                            else lstValue.Add("");
                            return true;
                        });
                    }
                    else
                    {
                        if (ps == null)
                        {
                            ps = TypeDescriptor.GetProperties(item).ToMyList(o => (o as PropertyDescriptor).Name);
                        }

                        if (MethodInfoGetDict == null)
                        {
                            MethodInfoGetDict = new Dictionary<string, MethodInfo>();
                            Columns.All(o =>
                            {
                                if (ps.Contains(o) == false) return true;

                                MethodInfoGetDict[o] = type.GetMethod("get_" + o);

                                return true;
                            });
                        }

                        Columns.All(o =>
                        {
                            if (ps.Contains(o))
                            {
                                var propertyValue = FastInvoke.GetPropertyValue(item, type, MethodInfoGetDict[o]).AsString();
                                if (o == idCol)
                                {
                                    idValue = propertyValue;
                                }

                                lstValue.Add(propertyValue);
                            }
                            else lstValue.Add("");
                            return true;
                        });
                    }
                }
                fr.Rows.Add(fr.NewRow(idValue, lstValue.ToArray()));
            }

            return fr;
        }

        private static FlexiJson ReturnNoSession(FlexiJson fr)
        {
            if (MySession.Get(MySessionKey.UserID).HasValue() == false)
            {
                fr.ErrorMsg = HttpContext.Current.Items["ReturnNoSession"].AsString(null) ?? @"没有数据，请登录再试";
            }
            //fr.ErrorMsg = "请登录!";
            return fr;
        }

        private static FlexiTreeJson ReturnNoSession(FlexiTreeJson fr)
        {
            if (MySession.Get(MySessionKey.UserID).HasValue() == false)
            {
                fr.ErrorMsg = HttpContext.Current.Items["ReturnNoSession"].AsString(null) ?? @"没有数据，请登录再试";
            }
            //fr.ErrorMsg = "请登录!";
            return fr;
        }


        public static FlexiJson LoadFlexiGrid(this MyOqlSet Set)
        {
            return LoadFlexiGrid(Set, DisplayStyle.MyDate);
        }

        public static FlexiJson LoadFlexiGrid(this MyOqlSet Set, DisplayStyle style)
        {
            FlexiJson fr = new FlexiJson(Set.Entity.GetName(), Set.Count);
            if (Set.OrderBy.HasValue())
            {
                fr.Sort = Set.OrderBy.ToList()
                    .Where(o => !o.Order.Name.StartsWith("__IgNoRe__"))
                    .Select(o => o.Order.Name)
                    .Join(",");
            }


            string[] Columns = System.Mvc.Model["FlexiGrid_Cols"].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


            if (fr.Totle == 0)
            {
                return ReturnNoSession(fr);
            }

            string idCol = System.Mvc.Model["FlexiGrid_Id"];

            int idIndex = Set.Columns.IndexOf(o =>
            {
                if (o.NameEquals(idCol)) return true;
                //else if (ColumnClip.NameEquals(o, Set.Entity.GetIdKey())) return true;
                else return false;
            });

            //List<int> ColIndexs = new List<int>();
            //Columns.All(o =>
            //{
            //    ColIndexs.Add(Set.Columns.IndexOf(p =>
            //    {
            //        return p.NameEquals(o);
            //    }));
            //    return true;
            //});

            //List<int> DateIndexs = new List<int>();

            //if (style.Contains(DisplayStyle.MyDate))
            //{
            //    ColIndexs.All(o =>
            //        {
            //            if (o < 0) return true;
            //            if (Set.Columns[o].DbType.DbTypeIsDateTime())
            //            {
            //                DateIndexs.Add(o);
            //            }
            //            return true;
            //        });
            //}

            foreach (var item in Set.Rows)
            {
                //ColIndexs.All(o =>
                //{
                //    if (o >= 0)
                //    {
                //        if (DateIndexs.Contains(o))
                //        {
                //            lstValue.Add(item[Set.Columns[o].Name].AsMyDate().AsString());
                //        }
                //        else
                //        {
                //            lstValue.Add(item[Set.Columns[o].Name].AsString());
                //        }
                //    }
                //    else lstValue.Add("");
                //    return true;
                //});
                var lstValue = new List<string>();
                Columns.All(o =>
                    {
                        var colIndex = Set.GetColumnIndex(o);
                        if (colIndex < 0) return true;

                        //Flexigrid 可以使用 ColumnName,也可以使用 ColumnDbName
                        var colName = Set.Columns[colIndex].Name;
                        if (Set.Columns[colIndex].DbType.DbTypeIsDateTime())
                        {
                            lstValue.Add(item[colName].AsMyDate().AsString());
                        }
                        else
                        {
                            lstValue.Add(item[colName].AsString());
                        }

                        return true;
                    });


                if (idIndex < 0)
                {
                    fr.Rows.Add(fr.NewRow(lstValue[0], lstValue.ToArray()));//   new FlexiJson.FlexiRow(lstValue.ToArray()));
                }
                else
                {
                    var id = item[Set.Columns[idIndex].Name].AsString();
                    fr.Rows.Add(fr.NewRow(id, lstValue.ToArray()));// new FlexiJson.FlexiRow(id, lstValue.ToArray()));
                }
            }

            return fr;
        }

        public static FlexiTreeJson LoadFlexiTreeGrid<T>(this T Entity, List<StringDict> Source, int Totle, Func<T, string> IDKeyFunc, Func<T, string> PIDKeyFunc)
             where T : RuleBase
        {
            return LoadFlexiTreeGrid<T>(Entity, Source, Totle, IDKeyFunc(Entity), PIDKeyFunc(Entity));
        }

        public static FlexiTreeJson LoadFlexiTreeGrid<T>(this T Entity, List<StringDict> Source, int Totle, string IDKey, string PIDKey)
            where T : RuleBase
        {
            if (Totle == 0)
            {
                Totle = Source.Count();
            }

            var retVal = new FlexiTreeJson(Entity.GetName(), Totle);
            if (retVal.PostColumns.Length == 0) return retVal;

            if (Totle == 0)
            {
                return ReturnNoSession(retVal);
            }

            foreach (var row in Source)
            {
                AddTreeRow(retVal, IDKey, PIDKey, row, pId => Source.First(o => o[IDKey] == pId), null);
            }

            return retVal;
        }

        public static FlexiTreeJson LoadFlexiTreeGrid<T, M>(this T Entity, List<M> Source, int Totle, string IDKey, string PIDKey)
            where T : RuleBase
            where M : IModel
        {
            var model = new List<StringDict>();
            Source.All(o =>
            {
                model.Add(dbo.ModelToStringDict(o));
                return true;
            });

            return LoadFlexiTreeGrid<T>(Entity, model, Totle, IDKey, PIDKey);
        }

        public static FlexiTreeJson LoadFlexiTreeGrid(this MyOqlSet Set, string IDKey, string PIDKey)
        {
            var retVal = new FlexiTreeJson(Set.Entity.GetName(), Set.Count);
            if (retVal.PostColumns.Length == 0) return retVal;

            if (Set.Count == 0)
            {
                return ReturnNoSession(retVal);
            }

            Func<RowData, StringDict> Translate = (vals) =>
            {
                var dict = new StringDict();
                if (vals == null || vals.Count == 0) return dict;

                foreach (var key in vals.Keys)
                {
                    dict[key] = vals[key].AsString();
                }
                return dict;
            };


            foreach (var row in Set.Rows)
            {
                AddTreeRow(retVal, IDKey, PIDKey, Translate(row), pId => Translate(Set.Rows.First(o => o[IDKey].AsString() == pId)), null);
            }

            return retVal;
        }

        public static FlexiTreeJson LoadFlexiWBSGrid<T>(this T Entity, List<StringDict> Source, int Totle, string IDKey, string WBSKey)
            where T : RuleBase
        {
            if (Totle == 0)
            {
                Totle = Source.Count();
            }


            var retVal = new FlexiTreeJson(Entity.GetName(), Totle);
            if (retVal.PostColumns.Length == 0) return retVal;

            if (Totle == 0)
            {
                return ReturnNoSession(retVal);
            }

            foreach (var row in Source)
            {
                AddWBSRow(retVal, IDKey, WBSKey, row, (pwbs, wbsKey) => Source.First(o => o[WBSKey] == pwbs));
            }

            return retVal;
        }


        /// <summary>
        /// 异步加载树控件数据.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="Entity"></param>
        /// <param name="Source">该级数据源.</param>
        /// <param name="Totle"></param>
        /// <param name="TreeSource">该级数据源加载时,需要额外展示的树节点段,其中该树的根节点须在该级数据源上出现. </param>
        /// <param name="HasChildFunc"></param>
        /// <returns></returns>
        public static FlexiTreeJson LoadFlexiAjaxGrid<T, S>(this T Entity, IEnumerable<S> Source, int Totle, Func<S, bool> HasChildFunc)
            where T : RuleBase
        {
            if (Totle == 0)
            {
                Totle = Source.Count();
            }

            FlexiTreeJson fr = new FlexiTreeJson(Entity.GetName(), Totle);

            if (Totle == 0)
            {
                return ReturnNoSession(fr);
            }

            string[] Columns = System.Mvc.Model["FlexiGrid_Cols"].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            string idCol = System.Mvc.Model["FlexiGrid_Id"];


            bool IsDict = false;
            Type type = typeof(S);
            if (type.GetInterface("System.Collections.IDictionary") != null)
            {
                IsDict = true;
            }

            IEnumerable<string> ps = null;
            Dictionary<string, MethodInfo> MethodInfoGetDict = null;

            foreach (S item in Source)
            {
                var row = fr.AddNewRootRow();


                if (IsDict)
                {
                    var dict = item as IDictionary;
                    row.Id = dict[idCol].AsString();
                    row.AjaxHasChildren = HasChildFunc(item);
                    Columns.All(o =>
                    {
                        if (dict.Contains(o))
                        {
                            row.Cell[o] = dict[o].AsString();
                        }
                        else row.Cell[o] = string.Empty;
                        return true;
                    });
                }
                else
                {
                    row.AjaxHasChildren = HasChildFunc(item);


                    var entity = item as IEntity;
                    if (entity != null)
                    {
                        Columns.All(o =>
                        {
                            if (entity.GetProperties().Contains(o))
                            {
                                var propertyValue = entity.GetPropertyValue(o).AsString();
                                if (o == idCol)
                                {
                                    row.Id = propertyValue;
                                }

                                row.Cell[o] = propertyValue;
                            }
                            else row.Cell[o] = string.Empty;
                            return true;
                        });
                    }
                    else
                    {
                        if (ps == null)
                        {
                            ps = TypeDescriptor.GetProperties(item).ToMyList(o => (o as PropertyDescriptor).Name);
                        }

                        if (MethodInfoGetDict == null)
                        {
                            MethodInfoGetDict = new Dictionary<string, MethodInfo>();
                            Columns.All(o =>
                            {
                                if (ps.Contains(o) == false) return true;

                                MethodInfoGetDict[o] = type.GetMethod("get_" + o);

                                return true;
                            });
                        }


                        Columns.All(o =>
                        {
                            if (ps.Contains(o))
                            {
                                var propertyValue = FastInvoke.GetPropertyValue(item, type, MethodInfoGetDict[o]).AsString();
                                if (o == idCol)
                                {
                                    row.Id = propertyValue;
                                }

                                row.Cell[o] = propertyValue;
                            }
                            else row.Cell[o] = string.Empty;
                            return true;
                        });
                    }

                }

                //fr.Rows.Add(row);
            }


            //if (TreeSource != null)
            //{
            //    TreeSource.JsonTree.All(o =>
            //        {
            //            var pRow = fr.JsonTree.FirstOrDefault(p => p.Id == o.Id);
            //            if (pRow != null)
            //            {
            //                pRow.Rows = o.Rows;
            //            }

            //            return true;
            //        });
            //}

            return fr;
        }



        /// <summary>
        /// 异步加载树型列表, 当数据源里有树型结构时, 加载该树型结构.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="Entity">如果是 List&lt;XmlDictionary&gt;&lt;/XmlDictionary&gt; 则一定是 string , string </param>
        /// <param name="Source"></param>
        /// <param name="Totle"></param>
        /// <param name="IdKey"></param>
        /// <param name="PIdKey"></param>
        /// <param name="HasChildFunc"></param>
        /// <returns></returns>
        public static FlexiTreeJson LoadFlexiAjaxGrid<T, S>(this T Entity, IEnumerable<S> Source, int Totle, string IdKey, string PIdKey, Func<XmlDictionary<string, string>, bool> HasChildFunc)
            where T : RuleBase
            where S : IReadEntity
        {
            if (Totle == 0)
            {
                Totle = Source.Count();
            }

            var retVal = new FlexiTreeJson(Entity.GetName(), Totle);
            if (retVal.PostColumns.Length == 0) return retVal;


            if (Totle == 0)
            {
                return ReturnNoSession(retVal);
            }

            Type type = typeof(S);
            var isDict = type.GetInterface("System.Collections.IDictionary") != null;
            MethodInfo MethodInfoGet = null;

            foreach (var row in Source)
            {
                AddTreeRow(retVal, IdKey, PIdKey, dbo.ModelToStringDict(row), pId =>
                {
                    if (isDict)
                    {
                        return dbo.ModelToStringDict(
                            Source.First(o => (o as IDictionary)[IdKey].AsString() == pId));
                    }
                    else
                    {

                        if (type.GetInterface("MyCmn.IEntity") != null)
                        {
                            return dbo.ModelToStringDict(
                                Source.FirstOrDefault(o => (o as IEntity).GetPropertyValue(IdKey).AsString() == pId)
                                );
                        }
                        else
                        {
                            if (MethodInfoGet == null)
                            {
                                MethodInfoGet = type.GetMethod("get_" + IdKey);
                            }

                            return dbo.ModelToStringDict(
                                Source.First(o => FastInvoke.GetPropertyValue(o, type, MethodInfoGet).AsString() == pId))
                                ;
                        }
                    }
                }, HasChildFunc);
            }

            return retVal;
        }



        #region private
        private static FlexiTreeJson.FlexiTreeRowData FindPNode(string pid, string IDKey, List<FlexiTreeJson.FlexiTreeRowData> Nodes)
        {
            FlexiTreeJson.FlexiTreeRowData retOne = null;
            Nodes.All(o =>
            {
                if (o.Cell[IDKey] == pid)
                {
                    retOne = o;
                    return false;
                }
                if (o.Rows != null && o.Rows.Count > 0)
                {
                    retOne = FindPNode(pid, IDKey, o.Rows);
                    if (retOne != null) return false;
                }
                return true;
            });

            return retOne;
        }

        private static void AddTreeRow(FlexiTreeJson retVal, string IDKey, string PIDKey, StringDict rowData, Func<string, StringDict> FindPNodeFunc, Func<StringDict, bool> HasChildFunc)
        {
            if (rowData == null) return;
            GodError.Check(rowData.ContainsKey(PIDKey) == false, "在实体上找不到父项键值:" + PIDKey);

            if (rowData[PIDKey].HasValue() == false || rowData[PIDKey].AsString() == "0" || rowData[PIDKey].AsString() == Guid.Empty.ToString())
            {
                if (retVal.Rows.Count(o => o.Cell[IDKey] == rowData[IDKey]) > 0) return;
                var newRow = retVal.AddNewRootRow();
                newRow.Id = rowData[IDKey].AsString();
                newRow.Cell = rowData;

                if (HasChildFunc != null)
                {
                    newRow.AjaxHasChildren = HasChildFunc(rowData);
                }

                //retVal.Rows.Add(newRow);
                return;
            }

            var pNode = FindPNode(rowData[PIDKey], IDKey, retVal.Rows);
            if (pNode == null)
            {
                AddTreeRow(retVal, IDKey, PIDKey, FindPNodeFunc(rowData[PIDKey]), FindPNodeFunc, HasChildFunc);
                pNode = FindPNode(rowData[PIDKey], IDKey, retVal.Rows);
            }

            {
                if (pNode.Rows.Count(o => o.Cell[IDKey] == rowData[IDKey]) > 0) return;

                var newRow = pNode.AddNewRow();
                newRow.Id = rowData[IDKey];
                newRow.Cell = rowData;

                if (HasChildFunc != null)
                {
                    newRow.AjaxHasChildren = HasChildFunc(rowData);
                }

                //pNode.Rows.Add(newRow);
            }
        }

        private static void AddWBSRow(FlexiTreeJson retVal, string IDKey, string WBSKey, StringDict rowData, Func<string, string, StringDict> FindFromWbsFunc)
        {
            if (rowData == null) return;

            if (rowData[WBSKey].HasValue() == false || rowData[WBSKey].Count(o => o == '.' || o == ',') == 0)
            {
                if (retVal.Rows.Count(o => o.Cell[IDKey] == rowData[IDKey]) > 0) return;
                var newRow = retVal.AddNewRootRow();
                newRow.Id = rowData[IDKey];
                newRow.Cell = rowData;
                //retVal.Rows.Add(newRow);
                return;
            }
            var wbss = rowData[WBSKey].Split(".,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var pwbAry = string.Join(".", wbss.Slice(0, wbss.Length - 1).ToArray());
            var pnodeData = FindFromWbsFunc(pwbAry, WBSKey);
            var pNodeInJson = FindPNode(pnodeData[IDKey], IDKey, retVal.Rows);
            if (pNodeInJson == null)
            {
                AddWBSRow(retVal, IDKey, WBSKey, pnodeData, FindFromWbsFunc);
                pNodeInJson = FindPNode(pnodeData[IDKey], IDKey, retVal.Rows);
            }

            {
                if (pNodeInJson.Rows.Count(o => o.Cell[IDKey] == rowData[IDKey]) > 0) return;
                var newRow = pNodeInJson.AddNewRow();
                newRow.Id = rowData[IDKey];
                newRow.Cell = rowData;
                //pNodeInJson.Rows.Add(newRow);
            }
        }
        #endregion
    }
}
