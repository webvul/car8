﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.17929
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------



[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IOleDbService")]
public interface IOleDbService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOleDbService/ReadAsMyOqlSet", ReplyAction="http://tempuri.org/IOleDbService/ReadAsMyOqlSetResponse")]
    string ReadAsMyOqlSet(string excelFileName, string SheetName);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOleDbService/ReadAsMyOqlSet", ReplyAction="http://tempuri.org/IOleDbService/ReadAsMyOqlSetResponse")]
    System.Threading.Tasks.Task<string> ReadAsMyOqlSetAsync(string excelFileName, string SheetName);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOleDbService/GetExcelSheetName", ReplyAction="http://tempuri.org/IOleDbService/GetExcelSheetNameResponse")]
    string GetExcelSheetName(string excelFileName, int sheetIndex);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOleDbService/GetExcelSheetName", ReplyAction="http://tempuri.org/IOleDbService/GetExcelSheetNameResponse")]
    System.Threading.Tasks.Task<string> GetExcelSheetNameAsync(string excelFileName, int sheetIndex);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOleDbService/GetExcelColumns", ReplyAction="http://tempuri.org/IOleDbService/GetExcelColumnsResponse")]
    string[] GetExcelColumns(string excelFileName, string SheetName);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOleDbService/GetExcelColumns", ReplyAction="http://tempuri.org/IOleDbService/GetExcelColumnsResponse")]
    System.Threading.Tasks.Task<string[]> GetExcelColumnsAsync(string excelFileName, string SheetName);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IOleDbServiceChannel : IOleDbService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class OleDbServiceClient : System.ServiceModel.ClientBase<IOleDbService>, IOleDbService
{
    
    public OleDbServiceClient()
    {
    }
    
    public OleDbServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public OleDbServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public OleDbServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public OleDbServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public string ReadAsMyOqlSet(string excelFileName, string SheetName)
    {
        return base.Channel.ReadAsMyOqlSet(excelFileName, SheetName);
    }
    
    public System.Threading.Tasks.Task<string> ReadAsMyOqlSetAsync(string excelFileName, string SheetName)
    {
        return base.Channel.ReadAsMyOqlSetAsync(excelFileName, SheetName);
    }
    
    public string GetExcelSheetName(string excelFileName, int sheetIndex)
    {
        return base.Channel.GetExcelSheetName(excelFileName, sheetIndex);
    }
    
    public System.Threading.Tasks.Task<string> GetExcelSheetNameAsync(string excelFileName, int sheetIndex)
    {
        return base.Channel.GetExcelSheetNameAsync(excelFileName, sheetIndex);
    }
    
    public string[] GetExcelColumns(string excelFileName, string SheetName)
    {
        return base.Channel.GetExcelColumns(excelFileName, SheetName);
    }
    
    public System.Threading.Tasks.Task<string[]> GetExcelColumnsAsync(string excelFileName, string SheetName)
    {
        return base.Channel.GetExcelColumnsAsync(excelFileName, SheetName);
    }
}
