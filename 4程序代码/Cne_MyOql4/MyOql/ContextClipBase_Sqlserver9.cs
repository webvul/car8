using System;
using System.Linq;
using System.Data.Common;

namespace MyOql
{

    public abstract partial class ContextClipBase
    {
        /// <summary>
        /// 过滤用的查询.只有出现唯一约束列时，才集成分页查询。
        /// </summary>
        /// <param name="ToEntityFunc">Map函数,Reader,是否采用集成分页（如果为true，则生成分页SQL，如果为false，则去除分页机制，按全部返回。）,
        /// 当采用集成分页时总是InTake，否则显示当前Reader位置状态,当前Reader索引号, 返回true,继续读取, 返回false停止.</param>
        /// <returns>是否是部分结果</returns>
        [Obsolete("只用在 SqlServer2000 及以下版本")]
        public bool ExecuteReader(Func<DbDataReader, bool, ReaderPositionEnum, int, bool> ToEntityFunc)
        {
            Action<Action<bool, int, int>> UseCommand = (act) =>
            {
                var Skip_Clip = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Skip) as SkipClip;
                int Skip = 0;
                if (Skip_Clip != null) Skip = Skip_Clip.SkipNumber;

                var Take_Clip = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.Take) as TakeClip;
                int Take = 0;
                if (Take_Clip != null)
                {
                    Take = Take_Clip.TakeNumber;
                }

                if (Skip == 0 && Take == 0)
                {
                    act(true, Skip, Take);
                    return;
                }

                //以后都是单表分页操作.

                //var hasJoin = this.Dna.Count(o => o.Key.IsIn(SqlKeyword.Join, SqlKeyword.LeftJoin, SqlKeyword.RightJoin)) > 0;
                //if (hasJoin)
                //{
                //    act(false, Skip, Take);
                //    return;
                //}

                if (/*hasJoin == false && */this.ContainsIdupKey())
                {
                    act(true, Skip, Take);
                    return;
                }

                //if (TableHasPolymer())
                //{
                //    act(false, Skip, Take);
                //    return;
                //}

                act(false, Skip, Take);
            };

            var usePager = true;
            var skip = 0;
            var take = 0;

            UseCommand((a, b, c) => { usePager = a; skip = b; take = c; });





            if (usePager)
            {
                MyCommand myCmd = this.ToCommand();

                int curPos = -1;
                ExecuteReader(myCmd, o =>
                {
                    return ToEntityFunc(o, usePager, ReaderPositionEnum.InTake, ++curPos);
                });
                AffectRow = 0 - curPos;
            }
            else
            {
                int curPos = -1;

                Func<ReaderPositionEnum> skipFunc = () =>
                {
                    ++curPos;
                    if (skip == 0 && take == 0)
                    {
                        return ReaderPositionEnum.InTake;
                    }

                    else if (curPos < skip)
                    {
                        return ReaderPositionEnum.BeforeSkip;
                    }
                    else if (curPos >= skip + take)
                    {
                        return ReaderPositionEnum.AfterTake;
                    }
                    else return ReaderPositionEnum.InTake;
                };

                this.Dna.RemoveAll(o => o.Key == SqlKeyword.Skip);
                this.Dna.RemoveAll(o => o.Key == SqlKeyword.Take);

                MyCommand myCmd = this.ToCommand();

                ExecuteReader(myCmd, o =>
                {
                    return ToEntityFunc(o, usePager, skipFunc(), curPos);
                });

                AffectRow = curPos;
            }

            return usePager;
        }
    }
}
