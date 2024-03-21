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
        //this.Manager
        //    .CreateMessage()
        //    .Accent("#F15B19")
        //    .Background("#F15B19")
        //    .HasHeader("Lost connection to server")
        //    .HasMessage("Reconnecting...")
        //    .WithOverlay(new ProgressBar
        //    {
        //        VerticalAlignment = VerticalAlignment.Bottom,
        //        HorizontalAlignment = HorizontalAlignment.Stretch,
        //        Height = 3,
        //        BorderThickness = new Thickness(0),
        //        Foreground = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
        //        Background = Brushes.Transparent,
        //        IsIndeterminate = true,
        //        IsHitTestVisible = false
        //    })
        //    .Queue();
        this.Manager
            .CreateMessage()
            .Accent("#1751C3")
            .Animates(true)
            .Background("#333")
            .HasBadge("Info")
            .HasMessage(
                "Update will be installed on next application restart. This message will be dismissed after 5 seconds.")
            .Dismiss().WithButton("Update now", button => { })
            .Dismiss().WithButton("Release notes", button => { })
            .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
            .Queue();
    }
}
