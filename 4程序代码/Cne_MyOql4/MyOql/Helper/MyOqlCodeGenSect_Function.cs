using System.Configuration;

namespace MyOql
{
    public partial class MyOqlCodeGenSect : ConfigurationSection
    {
        /// <summary>
        /// 函数定义是在表定久的基础上添加了参数列表。要求在定义出完整的列定义Enums
        /// </summary>
        public class FunctionCollection : TableCollection
        {
            protected override ConfigurationElement CreateNewElement()
            {
                return new FunctionGroupCollection(this);
            }

            public class FunctionGroupCollection : TableGroupCollection
            {
                protected override ConfigurationElement CreateNewElement()
                {
                    return new FunctionElement(this);
                }

                protected override object GetElementKey(ConfigurationElement element)
                {
                    var siteUser = element as FunctionElement;
                    return siteUser.Name;
                }

                public FunctionGroupCollection(FunctionCollection myOqlCodeGenViewsCollection)
                    : base(myOqlCodeGenViewsCollection)
                {
                }

                public class FunctionElement : TableElement
                {
                    public FunctionElement(TableGroupCollection myOqlCodeGenViewGroupCollection)
                        : base(myOqlCodeGenViewGroupCollection)
                    {
                    }
                }
            }
        }
    }
}
