using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Notification;
using System;

namespace tex2mathml.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";
    public INotificationMessageManager Manager { get; } = new NotificationMessageManager();
    public void ButtonBaseInfoOnClick()
    {
        this.Manager
            .CreateMessage()
            .Accent("#1751C3")
            .Animates(true)
            .Background("#333")
            .HasBadge("Info")
            .HasMessage(
                "Convert done.")
            .Dismiss().WithButton("😊", button => { })
            .Dismiss().WithButton("👍", button => { })
            .Dismiss().WithDelay(TimeSpan.FromSeconds(1))
            .Queue();
    }
}
