using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Xsl;
using System.Xml;
using System;
using Jint;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Packaging;
using System.Linq;

namespace tex2mathml.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
