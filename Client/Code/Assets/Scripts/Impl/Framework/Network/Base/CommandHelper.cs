using System;
using System.Collections.Generic;
using System.Reflection;
using Google.Protobuf;
using AosBaseFramework;
using AosHotfixRunTime;

namespace AosHotfixFramework
{
    public sealed class CommandHelper
    {
        static Dictionary<ushort, Type> sRspOpcode2Type = new Dictionary<ushort, Type>();
        static Dictionary<Type, ushort> sReqType2Opcode = new Dictionary<Type, ushort>();
        static Dictionary<Type, FieldInfo> sCommandType2FieldInfoDict = new Dictionary<Type, FieldInfo>();
        static Dictionary<ushort, List<MethodInfo>> sOpcode2MethodInfosDict = new Dictionary<ushort, List<MethodInfo>>();
        static Dictionary<ushort, List<MethodInfo>> sModuleID2MethodInfosDict = new Dictionary<ushort, List<MethodInfo>>();
        static object[] sParameterObjs = new object[1];

        public static void Init()
        {
            Type[] tmpTypes = HotFixHelper.GetHotfixTypes();
            
            Type tmpReqCommandAttribType = typeof(ReqCommandAttribute);
            Type tmpRspCommandAttribType = typeof(RspCommandAttribute);
            Type tmpCommandHandlerAttributeType = typeof(CommandHandlerAttribute);
            Type tmpCommandHandleAttributeType = typeof(CommandHandleAttribute);
            Type tmpCmdModuleHandlerAttributeType = typeof(CommandModuleHandlerAttribute);
            Type tmpCmdModuleHandleAttributeType = typeof(CommandModuleHandleAttribute);

            foreach (Type typeElem in tmpTypes)
            {
                //协议信息
                var tmpReqCommandAttributes = typeElem.GetCustomAttributes(tmpReqCommandAttribType, false);
                ReqCommandAttribute tmpReqCommand = tmpReqCommandAttributes.Length > 0 ? tmpReqCommandAttributes[0] as ReqCommandAttribute : null;

                RspCommandAttribute tmpRspCommand = null;
                if (null == tmpReqCommand)
                {
                    var tmpRspCommandAttributes = typeElem.GetCustomAttributes(tmpRspCommandAttribType, false);
                    tmpRspCommand = tmpRspCommandAttributes.Length > 0 ? tmpRspCommandAttributes[0] as RspCommandAttribute : null;
                }

                if (null != tmpReqCommand || null != tmpRspCommand)
                {
                    if (null != tmpReqCommand)
                    {
                        if (!sReqType2Opcode.ContainsKey(typeElem))
                            sReqType2Opcode.Add(typeElem, tmpReqCommand.Opcode);
                    }
                    else
                    {
                        if (!sRspOpcode2Type.ContainsKey(tmpRspCommand.Opcode))
                        {
                            sRspOpcode2Type.Add(tmpRspCommand.Opcode, typeElem);
                        }
                    }

                    if (!sCommandType2FieldInfoDict.ContainsKey(typeElem))
                    {
                        FieldInfo tmpFieldInfo = typeElem.GetField("Data");
                        if (null == tmpFieldInfo)
                        {
                            Logger.LogError("消息中不包含data数据");
                        }
                        else
                        {
                            sCommandType2FieldInfoDict.Add(typeElem, tmpFieldInfo);
                        }
                    }
                }

                //协议模块观察者信息
                do
                {
                    var tmpCmdModuleHandlerAttrs = typeElem.GetCustomAttributes(tmpCmdModuleHandlerAttributeType, false);

                    if (tmpCmdModuleHandlerAttrs.Length == 0)
                    {
                        break;
                    }

                    var tmpCmdModuleHandler = tmpCmdModuleHandlerAttrs[0] as CommandModuleHandlerAttribute;

                    if (null == tmpCmdModuleHandler)
                    {
                        break;
                    }

                    MethodInfo[] tmpMethods = typeElem.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                    foreach (MethodInfo methodInfoElem in tmpMethods)
                    {
                        var tmpCmdModuleHandleAttris = methodInfoElem.GetCustomAttributes(tmpCmdModuleHandleAttributeType, false);

                        if (tmpCmdModuleHandleAttris.Length == 0)
                        {
                            continue;
                        }

                        var tmpCmdModuleHandle = tmpCmdModuleHandleAttris[0] as CommandModuleHandleAttribute;

                        if (null == tmpCmdModuleHandle)
                        {
                            continue;
                        }

                        List<MethodInfo> tmpMethodInfoList = null;

                        if (!sModuleID2MethodInfosDict.TryGetValue(tmpCmdModuleHandle.ModuleID, out tmpMethodInfoList))
                        {
                            tmpMethodInfoList = new List<MethodInfo>();
                            sModuleID2MethodInfosDict.Add(tmpCmdModuleHandle.ModuleID, tmpMethodInfoList);
                        }

                        tmpMethodInfoList.Add(methodInfoElem);
                    }

                } while (false);


                //协议观察者信息
                do
                {
                    var tmpCommandHandlerAttris = typeElem.GetCustomAttributes(tmpCommandHandlerAttributeType, false);

                    if (tmpCommandHandlerAttris.Length == 0)
                    {
                        break;
                    }

                    var tmpCommandHandler = tmpCommandHandlerAttris[0] as CommandHandlerAttribute;

                    if (null == tmpCommandHandler)
                    {
                        break;
                    }

                    MethodInfo[] tmpMethods = typeElem.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                    foreach (MethodInfo methodInfoElem in tmpMethods)
                    {
                        var tmpCommandHandleAttris = methodInfoElem.GetCustomAttributes(tmpCommandHandleAttributeType, false);

                        if (tmpCommandHandleAttris.Length == 0)
                        {
                            continue;
                        }

                        CommandHandleAttribute tmpCommandHandle = tmpCommandHandleAttris[0] as CommandHandleAttribute;

                        if (null == tmpCommandHandle)
                            continue;

                        //ILRT暂不支持GetParameters
                        //ParameterInfo[] tmpParameters = methodInfoElem.GetParameters();

                        //if (tmpParameters.Length > 0 && tmpParameters[0].ParameterType != tmpCommandHandle.CommandType)
                        //{
                        //    Logger.LogError("消息处理方法形参类型错误 " + typeElem + " -> " + methodInfoElem.Name);
                        //    continue;
                        //}

                        List<MethodInfo> tmpMethodInfoList = null;
                        if (!sOpcode2MethodInfosDict.TryGetValue(tmpCommandHandle.Opcode, out tmpMethodInfoList))
                        {
                            tmpMethodInfoList = new List<MethodInfo>();
                            sOpcode2MethodInfosDict.Add(tmpCommandHandle.Opcode, tmpMethodInfoList);
                        }

                        tmpMethodInfoList.Add(methodInfoElem);
                    }
                } while (false);
            }
        }

