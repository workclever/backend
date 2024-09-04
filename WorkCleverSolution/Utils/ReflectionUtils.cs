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
                return (string)propertyInfo.GetValue(obj);
            }

            if (propType == typeof(int))
            {
                return ((int)propertyInfo.GetValue(obj)).ToString();
            }

            if (propType == typeof(bool))
            {
                return ((bool)propertyInfo.GetValue(obj)).ToString();
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
            bool isNullable = propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (isNullable)
            {
                propType = Nullable.GetUnderlyingType(propType);
            }

            if (value == null)
            {
                propertyInfo.SetValue(obj, null, null);
            }
            else if (propType == typeof(string))
            {
                propertyInfo.SetValue(obj, value, null);
            }
            else if (propType == typeof(int))
            {
                if (int.TryParse(value, out int intValue))
                {
                    propertyInfo.SetValue(obj, intValue, null);
                }
                else
                {
                    throw new ArgumentException($"Cannot convert '{value}' to Int32.");
                }
            }
            else if (propType == typeof(bool))
            {
                if (bool.TryParse(value, out bool boolValue))
                {
                    propertyInfo.SetValue(obj, boolValue, null);
                }
                else
                {
                    throw new ArgumentException($"Cannot convert '{value}' to Boolean.");
                }
            }
            // Add more type checks as needed
            else
            {
                throw new NotSupportedException($"Type {propType} is not supported.");
            }
        }
        else
        {
            throw new ArgumentException($"Property '{propertyName}' not found on object of type {obj.GetType().Name}.");
        }
    }
}