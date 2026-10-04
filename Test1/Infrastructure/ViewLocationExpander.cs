using Microsoft.AspNetCore.Mvc.Razor;

namespace IDS.Web.UI.Infrastructure
{
    public class AreaSubfolderViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // nothing needed here
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Report")
            {
                var newLocations = new[]
                {
                    "/Areas/Report/Views/GLReport/{1}/{0}.cshtml",
                    "/Areas/Report/Views/Factoring/{1}/{0}.cshtml"
                };

                return newLocations.Concat(viewLocations);
            }
            return viewLocations;
        }
    }
}
