using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public static class HelperUtilities 
{

    /// <summary>
    /// Empty string debug check
    /// </summary>
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (!string.IsNullOrEmpty(stringToCheck)) return false;
        
        Debug.Log($"{fieldName} is empty and must contain a value in object {thisObject.name}");
        return true;
    }
    
    /// <summary>
    /// List empty or coa
    /// </summary>
    public static bool ValidateCheckEnumerableValues<T>(Object thisObject, string fieldName,
        IEnumerable<T> enumerableObjectToCheck)
    {
        if (IsNull(enumerableObjectToCheck))
        {
            Debug.Log($"{fieldName} is empty and must contain a value in object {thisObject.name}");
            return true;
        }
        
        if (IsEmpty(enumerableObjectToCheck))
        {
            Debug.Log($"{fieldName} has no values in object {thisObject.name}");
            return true;
        }
        
        return false;
    }
    
    private static bool IsNull<T>(this IEnumerable<T> enumerable) => enumerable == null;
    
    private static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();
}
