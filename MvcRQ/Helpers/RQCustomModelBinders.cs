using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;

namespace MvcRQ.Helpers
{
    public class RQCustomModelBinders : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        //    var originalMetadata = bindingContext.ModelMetadata;

        //    bindingContext.ModelMetadata = new ModelMetadata(ModelMetadataProviders.Current,
        //                                                     originalMetadata.ContainerType,
        //                                                     () => null,  //Forces model to null  
        //                                                     originalMetadata.ModelType,
        //                                                     originalMetadata.PropertyName);
            try
            {
                return base.BindModel(controllerContext, bindingContext);
            }
            catch (Exception)
            {
                return null;
            }
        }

        //protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
        //    PropertyDescriptor propertyDescriptor)
        //{
        //    base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        //}

        /// <summary> 
        /// Fix for the default model binder's failure to decode enum types when binding to JSON. 
        /// </summary> 
        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            var propertyType = propertyDescriptor.PropertyType;
            if (propertyType.IsEnum)
            {
                var providerValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (null != providerValue)
                {
                    var value = providerValue.RawValue;
                    if (null != value)
                    {
                        var valueType = value.GetType();
                        if (!valueType.IsEnum)
                        {
                            if (valueType == typeof(String))
                            {
                                switch (((String) value).ToLower())
                                {
                                    case "ddc": 
                                        value = 0;
                                        break;
                                    case "rvk":
                                        value = 1;
                                        break;
                                    case "jel":
                                        value = 2;
                                        break;
                                    case "rq":
                                        value = 3;
                                        break;
                                    case "oldrq":
                                        value = 4;
                                        break;
                                    default:
                                        value = 5;
                                        break;
                                }
                            }
                            return Enum.ToObject(propertyType, value);
                        }
                    }
                }
            }
            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }

        //protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
        //    PropertyDescriptor propertyDescriptor, Object value)
        //{
        //    if (propertyDescriptor.Name == "Notes")
        //    {
        //        string test = "TEST";
        //    }
        //    base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
        //}
    }
}
