using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RemoteControl.Server
{
    public class RemoteHub : Hub
    {
        public Task SendMouseMove(int x, int y)
        {
            WindowsInput.MoveMouse(x, y);
            return Task.CompletedTask;
        }

        public Task SendLeftClick()
        {
            WindowsInput.LeftClick();
            return Task.CompletedTask;
        }

        public Task SendRightClick()
        {
            WindowsInput.RightClick();
            return Task.CompletedTask;
        }

        public Task SendType(string txt)
        {
            WindowsInput.TypeText(txt);
            return Task.CompletedTask;
        }
    }
}
