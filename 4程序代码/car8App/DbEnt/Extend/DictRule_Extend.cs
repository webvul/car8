//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web;
//using MyCmn;
//
//namespace MyOql
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public partial class DictRule
//    {
//        /// <summary>
//        /// KeyPath 是 Key 的路径. 节点之间以\分隔.如果单个键名包含\,用\\替换.
//        /// </summary>
//        /// <param name="KeyPath"></param>
//        /// <returns></returns>
//        public static string GetValue(string KeyPath)
//        {
//            KeyPath = KeyPath.Replace(@"\\", ValueProc.SplitSect.ToString());
//            var keys = KeyPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)
//                .Select(o => o.Replace(ValueProc.SplitSect, '\\'))
//                .ToArray();
//
//            //很可能会有重复数据. 
//            var set = dbr.Dict
//                .SelectWhere(o => o.Key.In(keys))
//                .OrderBy(o => o.RootPath.Asc)
//                .ToEntityList(o => o._);
//
//
//            DictRule.Entity curEnt = new Entity() { ID = 0 };
//            for (int i = 0; i < keys.Length; i++)
//            {
//                curEnt = set.FirstOrDefault(o => o.PID == curEnt.ID & o.Key == keys[i]);
//                if (curEnt == null) return keys.LastOrDefault();
//            }
//            return curEnt.Value;
//        }
//
//        public static void SetValue(string KeyPath, string Value, DictTraitEnum Trait)
//        {
//            KeyPath = KeyPath.Replace(@"\\", ValueProc.SplitSect.ToString());
//            var keys = KeyPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)
//                .Select(o => o.Replace(ValueProc.SplitSect, '\\'))
//                .ToArray();
//
//            //很可能会有重复数据. 
//            var set = dbr.Dict
//                .SelectWhere(o => o.Key.In(keys))
//                .OrderBy(o => o.RootPath.Asc)
//                .ToEntityList(o => o._);
//
//            DictRule.Entity curEnt = null;
//            DictRule.Entity preEnt = new Entity() { ID = 0 };
//
//            Action<string[], DictRule.Entity> Insert = (InsertKeys, root) =>
//            {
//                for (int j = 0; j < InsertKeys.Length; j++)
//                {
//                    root = new Entity()
//                    {
//                        PID = root.ID,
//                        Key = InsertKeys[j],
//                        RootPath = root.RootPath + "," + root.ID.AsString()
//                    };
//
//                    if (j == InsertKeys.Length - 1)
//                    {
//                        root.Value = Value;
//                        root.Trait = Trait.AsString();
//                    }
//                    var insert = dbr.Dict.Insert(root);
//
//                    insert.Execute();
//                    root.ID = insert.LastAutoID;
//                }
//
//            };
//            for (int i = 0; i < keys.Length; i++)
//            {
//                curEnt = set.FirstOrDefault(o => o.PID == preEnt.ID & o.Key == keys[i]);
//                if (curEnt == null)
//                {
//                    Insert(keys.GetSub(i, keys.Length - i - 1).ToArray(), preEnt);
//                    return;
//                }
//
//                preEnt = curEnt;
//            }
//
//            if (curEnt.Value != Value || curEnt.Trait != Trait.AsString())
//            {
//                curEnt.Value = Value;
//                curEnt.Trait = Trait.AsString();
//
//                dbr.Dict.Update(curEnt).Execute();
//            }
//
//
//            //object[] row = null;
//            //int idIndex = set.Columns.IndexOf(dbr.Dict.ID);
//            //int pidIndex = set.Columns.IndexOf(dbr.Dict.PID);
//            //int keyIndex = set.Columns.IndexOf(dbr.Dict.Key);
//            //int valIndex = set.Columns.IndexOf(dbr.Dict.Value);
//            //Func<int, string, object[]> GetChild = (ParentId, Key) =>
//            //{
//            //    return set.Rows.FirstOrDefault(o => o[pidIndex].GetInt() == ParentId && o[keyIndex].AsString() == Key);
//            //};
//
//            //for (int i = 0; i < keys.Length; i++)
//            //{
//            //    var key = keys[i];
//            //    var preId = row[idIndex].GetInt() ;
//            //    var parentRootPath = row[
//            //    row = GetChild(row == null ? 0 : preId, key);
//            //    if (row == null) {
//
//            //        for (int j = i; j < keys.Length - 1 ; j++)
//            //        {
//            //            var insert = dbr.Dict.Insert(o => o.PID == preId & o.Key == keys[j] & 
//            //                o.RootPath == );
//            //        }
//            //        break;
//            //    }
//
//            //}
//
//            //return row[valIndex].AsString();
//        }
//
//        public static string GetRes<T>(T ResKey) where T : IComparable, IFormattable, IConvertible
//        {
//            return GetValue(@"Enum\Zh\" + ResKey.GetType().FullName + "." + ResKey.AsString());
//        }
//
//        public static string GetRes(string ResKey)
//        {
//            return GetValue(@"Enum\Zh\" + ResKey);
//        }
//
//        public static void SetRes<T>(T ResKey, string Value, DictTraitEnum Trait) where T : IComparable, IFormattable, IConvertible
//        {
//            SetValue(@"Enum\Zh\" + ResKey.GetType().FullName + "." + ResKey.AsString(), Value, Trait);
//        }
//    }
//}
