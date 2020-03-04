using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace RecipeManagement.Api.Infrastructure.Routing
{
    public class CamelCaseRoutingConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            foreach (SelectorModel selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel.Template = $"/api/{CamelCase(controller.ControllerName)}";
                }
            }
        }

        private static string CamelCase(string controllerName)
        {
            return $"{controllerName.Substring(0, 1).ToLower()}{controllerName.Substring(1)}";
        }
    }
}