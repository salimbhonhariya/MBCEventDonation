//------------------------------------------------------------------------------
// Generated by the DevExpress.Blazor package.
// To prevent this operation, add the DxExtendStartupHost property to the project and set this property to False.
//
// MBCEventDonation.csproj:
//
// <Project Sdk="Microsoft.NET.Sdk.Web">
//  <PropertyGroup>
//    <TargetFramework>netcoreapp3.0</TargetFramework>
//    <DxExtendStartupHost>False</DxExtendStartupHost>
//  </PropertyGroup>
//------------------------------------------------------------------------------
using System;
using System.IO;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(MBCEventDonation.DevExpressHostingStartup))]

namespace MBCEventDonation {
    public partial class DevExpressHostingStartup : IHostingStartup {
        void IHostingStartup.Configure(IWebHostBuilder builder) {
            builder.ConfigureServices((serviceCollection) => {
                serviceCollection.AddDevExpressBlazor();
            });

            #if DEBUG
                        try
                        {
                            File.WriteAllText("browsersync-update.txt", DateTime.Now.ToString());
                        }
                        catch
                        {
                            // ignore
                        }
            #endif

        }
    }
}
