using System;
using System.Reflection;

namespace AosBaseFramework
{
	public class MonoInstanceMethod : IInstanceMethod
	{
		private readonly object obj;
		private readonly MethodInfo methodInfo;
		private readonly object[] param;

		public MonoInstanceMethod(Type type, string methodName, object typeInstance = null)
		{
            if (null == typeInstance)
                this.obj = Activator.CreateInstance(type);
            else
                this.obj = typeInstance;

			this.methodInfo = type.GetMethod(methodName);
			this.param = new object[this.methodInfo.GetParameters().Length];
		}

		public void Run()
		{
			this.methodInfo.Invoke(this.obj, param);
		}

		public void Run(object a)
		{
			this.param[0] = a;
			this.methodInfo.Invoke(this.obj, param);
		}

		public void Run(object a, object b)
		{
			this.param[0] = a;
			this.param[1] = b;
			this.methodInfo.Invoke(this.obj, param);
		}

		public void Run(object a, object b, object c)
		{
			this.param[0] = a;
			this.param[1] = b;
			this.param[2] = c;
			this.methodInfo.Invoke(this.obj, param);
		}
	}

	public class MonoStaticMethod : IStaticMethod
	{
		private readonly MethodInfo methodInfo;

		private readonly object[] param;

		public MonoStaticMethod(Type type, string methodName)
		{
			this.methodInfo = type.GetMethod(methodName);
			this.param = new object[this.methodInfo.GetParameters().Length];
		}

		public void Run()
		{
			this.methodInfo.Invoke(null, param);
		}

		public void Run(object a)
		{
			this.param[0] = a;
			this.methodInfo.Invoke(null, param);
		}

		public void Run(object a, object b)
		{
			this.param[0] = a;
			this.param[1] = b;
			this.methodInfo.Invoke(null, param);
		}

		public void Run(object a, object b, object c)
		{
			this.param[0] = a;
			this.param[1] = b;
			this.param[2] = c;
			this.methodInfo.Invoke(null, param);
		}
	}
}
