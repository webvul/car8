using System;
using MyCmn;

namespace MyCmn
{
    /// <summary>
    /// 资源接口
    /// </summary>
    public interface IRes
    {
        StringLinker GetRes(string Key);
    }
}