using System.Data;

namespace MyOql
{
    public static partial class dbo
    {
        /// <summary>
        /// 根据自定义Sql返回结果集.
        /// </summary>
        /// <param name="ConfigName">配置文件的数据库连接名.</param>
        /// <param name="Sql"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(string ConfigName, string Sql)
        {
            var provider = dbo.GetDbProvider(ConfigName);
            var cmd = provider.GetConnection(ConfigName).CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = Sql;

            var sqlAdapter = provider.GetDataAdapter();
            sqlAdapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds);
            if (ds == null) return null;
            return ds.Tables[0];
        }

        /// <summary>
        /// 根据自定义Sql返回结果集.
        /// </summary>
        /// <param name="ConfigName">配置文件的数据库连接名.如果为空,取默认值 dbo</param>
        /// <param name="Sql"></param>
        /// <returns></returns>
        public static object ToScalar(string ConfigName, string Sql)
        {
            var provider = dbo.GetDbProvider(ConfigName);
            var cmd = provider.GetConnection(ConfigName).CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = Sql;


            return Open(cmd.Connection, () =>
            {
                return cmd.ExecuteScalar();
            });
        }


        public static int ExecuteSql(string ConfigName, string Sql)
        {
            var provider = dbo.GetDbProvider(ConfigName);
            var cmd = provider.GetConnection(ConfigName).CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = Sql;


            return Open(cmd.Connection, () =>
            {
                return cmd.ExecuteNonQuery();
            });
        }
    }
}
