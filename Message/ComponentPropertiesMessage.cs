using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMKernelUI.Message;
/// <summary>
///  部件属性变化
/// </summary>
/// <param name="Value"></param>
/// <remarks>Message</remarks>
public record ComponentPropertyChangedMessage( object? Value = null );

/// <summary>
/// 部件连接变化
/// </summary>
/// <param name="Value"></param>
/// <remarks>Message</remarks>
public record ConnectionChangedMessage( object? Value = null );

/// <summary>
/// 部件连接名称变化
/// </summary>
/// <remarks>Message</remarks>
public record ComponentConnectionNameChangedMessage( int ComponentID, string NewName );