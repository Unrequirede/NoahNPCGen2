#pragma checksum "C:\Users\apple\source\repos\NoahNPCGen2\NoahNPCGen\Pages\_Host.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e6ebe1358c8f04ef525fac3077bb119f41484d16"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(NoahNPCGen.Pages.Pages__Host), @"mvc.1.0.razor-page", @"/Pages/_Host.cshtml")]
namespace NoahNPCGen.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\apple\source\repos\NoahNPCGen2\NoahNPCGen\Pages\_ViewImports.cshtml"
using NoahNPCGen;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemMetadataAttribute("RouteTemplate", "/blazor")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e6ebe1358c8f04ef525fac3077bb119f41484d16", @"/Pages/_Host.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9793a388f638fc20ac76800c21259fa9a5bdae70", @"/Pages/_ViewImports.cshtml")]
    #nullable restore
    public class Pages__Host : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(" \n");
#nullable restore
#line 3 "C:\Users\apple\source\repos\NoahNPCGen2\NoahNPCGen\Pages\_Host.cshtml"
  
    Layout = "_Layout";

#line default
#line hidden
#nullable disable
            WriteLiteral(" \n<app>\n    ");
#nullable restore
#line 8 "C:\Users\apple\source\repos\NoahNPCGen2\NoahNPCGen\Pages\_Host.cshtml"
Write(await Html.RenderComponentAsync<App>(RenderMode.Server));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n</app>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Pages__Host> Html { get; private set; } = default!;
        #nullable disable
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<Pages__Host> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<Pages__Host>)PageContext?.ViewData;
        public Pages__Host Model => ViewData.Model;
    }
}
#pragma warning restore 1591
