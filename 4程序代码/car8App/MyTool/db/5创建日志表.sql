use MyLog
go

CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[Type] [varchar](50) NULL,
	[Url] [varchar](1024) NULL,
	[Title] [varchar](200) NULL,
	[Doer] [varchar](200) NULL,
	[Request] [varchar](max) NULL,
	[Msg] [varchar](1000) NULL,
	[Detail] [varchar](max) NULL,
	[Exception] [varchar](1000) NULL,
	[Value] [bigint] NULL,
	[AddTime] [datetime] NULL,
	[IP] [varchar](50) NULL,
	[Client] [varchar](200) NULL,
	[UserName] [varchar](50) NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'子系统名称 或 模块名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Log', @level2type=N'COLUMN',@level2name=N'Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'信息类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Log', @level2type=N'COLUMN',@level2name=N'Type'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发生页面的标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Log', @level2type=N'COLUMN',@level2name=N'Title'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发生页面的事件源' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Log', @level2type=N'COLUMN',@level2name=N'Doer'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'请求数据' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Log', @level2type=N'COLUMN',@level2name=N'Request'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'消息正文' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Log', @level2type=N'COLUMN',@level2name=N'Msg'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'消息详情' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Log', @level2type=N'COLUMN',@level2name=N'Detail'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数值型消息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Log', @level2type=N'COLUMN',@level2name=N'Value'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'添加时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Log', @level2type=N'COLUMN',@level2name=N'AddTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户端IP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Log', @level2type=N'COLUMN',@level2name=N'IP'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'客户端详细信息，如浏览器，操作系统等' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Log', @level2type=N'COLUMN',@level2name=N'Client'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前用户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Log', @level2type=N'COLUMN',@level2name=N'UserName'
GO


