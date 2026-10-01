using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UserManagement.AM.Composition;
using UserManagement.AM.RequestDTOValidator;

namespace FileProcessing.WebSolution.ModuleComposition
{
    public static class GlobalValidatorAssemblyDependencyInjection
    {
        public static IServiceCollection GlobalValidatorAssemblyConfiguration(this IServiceCollection service)
        {
            /*   service.AddValidatorsFromAssemblyContaining<SignupRequestDtoValidator>(); //auto scan current assembly and apply all validator which is running inside the current assembly
               service.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); //both are same work the differece is only declaration of syntax.
            */

            //create array of all assemblies
            var assemblies = new Assembly[]
            {
                new UserManagementAssemblyProvider().AssemblyName,
                //another assemblies
            };
            service.AddValidatorsFromAssemblies(assemblies);
            return service;
        }
    }
}




