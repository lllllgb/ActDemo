using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Intepreter;

namespace AosBaseFramework
{
	public class ILInstanceMethod : IInstanceMethod
	{
		private readonly ILRuntime.Runtime.Enviorment.AppDomain mAppDomain;
		private readonly ILTypeInstance mInstance;
		private readonly IMethod mMethod;
		private readonly object[] mParams;

		public ILInstanceMethod(ILRuntime.Runtime.Enviorment.AppDomain appDomain, Type type, string methodName)
		{
            mAppDomain = appDomain;
			this.mInstance = this.mAppDomain.Instantiate(type.FullName);
			this.mMethod = this.mInstance.Type.GetMethod(methodName);
			int n = this.mMethod.ParameterCount;
			this.mParams = new object[n];
		}

		public void Run()
		{
			this.mAppDomain.Invoke(this.mMethod, this.mInstance, mParams);
		}

		public void Run(object a)
		{
			this.mParams[0] = a;
			this.mAppDomain.Invoke(this.mMethod, this.mInstance, mParams);
		}

		public void Run(object a, object b)
		{
			this.mParams[0] = a;
			this.mParams[1] = b;
			this.mAppDomain.Invoke(this.mMethod, this.mInstance, mParams);
		}

		public void Run(object a, object b, object c)
		{
			this.mParams[0] = a;
			this.mParams[1] = b;
			this.mParams[2] = c;
			this.mAppDomain.Invoke(this.mMethod, this.mInstance, mParams);
		}
	}

	public class ILStaticMethod : IStaticMethod
	{
		private readonly ILRuntime.Runtime.Enviorment.AppDomain mAppDomain;
		private readonly IMethod mMethod;
		private readonly object[] mParams;

		public ILStaticMethod(ILRuntime.Runtime.Enviorment.AppDomain appDomain, string typeName, string methodName, int paramsCount)
		{
            mAppDomain = appDomain;
			this.mMethod = appDomain.GetType(typeName).GetMethod(methodName, paramsCount);
			this.mParams = new object[paramsCount];
		}

		public void Run()
		{
			this.mAppDomain.Invoke(this.mMethod, null, this.mParams);
		}

		public void Run(object a)
		{
			this.mParams[0] = a;
			this.mAppDomain.Invoke(this.mMethod, null, mParams);
		}

		public void Run(object a, object b)
		{
			this.mParams[0] = a;
			this.mParams[1] = b;
			this.mAppDomain.Invoke(this.mMethod, null, mParams);
		}

		public void Run(object a, object b, object c)
		{
			this.mParams[0] = a;
			this.mParams[1] = b;
			this.mParams[2] = c;
			this.mAppDomain.Invoke(this.mMethod, null, mParams);
		}
	}
}