        public static object Process(BufferReader reader)
        {
            byte tmpFirst = 0, tmpSecond = 0;
            reader.Read(ref tmpFirst).Read(ref tmpSecond);
            ushort tmpOpcode = tmpFirst;
            tmpOpcode <<= 8;
            tmpOpcode |= tmpSecond;

            Type tmpType = null;

            if (!sRspOpcode2Type.TryGetValue(tmpOpcode, out tmpType))
            {
                Logger.LogError($"找不到相应的协议包. First = {tmpFirst} Second = {tmpSecond}");
                return null;
            }

            object tmpCommand = null;
            try
            {
                tmpCommand = Deserialize(reader, tmpType);
            }
            catch (Exception ex)
            {
                Logger.LogError($"协议反序列化异常. type:{tmpType}  异常信息:{ex.Message}");
            }

            if (null != tmpCommand)
            {
                (tmpCommand as CommandBaseBase).First = tmpFirst;
                (tmpCommand as CommandBaseBase).Second = tmpSecond;
                (tmpCommand as CommandBaseBase).Opcode = tmpOpcode;
                List<MethodInfo> tmpMethodInfoList = null;

                if (sOpcode2MethodInfosDict.TryGetValue(tmpOpcode, out tmpMethodInfoList))
                {
                    sParameterObjs[0] = tmpCommand;

                    for (int i = 0, max = tmpMethodInfoList.Count; i < max; ++i)
                    {
                        MethodInfo tmpMethodInfo = tmpMethodInfoList[i];
                        tmpMethodInfoList[i].Invoke(null, sParameterObjs);
                    }
                }
                else
                {
                    //Logger.LogError($"找不到对应的处理者. First = {tmpFirst} Second = {tmpSecond}");
                }


                if (sModuleID2MethodInfosDict.TryGetValue(tmpFirst, out tmpMethodInfoList))
                {
                    sParameterObjs[0] = tmpCommand;

                    for (int i = 0, max = tmpMethodInfoList.Count; i < max; ++i)
                    {
                        MethodInfo tmpMethodInfo = tmpMethodInfoList[i];
                        tmpMethodInfoList[i].Invoke(null, sParameterObjs);
                    }
                }

            }

            return tmpCommand;
        }

        public static void Serialize(BufferWriter writer, object command)
        {
            Type tmpCommandType = command.GetType();
            ushort tmpOpcode = 0;
            if (!sReqType2Opcode.TryGetValue(tmpCommandType, out tmpOpcode))
            {
                Logger.LogError($"此协议没有设置协议号! type = {tmpCommandType}");
                return;
            }

            writer.Write((byte)(tmpOpcode >> 8));
            writer.Write((byte)(tmpOpcode & 0xff));

            FieldInfo tmpFieldInfo = null;
            if (!sCommandType2FieldInfoDict.TryGetValue(tmpCommandType, out tmpFieldInfo))
            {
                return;
            }

            IMessage tmpProtoMsg = tmpFieldInfo.GetValue(command) as IMessage;
            if (null == tmpProtoMsg)
                return;

            tmpProtoMsg.WriteTo(writer.stream);
        }

        public static object Deserialize(BufferReader reader, Type type)
        {
            FieldInfo tmpFieldInfo = null;
            if (!sCommandType2FieldInfoDict.TryGetValue(type, out tmpFieldInfo))
            {
                return null;
            }

            object tmpCommand = Activator.CreateInstance(type);
            IMessage tmpProtoMsg = tmpFieldInfo.GetValue(tmpCommand) as IMessage;

            if (null == tmpProtoMsg)
            {
                return null;
            }
            
            tmpProtoMsg.MergeFrom(reader.stream);
            return tmpCommand;
        }
    }
}
