using System.Web.Mvc;

namespace LCCS_School_Parent_Communication_System.Areas.Academic_Director
{
    public class Academic_DirectorAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Academic_Director";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Academic_Director_default",
                "Academic_Director/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}