#pragma checksum "C:\Users\filip\source\repos\ChatApp\ChatApp\Views\Home\UsersList.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "33820b2c9e865acd306a1e7c5c4405992975b9f6"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_UsersList), @"mvc.1.0.view", @"/Views/Home/UsersList.cshtml")]
namespace AspNetCore
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
#line 1 "C:\Users\filip\source\repos\ChatApp\ChatApp\Views\_ViewImports.cshtml"
using ChatApp;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\filip\source\repos\ChatApp\ChatApp\Views\_ViewImports.cshtml"
using ChatApp.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"33820b2c9e865acd306a1e7c5c4405992975b9f6", @"/Views/Home/UsersList.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6cf30333ff74047b044a1c577267f3d6907496f9", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_UsersList : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<div id=""myModal"" class=""modal"">
    <div class=""modal-content"">
        <div class=""modal-profile-inner"">
            <input placeholder=""Search""/>
            <button class=""modal-profile-button""><i class=""fas fa-search""></i></button>
            <div class=""modal-profile-list"">
                <ul>
                </ul>
            </div>
            <div id=""modal-arrow"">
                <i class=""fas fa-angle-down""></i>
            </div>
        </div>
        <div>
            <span class=""close"">&times;</span>
        </div>
    </div>
</div>
");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
