using System;
using System.Collections.Generic;
using System.Data.Common;
using MyCmn;

namespace MyOql.Provider
{
    //public interface IDbProvider
    //{
    //    DbCommand ToCommand(ContextClipBase Context);
    //    DbConnection GetConnection(string ConfigName);
    //    DbDataAdapter GetDataAdapter();
    //    //string DecryptConnectionString(Func<string, string> Func);
    //}

    public class CommandValue
    {
        public CommandValue()
        {
            this.Sql = string.Empty;
            this.Parameters = new List<DbParameter>();
        }
        public string Sql { get; set; }
        public List<DbParameter> Parameters { get; set; }

        public override string ToString()
        {
            return Sql;
        }

        public class CompareDbParameter : IEqualityComparer<DbParameter>
        {
            #region IEqualityComparer<DbParameter> Members

            public bool Equals(DbParameter x, DbParameter y)
            {
                return x.ParameterName == y.ParameterName;
            }

            public int GetHashCode(DbParameter obj)
            {
                return obj.ParameterName.GetHashCode();
            }

            #endregion
        }

        public static CompareDbParameter CompareParameter = new CompareDbParameter();

        public static CommandValue operator &(CommandValue left, CommandValue right)
        {
            left.Sql += right.Sql;
            left.Parameters.AddRange(right.Parameters);
            //left.Parameters = left.Parameters.Distinct(CompareParameter).ToList();
            return left;
        }

        public string ToFullSql()
        {
            string fullSql = this.Sql;
            foreach (var item in this.Parameters)
            {
                var val = item.Value.AsString();
                if (item.DbType.DbTypeIsNumber() == false || item.DbType == System.Data.DbType.Boolean )
                {
                    val = "'" + val.Replace("'", "''") + "'";
                }

                var index = fullSql.IndexOf(item.ParameterName, 0, StringComparison.Ordinal,
                    c =>
                    {
                        if (c == ' ') return true;
                        if (char.IsLetterOrDigit(c)) return false;
                        if (c == '_') return false;
                        return false;
                    },
                    c =>
                    {
                        if (c == ' ') return true;
                        if (char.IsLetterOrDigit(c)) return false;
                        if (c == '_') return false;
                        return false;
                    });

                GodError.Check(index < 0,   "在" + fullSql + "中找不到:" + item.ParameterName);


                fullSql = fullSql.Remove(index, item.ParameterName.Length).Insert(index, val);
            }

            return fullSql;
        }
    }
}
