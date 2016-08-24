﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WiseLabs.Analytics
{
    using System;
    using System.Collections.Generic;
    
    #line 1 "..\..\DashboardPage.cshtml"
    using System.Configuration;
    
    #line default
    #line hidden
    
    #line 2 "..\..\DashboardPage.cshtml"
    using System.Linq;
    
    #line default
    #line hidden
    using System.Text;
    
    #line 3 "..\..\DashboardPage.cshtml"
    using WiseLabs.Analytics;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class DashboardPage : WebTemplateBase
    {
#line hidden

        public override void Execute()
        {



WriteLiteral("\r\n");


WriteLiteral(@"
<h1 id=""PageTitle"">
    Dashboard
</h1>

<style>
    .funnel { box-sizing: border-box; list-style-type: none; background-color: #ddd; margin: 0px; padding: 0px; width: 300px; }
    .funnel li .bar { background-color: #000; }
    .funnel li + li .bar { background-color: #333; }
    .funnel li + li + li .bar { background-color: #666; }
    .funnel li + li + li + li .bar { background-color: #999; }
    .funnel li + li + li + li + li .bar { background-color: #ccc; }

    .funnel li {
     width: 300px;   
     position: relative;
     height: 40px;
    }

    .text {
         position: absolute;   
         text-align: center;
         color: #fff; 
         width: 100%;
         top: 10px;
    }

    .bar {
         position: absolute;   
         height: 40px;
    }
</style>

");


            
            #line 39 "..\..\DashboardPage.cshtml"
  
    var funnel = ConfigurationManager.AppSettings["WiseLabs.Analytics.Funnel"].Split(',');
    var cohorts = Tracker.GetEvents(funnel[0])
        .GroupBy(x => x.CohortName)
        .OrderByDescending(x => x.Key);


            
            #line default
            #line hidden
WriteLiteral("\r\n<p>\r\n    Cohors found: ");


            
            #line 47 "..\..\DashboardPage.cshtml"
             Write(cohorts.Count());

            
            #line default
            #line hidden
WriteLiteral("\r\n</p>\r\n\r\n");


            
            #line 50 "..\..\DashboardPage.cshtml"
 foreach (var cohort in cohorts)
{
    var firstEvent = cohort.SingleOrDefault(x => x.EventName == funnel[0]);
    double max = firstEvent == null ? 0 : firstEvent.EventCount;
    if (max != 0)
    {

            
            #line default
            #line hidden
WriteLiteral("        <h3>");


            
            #line 56 "..\..\DashboardPage.cshtml"
       Write(cohort.Key);

            
            #line default
            #line hidden
WriteLiteral("</h3>\r\n");



WriteLiteral("        <ul class=\"funnel\">\r\n\r\n");


            
            #line 59 "..\..\DashboardPage.cshtml"
             foreach (var step in funnel)
            {
                var e = cohort.SingleOrDefault(x => x.EventName == step);
                var percent = e == null ? 0 : (double) e.EventCount * 100 / max;
                var pixels = percent * 3;


            
            #line default
            #line hidden
WriteLiteral("                <li>\r\n                    ");


            
            #line 66 "..\..\DashboardPage.cshtml"
               Write(WiseLabs.Analytics.Html.Raw("<div class='bar' style='width: " + pixels + "px;'></div>"));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    <span class=\"text\">");


            
            #line 67 "..\..\DashboardPage.cshtml"
                                   Write(step + ": " + percent.ToString("00.00") + "% (" + (e == null ? 0 : e.EventCount) + ")");

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n                    \r\n                </li>\r\n");


            
            #line 70 "..\..\DashboardPage.cshtml"
                
            }

            
            #line default
            #line hidden
WriteLiteral("        </ul>\r\n");


            
            #line 73 "..\..\DashboardPage.cshtml"
    }
}

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 76 "..\..\DashboardPage.cshtml"
  
    
    var experiments = SplitTesting.GetExperiments();
    var experimentData = SplitTesting.GetEventsForExperiments()
        .GroupBy(x => x.ExperimentId)
        .OrderByDescending(x => x.Key);


            
            #line default
            #line hidden
WriteLiteral("\r\n<p>\r\n    Experiments found: ");


            
            #line 85 "..\..\DashboardPage.cshtml"
                  Write(experimentData.Count());

            
            #line default
            #line hidden
WriteLiteral("\r\n</p>\r\n\r\n");


            
            #line 88 "..\..\DashboardPage.cshtml"
 foreach (var experiment in experimentData)
{
    var firstEvent = experiment.SingleOrDefault(x => x.EventName == funnel[0]);
    double max = firstEvent == null ? 0 : firstEvent.EventCount;
    if (max != 0)
    {

            
            #line default
            #line hidden
WriteLiteral("        <h3>Experiment #");


            
            #line 94 "..\..\DashboardPage.cshtml"
                   Write(experiment.Key);

            
            #line default
            #line hidden
WriteLiteral(": ");


            
            #line 94 "..\..\DashboardPage.cshtml"
                                    Write(experiments.Single(x => x.ExperimentId == experiment.Key).Name);

            
            #line default
            #line hidden
WriteLiteral("</h3>\r\n");



WriteLiteral("        <ul class=\"funnel\">\r\n\r\n");


            
            #line 97 "..\..\DashboardPage.cshtml"
             foreach (var step in funnel)
            {
                var e = experiment.SingleOrDefault(x => x.EventName == step);
                var percent = e == null ? 0 : (double) e.EventCount * 100 / max;
                var pixels = percent * 3;


            
            #line default
            #line hidden
WriteLiteral("                <li>\r\n                    ");


            
            #line 104 "..\..\DashboardPage.cshtml"
               Write(WiseLabs.Analytics.Html.Raw("<div class='bar' style='width: " + pixels + "px;'></div>"));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    <span class=\"text\">");


            
            #line 105 "..\..\DashboardPage.cshtml"
                                   Write(step + ": " + percent + "% (" + (e == null ? 0 : e.EventCount) + ")");

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n                </li>\r\n");


            
            #line 107 "..\..\DashboardPage.cshtml"

            }

            
            #line default
            #line hidden
WriteLiteral("        </ul>\r\n");


            
            #line 110 "..\..\DashboardPage.cshtml"
    }
}

            
            #line default
            #line hidden
WriteLiteral("<p>\r\n    Generated by Visual Studio extension RazorGenerator: https://visualstudi" +
"ogallery.msdn.microsoft.com/1f6ec6ff-e89b-4c47-8e79-d2d68df894ec\r\n</p>\r\n");


        }
    }
}
#pragma warning restore 1591
