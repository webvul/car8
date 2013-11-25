using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DbEnt;
using MyOql;

namespace MyWeb.Areas.Admin.Models
{
    public class Menus
    {
        public List<MenuRule.Entity> menu = new List<MenuRule.Entity>();
        public PersonRule.Entity person = new PersonRule.Entity();
    }
}