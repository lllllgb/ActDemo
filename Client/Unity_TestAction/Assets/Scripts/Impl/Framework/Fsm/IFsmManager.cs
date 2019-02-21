using System;

namespace AosHotfixFramework
{
    /// <summary>
    /// 有限状态机管理器。
    /// </summary>
    public interface IFsmManager
    {
        int Count
        {
            get;
        }
        
        bool HasFsm<T>() where T : class;
        
        IFsm<T> GetFsm<T>() where T : class;
        
        FsmBase[] GetAllFsms();
        
        IFsm<T> CreateFsm<T>(T owner) where T : class;
        
        bool DestroyFsm<T>() where T : class;

        bool DestroyFsm(FsmBase fsm);
    }
}
