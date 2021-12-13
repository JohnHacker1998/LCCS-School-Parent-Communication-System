using System.Web.Mvc;

namespace LCCS_School_Parent_Communication_System.Areas.Registrar
{
    public class RegistrarAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Registrar";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Registrar_default",
                "Registrar/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}