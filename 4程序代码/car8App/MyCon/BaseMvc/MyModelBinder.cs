using System.Web.Mvc;
using System.IO;
using MyOql;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;
using MyBiz;
using System.Web;
using MyCon;

namespace MyCmn
{
    public class MyModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType.IsEnum)
            {
                ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (valueProviderResult != null)
                {
                    return EnumHelper.ToEnum(valueProviderResult.AttemptedValue, bindingContext.ModelType, 0);
                }
            }

            if (bindingContext.ModelType.IsValueType && bindingContext.ModelType.FullName == "System.Boolean")
            {
                ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "[0]");
                if (valueProviderResult != null/* && valueProviderResult.AttemptedValue == "on"*/)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return base.BindModel(controllerContext, bindingContext);
            }
        }
    }
}