using System.Runtime.CompilerServices;

public class ConfirmInfo
{
    public ConfirmInfo(bool success, string msg) { this.success = success; this.msg = msg; }
    public bool success;
    public string msg;
}