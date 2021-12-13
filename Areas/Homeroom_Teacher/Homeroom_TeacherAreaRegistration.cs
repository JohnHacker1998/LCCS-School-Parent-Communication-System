using System.Web.Mvc;

namespace LCCS_School_Parent_Communication_System.Areas.Homeroom_Teacher
{
    public class Homeroom_TeacherAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Homeroom_Teacher";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Homeroom_Teacher_default",
                "Homeroom_Teacher/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}