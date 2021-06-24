using Holism.Framework;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Holism.Api
{
    public class ListParametersModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType != typeof(ListParameters))
            {
                return null;
            }

            return new BinderTypeModelBinder(typeof(ListParametersModelBinder));
        }
    }
}