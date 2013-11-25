using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using MyCmn;
using System.Data;

namespace MyOql
{
    /// <summary>
    /// 表示一个存储过程.基类.
    /// </summary>
    public class ProcRule : RuleBase, IProcRule
    {
        public ProcRule(string Name)
            : base(Name)
        {
        }
        public override string GetDbName()
        {
            return GetName();
        }

        public override SimpleColumn[] GetColumns()
        {
            return null;
        }

        public override SimpleColumn[] GetPrimaryKeys()
        {
            return null;
        }

        public override SimpleColumn[] GetComputeKeys()
        {
            return null;
        }

        public override SimpleColumn GetAutoIncreKey()
        {
            return null;
        }

        public override SimpleColumn GetUniqueKey()
        {
            return null;
        }

        public override object Clone()
        {
            var tab = new ProcRule(this.Name);
            tab._Config_ = this._Config_;
            //this._Extend.All(o =>
            //{
            //    tab._Extend[o.Key] = o.Value;
            //    return true;
            //});
            return tab;
        }
    }

    //public class ProcNameClip : SqlClipBase
    //{
    //    public ProcNameClip()
    //    {
    //        this.Key = SqlKeyword.ProcName;
    //    }
    //    public string Name { get; set; }
    //}

    public class ProcParameterClip : SqlClipBase
    {
        public ProcParameterClip()
        {
            this.Key = SqlKeyword.ProcParameter;
            this.Parameters = new List<DbParameter>();
        }
        public List<DbParameter> Parameters { get; set; }

        public override object Clone()
        {
            return this.MustDeepClone();
        }
    }


    //public class ProcVarClip   : ContextClipBase
    //{
    //    public string ColumnName { get; set; }

    //    public ProcVarClip(ProcClip Proc)
    //    {
    //        this.Key = SqlKeyword.ProcVar;

    //        this.AffectRow = dbr.AffectRow;
    //        this.Connection = dbr.Connection;
    //        this.CurrentRule = dbr.CurrentRule;
    //        this.Dna = dbr.Dna;
    //        this.Transaction = dbr.Transaction;
    //    }
    //}

    /// <summary>
    /// 存储过程。
    /// </summary>
    public class ProcClip : ContextClipBase
    {
        //private MyCommand MyCmd { get; set; }
        public ProcClip(string ProcName)
        {
            this.Key = SqlKeyword.Proc;

            this.CurrentRule = new ProcRule(ProcName);

            //this.Dna.Add(new ProcNameClip() { Name = ProcName });
        }

        public void AddParameter(DbParameter Parameter)
        {
            ProcParameterClip paras = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.ProcParameter) as ProcParameterClip;
            bool isHave = false;
            if (paras == null)
            {
                paras = new ProcParameterClip();
                isHave = false;
            }
            else isHave = true;

            paras.Parameters.Add(Parameter);

            if (isHave == false)
            {
                this.Dna.Add(paras);
            }
        }

        public XmlDictionary<string, object> GetParaData()
        {
            ProcParameterClip paras = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.ProcParameter) as ProcParameterClip;
            XmlDictionary<string, object> Data = new XmlDictionary<string, object>();

            if (paras == null) return Data;

            foreach (var item in paras.Parameters)
            {
                if (item.Direction == System.Data.ParameterDirection.Output ||
                    item.Direction == System.Data.ParameterDirection.ReturnValue)
                {
                    continue;
                }
                Data[item.ParameterName] = item.Value;
            }
            return Data;
        }

        /// <summary>
        /// 执行,返回存储过程的返回值 , 如果不存在 ReturnValue 参数 ,则返回影响行数.
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            if (dbo.Event.OnProcing(this) == false) return 0;

            var myCmd = ToCommand();

            var retVal = Execute(myCmd);

            return retVal;
        }

        public override int Execute(MyCommand myCmd)
        {
            var retVal = ExecuteBase(myCmd);
            ProcParameterClip paras = this.Dna.FirstOrDefault(o => o.Key == SqlKeyword.ProcParameter) as ProcParameterClip;
            if (paras != null)
            {
                var retrunParam = paras.Parameters.FirstOrDefault(o => o.Direction == ParameterDirection.ReturnValue);
                if (retrunParam != null)
                {
                    retVal = retrunParam.Value.AsInt(-1);
                }
            }

            dbo.Event.OnProced(this);

            return retVal;
        }


        /// <summary>
        /// 执行并返回记录
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public override bool ExecuteReader(Func<DbDataReader, bool> func)
        {
            if (
                dbo.Event.OnProcing(this) == false) return false;

            var retVal = this.ExecuteReader(ToCommand(), func);


            return retVal;
        }

        public override bool ExecuteReader(MyCommand myCmd, params Func<DbDataReader, bool>[] func)
        {
            var retVal = ExecuteReaderBase(myCmd, func);
            dbo.Event.OnProced(this);
            return retVal;
        }


        public virtual DataSet ToDataSet()
        {
            DataSet ds = new DataSet();
            if (dbo.Event.OnProcing(this) == false) return null;

            var myCmd = this.ToCommand();

            var cmd = myCmd.Command;

            if (dbo.Event.OnExecute(this) == false) return null;


            return dbo.Open(myCmd, this, () =>
            {
                try
                {
                    var da = GetDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(ds);

                    dbo.Event.OnProced(this);
                    //cmd.Parameters.Clear();
                    return ds;
                }
                catch (Exception ee)
                {
                    ee.ThrowError(myCmd);
                    throw;
                }
            });
        }

        /// <summary>
        /// 推荐使用 ToDictList 方法。
        /// </summary>
        /// <returns></returns>
        public virtual DataTable ToDataTable()
        {
            DataSet ds = ToDataSet();
            if (ds == null || ds.Tables.Count == 0) return null;
            return ds.Tables[0];
        }


        //public ProcVarClip AsVarArray(string ColumnName)
        //{
        //    ProcVarClip proc = new ProcVarClip(this);

        //    proc.ColumnName = ColumnName;


        //    return null;
        //}

        /// <summary>
        /// 当存储过程做为查询结果放到表变量里时，表示结果的定义。不可以为 ConstRule ,忽略out参数。
        /// </summary>
        public RuleBase Result { get; set; }
    }


    ///// <summary>
    ///// 带有返回数据集的存储过程。
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public class ProcClip<T> : ProcClip
    //    where T : RuleBase, new()
    //{
    //    public ProcClip(string ProcName)
    //        : base(ProcName)
    //    {
    //        this.Result = new T();
    //    }
    //}
}
