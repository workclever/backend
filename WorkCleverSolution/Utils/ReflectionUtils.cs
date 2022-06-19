namespace WorkCleverSolution.Utils;

public static class ReflectionUtils
{
    public static string GetObjectPropertyValue(object obj, string propertyName)
    {
        var propertyInfo = obj.GetType().GetProperty(propertyName);
        if (propertyInfo != null)
        {
            var propType = propertyInfo.PropertyType;
            if (propType.IsGenericType &&
                propType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propType = propType.GetGenericArguments()[0];
            }

            if (propType == typeof(string))
            {
                return (string) propertyInfo.GetValue(obj);
            }

            if (propType == typeof(int))
            {
                return ((int) propertyInfo.GetValue(obj)).ToString();
            }

            if (propType == typeof(bool))
            {
                return ((bool) propertyInfo.GetValue(obj)).ToString();
            }
        }

        throw new System.ApplicationException("PROPERTY_OF_OBJECT_NOT_FOUND");
    }

    public static void SetObjectProperty(object obj, string propertyName, string value)
    {
        var propertyInfo = obj.GetType().GetProperty(propertyName);
        if (propertyInfo != null)
        {
            var propType = propertyInfo.PropertyType;
            if (propType.IsGenericType &&
                propType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propType = propType.GetGenericArguments()[0];
            }

            if (propType == typeof(string))
            {
                propertyInfo.SetValue(obj, value, null);
            }

            if (propType == typeof(int))
            {
                // client sends 'null', we have restrictions
                if (value == "null")
                {
                    propertyInfo.SetValue(obj, null, null);
                }
                else
                {
                    propertyInfo.SetValue(obj, Int32.Parse(value), null);
                }
               
            }
            else if (propType == typeof(bool))
            {
                propertyInfo.SetValue(obj, Boolean.Parse(value), null);
            }
        }
    }
}