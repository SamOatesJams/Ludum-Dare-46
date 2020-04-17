using UnityEngine;
using System;
using SamOatesGames.Systems.Core;

namespace SamOatesGames.Systems
{
    public class VariableResolvingSystem : UnitySingleton<VariableResolvingSystem>
    {
        /// <summary>
        /// Resolve a given string variable into its actual value.
        /// </summary>
        /// <param name="variable">The variable to resolve, split via fullstops e.g. Class.Variable</param>
        /// <returns></returns>
        public string ResolveVariable(string variable)
        {
            var parts = variable.Split('.');
            if (parts.Length != 2)
            {
                Debug.LogErrorFormat("The variable '{0}' is in the wrong format. It must be Class.Variable.", variable);
                return variable;
            }

            var className = parts[0];
            var propertyName = parts[1];

            var classType = Type.GetType(className);
            if (classType == null)
            {
                Debug.LogErrorFormat("The variable '{0}' uses an unknown class '{1}'.", variable, className);
                return variable;
            }

            var propertyType = classType.GetProperty(propertyName);
            if (propertyType == null)
            {
                Debug.LogErrorFormat("The variable '{0}' uses an unknown property '{1}' from class '{2}'.", variable, propertyName, className);
                return variable;
            }

            return propertyType.GetValue(null, null).ToString();
        }
    }
}
