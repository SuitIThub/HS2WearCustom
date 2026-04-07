using System;
using System.Reflection;

namespace HS2WearCustom
{
	public static class PrivateAdapter
	{
		public static T GetPrivateField<T>(this object instance, string fieldname)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			return (T)((object)instance.GetType().GetField(fieldname, bindingAttr).GetValue(instance));
		}

		public static T GetPrivateProperty<T>(this object instance, string propertyname)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			return (T)((object)instance.GetType().GetProperty(propertyname, bindingAttr).GetValue(instance, null));
		}

		public static void SetPrivateField(this object instance, string fieldname, object value)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			instance.GetType().GetField(fieldname, bindingAttr).SetValue(instance, value);
		}

		public static void SetPrivateProperty(this object instance, string propertyname, object value)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			instance.GetType().GetProperty(propertyname, bindingAttr).SetValue(instance, value, null);
		}

		public static T CallPrivateMethod<T>(this object instance, string name, params object[] param)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			return (T)((object)instance.GetType().GetMethod(name, bindingAttr).Invoke(instance, param));
		}

		public static void CallPrivateMethod(this object instance, string name, params object[] param)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			instance.GetType().GetMethod(name, bindingAttr).Invoke(instance, param);
		}
	}
}
