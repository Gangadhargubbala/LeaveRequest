using System.Web.Http;
using WebActivatorEx;
using LeaveRequest;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace LeaveRequest
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "LeaveRequest");

                        //c.ApiKey("Authorization").Description("Enter your JWT token here")
                        //.Name("Authorization").In("header");
                        // Fix for CS0246 and CS1061: Use OpenApiSecurityScheme instead of ApiKeyScheme  
                        c.ApiKey("Authorization")
                    .Description("JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"")
                    .Name("Authorization")
                    .In("header");
                    })
                    
                .EnableSwaggerUi(c =>
                    {
                        c.EnableApiKeySupport("Authorization", "header");
                    });
        }
    }
}
