﻿using System.Web;
using System.Web.Optimization;

namespace TMDT.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                "~/Assets/js/kendo/kendo.all.min.js",
                // "~/Scripts/kendo/kendo.timezones.min.js", // uncomment if using the Scheduler
                "~/Assets/js/kendo/kendo.aspnetmvc.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"                      
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"                     
                      ));
            bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
               "~/Assets/css/kendo/kendo.common-bootstrap.min.css",
               "~/Assets/css/kendo/kendo.bootstrap.min.css"));

            bundles.IgnoreList.Clear();
        }
    }
}
