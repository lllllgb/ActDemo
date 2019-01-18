using System;
using System.Reflection;
using ILRuntime.Runtime.Enviorment;
using UnityEngine;
using System.Collections.Generic;

namespace AosBaseFramework
{
	public static class ILHelper
	{
		public static unsafe void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
		{
            // 注册重定向函数

            // 注册委托
            appdomain.DelegateManager.RegisterFunctionDelegate<KeyValuePair<String, Int32>, Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<KeyValuePair<String, Int32>, String>();            appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32>();            appdomain.DelegateManager.RegisterFunctionDelegate<Adapt_IMessage.Adaptor>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Threading.Tasks.Task>();            appdomain.DelegateManager.RegisterMethodDelegate<System.Object, ILRuntime.Runtime.Intepreter.ILTypeInstance>();            appdomain.DelegateManager.RegisterMethodDelegate<System.IAsyncResult>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.PointerEventData>();            appdomain.DelegateManager.RegisterMethodDelegate<AosBaseFramework.SimpleFsm.State>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>();

            appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.EventHandler<ILRuntime.Runtime.Intepreter.ILTypeInstance>((sender, e) =>
                {
                    ((Action<System.Object, ILRuntime.Runtime.Intepreter.ILTypeInstance>)act)(sender, e);
                });
            });            appdomain.DelegateManager.RegisterDelegateConvertor<System.AsyncCallback>((act) =>
            {
                return new System.AsyncCallback((ar) =>
                {
                    ((Action<System.IAsyncResult>)act)(ar);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UGUIEventListener.VoidDelegate>((act) =>
            {
                return new UGUIEventListener.VoidDelegate((eventData) =>
                {
                    ((Action<UnityEngine.EventSystems.PointerEventData>)act)(eventData);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<AosBaseFramework.SimpleFsm.State.StateFunction>((act) =>
            {
                return new AosBaseFramework.SimpleFsm.State.StateFunction((s) =>
                {
                    ((Action<AosBaseFramework.SimpleFsm.State>)act)(s);
                });
            });            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>((arg0, arg1) =>
                {
                    ((Action<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>)act)(arg0, arg1);
                });
            });

            // 注册适配器
            appdomain.RegisterCrossBindingAdaptor(new Adapt_IMessage());
            appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor());
        }
    }
}