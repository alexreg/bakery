﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Cake.ScriptServer.Reflection
{
    public sealed class MethodSignature
    {
        public string Name { get; set; }
        public ObsoleteAttribute Obsolete { get; set; }
        public TypeSignature ReturnType { get; }
        public TypeSignature DeclaringType { get; }
        public List<string> GenericParameters { get; }
        public IReadOnlyList<ParameterSignature> Parameters { get; }

        private MethodSignature(
            string name,
            ObsoleteAttribute obsolete,
            TypeSignature declaringType,
            TypeSignature returnType,
            IEnumerable<string> genericParameters,
            IEnumerable<ParameterSignature> parameters)
        {
            Name = name;
            Obsolete = obsolete;
            ReturnType = returnType;
            DeclaringType = declaringType;
            GenericParameters = new List<string>(genericParameters);
            Parameters = new List<ParameterSignature>(parameters);
        }

        public static MethodSignature Create(MethodDefinition method)
        {
            // Get the method Identity and name.
            var name = GetMethodName(method);

            // Get the declaring type and return type.
            var declaringType = TypeSignature.Create(method.DeclaringType);
            var returnType = TypeSignature.Create(method.ReturnType);

            // Get generic parameters and arguments.
            var genericParameters = new List<string>();
            if (method.HasGenericParameters)
            {
                // Generic parameters
                genericParameters.AddRange(
                    method.GenericParameters.Select(
                        genericParameter => genericParameter.Name));
            }

            // Get all parameters.
            var parameters = method.Parameters.Select(ParameterSignature.Create).ToList();

            // Return the method signature.
            return new MethodSignature(
                name, method.GetObsoleteAttribute(), declaringType, returnType, genericParameters, parameters);
        }

        private static string GetMethodName(MethodDefinition definition)
        {
            if (definition.IsConstructor)
            {
                var name = definition.DeclaringType.Name;
                var index = name.IndexOf('`');
                if (index != -1)
                {
                    name = name.Substring(0, index);
                }
                return name;
            }
            return definition.Name;
        }
    }
}
