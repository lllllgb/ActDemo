using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IBaseLogger
{
    void Log(object value);
    void LogFormat(string format, params object[] args);
    
    void LogWarning(object value);
    void LogWarningFormat(string format, params object[] args);
    
    void LogError(object value);
    void LogErrorFormat(string format, params object[] args);
}
