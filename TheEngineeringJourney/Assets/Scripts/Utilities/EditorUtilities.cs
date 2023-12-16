using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public static class EditorUtilities 
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
    /// null value debug check
    /// </summary>
    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, Object objectToCheck)
    {
        if (objectToCheck is not null) return false;
        
        Debug.Log(fieldName + " is null and must contain a value in object " + thisObject.name);
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
    
    /// <summary>
    /// positive value debug check- if zero is allowed set isZeroAllowed to true. Returns true if there is an error
    /// </summary>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        if (isZeroAllowed ? valueToCheck >= 0 : valueToCheck > 0) return false;
        
        var str = (isZeroAllowed ? "positive value or zero" : "positive");
        Debug.Log(fieldName + " must contain a " + str + " value in object " + thisObject.name);
        return true;
    }
    
    /// <summary>
    /// positive range debug check - set isZeroAllowed to true if the min and max range values can both be zero. Returns true if there is an error
    /// </summary>
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float valueToCheckMinimum, string fieldNameMaximum, float valueToCheckMaximum, bool isZeroAllowed)
    {
        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + " must be less than or equal to " + fieldNameMaximum + " in object " + thisObject.name.ToString());
            return true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) return true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) return true;

        return false;
    }

    /// <summary>
    /// Create deep copy of string list
    /// </summary>
    public static List<string> CopyStringList(this IEnumerable<string> oldStringList) => 
        oldStringList.ToList();
    
    private static bool IsNull<T>(this IEnumerable<T> enumerable) => enumerable == null;
    
    private static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();
}
