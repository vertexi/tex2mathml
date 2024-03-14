using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Xsl;
using System.Xml;
using System;

namespace tex2mathml.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var ConvertButton = this.FindControl<Button>("ConvertButton");
        ConvertButton.Click += ConvertButton_Click;

        var copyButton = this.FindControl<Button>("copyButton");
        copyButton.Click += CopyButton_Click;
    }

    private void CopyButton_Click(object? sender, RoutedEventArgs e)
    {
        Clipboard.SetTextAsync(mathml.Text);
    }

    public string GetWordDocumentAsMathML(string mathParagraphXml, string officeVersion = "14")
    {
        string officeML = string.Empty;

        string testMathXml = @"<m:oMathPara xmlns:m=""http://schemas.openxmlformats.org/officeDocument/2006/math"">
</m:oMathPara>";
        Console.WriteLine(mathParagraphXml);
        XslCompiledTransform xslTransform = new XslCompiledTransform();

        // The OMML2MML.xsl file is located under 
        // %ProgramFiles%\Microsoft Office\Office15\
        xslTransform.Load(@"c:\Program Files\Microsoft Office\root\Office16" + @"\OMML2MML.XSL");

        using (TextReader tr = new StringReader(mathParagraphXml))
        {
            // Load the xml of your main document part.
            using (XmlReader reader = XmlReader.Create(tr))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlWriterSettings settings = xslTransform.OutputSettings.Clone();

                    // Configure xml writer to omit xml declaration.
                    settings.ConformanceLevel = ConformanceLevel.Fragment;
                    settings.OmitXmlDeclaration = true;

                    XmlWriter xw = XmlWriter.Create(ms, settings);

                    // Transform our OfficeMathML to MathML.
                    xslTransform.Transform(reader, xw);
                    ms.Seek(0, SeekOrigin.Begin);

                    using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
                    {
                        officeML = sr.ReadToEnd();
                        // Console.Out.WriteLine(officeML);
                    }
                }
            }
        }
        return officeML;
    }

    private string ReplaceFirstOccurrence(string Source, string Find, string Replace)
    {
        int Place = Source.IndexOf(Find);
        string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
        return result;
    }

    private void ConvertButton_Click(object? sender, RoutedEventArgs e)
    {
        Process p = new Process();
        p.StartInfo.FileName = "./assets/texmath.exe";
        p.StartInfo.Arguments = "-f tex -t omml";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        p.StandardInput.WriteLine($"{formula.Text}\n");
        p.StandardInput.Flush();
        p.StandardInput.Close();
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit(); // This waits until the program called is closed

        Debug.WriteLine("Output:");
        Debug.WriteLine(output);

        var result = ReplaceFirstOccurrence(output, @"<m:oMathPara>", @"<m:oMathPara xmlns:m=""http://schemas.openxmlformats.org/officeDocument/2006/math"">");

        Debug.WriteLine("Convert Output:");
        string final_result = (GetWordDocumentAsMathML(result));

        mathml.Text = final_result;
    }
}
