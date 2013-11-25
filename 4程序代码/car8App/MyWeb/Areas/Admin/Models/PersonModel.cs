using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyOql;
using DbEnt;
using MyCmn;

namespace MyWeb.Areas.Admin.Models
{
    /// <summary>
    /// 自定义人员模型
    /// </summary>
    public class PersonModel  
    {
        public PersonRule.Entity Person { get; set; }
        /// <summary>
        /// 部门,用于上传接收值.
        /// </summary>
        public string Dept { get; set; }

        /// <summary>
        /// 角色,用于上传接收值.
        /// </summary>
        public string Role { get; set; }

        public PersonModel()
        {
            Person = new PersonRule.Entity();
        }

        public PersonModel(PersonRule.Entity Entity)
        {
            Person = Entity.Clone() as PersonRule.Entity;
        }
    }
}