using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace LCCS_School_Parent_Communication_System
{
    public class BundleConfig
    {
        public static void RegisterBuldles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
          "~/Scripts/jquery-ui-1.13.0.js"));
            //css  
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                         "~/Content/themes/base/jquery-ui.css",
                         "~/Content/themes/base/jquery-ui.all.css"
                         //"~/Content/themes/base/jquery.ui.datepicker.css"
                         //"~/Content/themes/base/jquery-ui.css",
                         //"~/Content/themes/base/jquery.ui.core.css",
                         //"~/Content/themes/base/jquery.ui.resizable.css",
                         //"~/Content/themes/base/jquery.ui.selectable.css",
                         //"~/Content/themes/base/jquery.ui.accordion.css",
                         //"~/Content/themes/base/jquery.ui.autocomplete.css",
                         //"~/Content/themes/base/jquery.ui.button.css",
                         //"~/Content/themes/base/jquery.ui.dialog.css",
                         //"~/Content/themes/base/jquery.ui.slider.css",
                         //"~/Content/themes/base/jquery.ui.tabs.css",
                         //"~/Content/themes/base/jquery.ui.datepicker.css",
                         //"~/Content/themes/base/jquery.ui.progressbar.css",
                         //"~/Content/themes/base/jquery.ui.theme.css"
                         ));


        }
    }
}