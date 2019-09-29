namespace System
{
    public static class TypeExtensions
    {
        public static bool AssignableTo(this Type @this, Type other)
        {
            return other.IsAssignableFrom(@this);
        }

        public static bool AssignableTo<T>(this Type @this)
        {
            return @this.AssignableTo(typeof (T));
        }

        public static bool CanBeInstantiated(this Type @this)
        {
            return @this.IsClass && !@this.IsAbstract;
        }

        public static bool Is<TOther>(this Type @this)
        {
            return @this == (typeof (TOther));
        }

        public static bool IsNot<TOther>(this Type @this)
        {
            return !@this.Is<TOther>();
        }

        public static string ToFriendlyName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.FullName;
            }

            string name = type.GetGenericTypeDefinition().FullName;
            if (name == null)
                return type.Name;

            name = name.Substring(0, name.IndexOf('`'));
            name += "<";

            Type[] arguments = type.GetGenericArguments();
            for (int i = 0; i < arguments.Length; i++)
            {
                if (i > 0)
                    name += ",";

                name += arguments[i].Name;
            }

            name += ">";

            return name;
        }
    }
}