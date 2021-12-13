using System.Web.Mvc;

namespace LCCS_School_Parent_Communication_System.Areas.Unit_Leader
{
    public class Unit_LeaderAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Unit_Leader";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Unit_Leader_default",
                "Unit_Leader/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}