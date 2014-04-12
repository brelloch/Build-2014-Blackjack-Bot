using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BlackjackBot.Shared
{
    /// <summary>
    /// A helper class used to display properties and corresponding values for an instance of a class.
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Used to return name/value pairs of property name/property value for an instance of a class.
        /// </summary>
        /// <typeparam name="T">The type you want information from</typeparam>
        /// <param name="data">The instance of the class to get data from</param>
        /// <param name="propertiesToExclude">a list of properties to exclude</param>
        /// <returns>A string representing the type</returns>
        public static string GetTypes<T>(T data, List<string> propertiesToExclude)
        {
            StringBuilder sb = new StringBuilder();

            Type thisClass = typeof(T);

            var props = thisClass.GetRuntimeProperties();

            foreach (PropertyInfo i in props)
            {
                if (!propertiesToExclude.Contains(i.Name))
                {
                    sb.AppendFormat("{0}: {1}" + Environment.NewLine, i.Name, i.GetValue(data, null));
                }                
            }

            return sb.ToString(); 
        }

        /// <summary>
        /// A method to display property names and values
        /// </summary>
        /// <typeparam name="T">The type to display</typeparam>
        /// <param name="data">The instance to show property name/values</param>
        /// <returns>A string representing the types</returns>
        public static string GetTypes<T>(T data)
        {
            return GetTypes(data, new List<string>()); 
        }
    }
}
