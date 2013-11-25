using System.Configuration;
using MyCmn;

namespace MyOql
{
    /// <summary>
    /// 示例见实际项目.
    /// </summary>
    /// <remarks>
    /// 一.定义规范:
    ///     a) 名称为了可读性。请参见 TranslateDbName <see cref="dbo.TranslateDbName"/>
    ///     b) 在实际数据业务中,可以定义多个业务模块,再对这些模块定义功能.
    ///         可以根据实际情况,把模块定义为 Group , 把各个表定义到各个Group.
    ///         对于系统组, 定义其 Name 为空
    /// 二.定义基本结构
    /// 实例参见： MyTool\MyOqlCodeGen.config
    /// 二.定义数据库 
    ///1.定义数据库表或视图(视图的操作和表相同)，规范如下：
    ///     a)	必须定义主键，其它键可无:  外键 ，计算列 ,自增键,唯一键,列定义.
    ///     b)	列名不能与表名相同。（该规则是因为：类的属性名不能与类名相同）
    ///     数据库表的配置 Entity节点属性如下:
    ///     Name是表的名称
    ///     PKs 是主键列表，多个主键，用“，” 分隔
    ///     Fks 是外键列表，格式： 外键名称＝引用表：引用表键 ，多个外键用“，”分隔. 格式参见:MyFkNode <see cref="MyOql.MyOql_CodeGen.MyFkNode"/>
    ///     AutoIncreKey 是自增列，最多有一个
    ///     UniqueKey 唯一约束，虽然数据库可以有多个， 但对于业务系统来说 ， 只写一个即可。
    ///     ComputeKeys是计划列，多个用“，”分隔
    ///     Enums是枚举列类型. 如指定某列为枚举. MyOql 会根据数据库存的是 Int / Varchar2 , 动态转换枚举值. 也可以指定某列的类型.如指定时间格式的C#类型是 MyDate (MyOql 默认用MyDate类型映射到数据库的 时间格式.).
    ///2.定义存储过程 规范如下:
    ///     Name 是存储过程的名称
    ///     Paras 是存储过程的参数列表，多个参数用“，”分隔，单个参数 格式参见: MyParaNode <see cref="MyOql.MyOql_CodeGen.MyParaNode"/>
    ///     Return 是指存储过程返回的数据类型默认out参数，返回格式参见: MyParaReturnDefine <see cref="MyOql.MyOql_CodeGen.MyParaReturnDefine"/>
    ///         但要特别注意的是：如果果 Return 返回定义为空，则调用 ProcClip.Execute，即返回存储过程的返回值（存储过程中return Value 形式,如果没有定义return ，则返回影响行数）
    ///         而如果 Return 显式定义了返回数字类型，则调用 ProcClip.ExecuteReader ，即返回存储过程结果集的第一行第一列的值（select * from table）
    ///3.定义返回表值型函数 , 类似于定义带参数的表 . 其规范与表相同. 其特殊性在于,必须显式的定义全部列.
    /// </remarks>
    public partial class MyOqlCodeGenSect : ConfigurationSection
    {
        //[ConfigurationProperty("IgnoreTables", IsRequired = false)]
        //public string IgnoreTables
        //{
        //    get { return this["IgnoreTables"] as string; }
        //    set { this["IgnoreTables"] = value; }
        //}

        [ConfigurationProperty("Table")]
        public TableCollection Table
        {
            get { return this["Table"] as TableCollection; }
        }

        [ConfigurationProperty("View")]
        public ViewCollection View
        {
            get { return this["View"] as ViewCollection; }
        }

        [ConfigurationProperty("Proc")]
        public ProcCollection Proc
        {
            get { return this["Proc"] as ProcCollection; }
        }

        [ConfigurationProperty("Function")]
        public FunctionCollection Function
        {
            get { return this["Function"] as FunctionCollection; }
        }


        public IConfigGroupSect GetGroup(string Entity)
        {
            foreach (TableCollection.TableGroupCollection group in this.Table)
            {
                foreach (TableCollection.TableGroupCollection.TableElement item in group)
                {
                    if (item.Name == Entity || item.MapName == Entity) return group;
                }
            }

            foreach (ViewCollection.ViewGroupCollection group in this.View)
            {
                foreach (ViewCollection.ViewGroupCollection.ViewElement item in group)
                {
                    if (item.Name == Entity || item.MapName == Entity) return group;
                }
            }   
            
            foreach (ProcCollection.ProcGroupCollection group in this.Proc)
            {
                foreach (ProcCollection.ProcGroupCollection.ProcElement item in group)
                {
                    if (item.Name == Entity || item.MapName == Entity) return group;
                }
            }

            foreach (FunctionCollection.TableGroupCollection group in this.Function)
            {
                foreach (FunctionCollection.TableGroupCollection.TableElement item in group)
                {
                    if (item.Name == Entity || item.MapName == Entity) return group;
                }
            }

            throw new GodError("找不到：" + Entity + " 所属的组");
        }
        public IConfigSect GetEntity(string Entity)
        {
            foreach (TableCollection.TableGroupCollection group in this.Table)
            {
                foreach (TableCollection.TableGroupCollection.TableElement item in group)
                {
                    if (item.Name == Entity || item.MapName == Entity) return item;
                }
            }

            foreach (ViewCollection.ViewGroupCollection group in this.View)
            {
                foreach (ViewCollection.ViewGroupCollection.ViewElement item in group)
                {
                    if (item.Name == Entity || item.MapName == Entity) return item;
                }
            }   
            
            foreach (ProcCollection.ProcGroupCollection group in this.Proc)
            {
                foreach (ProcCollection.ProcGroupCollection.ProcElement item in group)
                {
                    if (item.Name == Entity || item.MapName == Entity) return item;
                }
            }

            foreach (FunctionCollection.TableGroupCollection group in this.Function)
            {
                foreach (FunctionCollection.TableGroupCollection.TableElement item in group)
                {
                    if (item.Name == Entity || item.MapName == Entity) return item;
                }
            }

            throw new GodError("找不到：" + Entity + " 实体");
        }
    }
}
