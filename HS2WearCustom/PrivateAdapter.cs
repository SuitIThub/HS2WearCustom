using System;
using System.Reflection;

namespace HS2WearCustom
{
	// Token: 0x02000004 RID: 4
	public static class PrivateAdapter
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002190 File Offset: 0x00000390
		public static T GetPrivateField<T>(this object instance, string fieldname)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			return (T)((object)instance.GetType().GetField(fieldname, bindingAttr).GetValue(instance));
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000021B8 File Offset: 0x000003B8
		public static T GetPrivateProperty<T>(this object instance, string propertyname)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			return (T)((object)instance.GetType().GetProperty(propertyname, bindingAttr).GetValue(instance, null));
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000021E4 File Offset: 0x000003E4
		public static void SetPrivateField(this object instance, string fieldname, object value)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			instance.GetType().GetField(fieldname, bindingAttr).SetValue(instance, value);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002208 File Offset: 0x00000408
		public static void SetPrivateProperty(this object instance, string propertyname, object value)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			instance.GetType().GetProperty(propertyname, bindingAttr).SetValue(instance, value, null);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002230 File Offset: 0x00000430
		public static T CallPrivateMethod<T>(this object instance, string name, params object[] param)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			return (T)((object)instance.GetType().GetMethod(name, bindingAttr).Invoke(instance, param));
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000225C File Offset: 0x0000045C
		public static void CallPrivateMethod(this object instance, string name, params object[] param)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			instance.GetType().GetMethod(name, bindingAttr).Invoke(instance, param);
		}
	}
}
