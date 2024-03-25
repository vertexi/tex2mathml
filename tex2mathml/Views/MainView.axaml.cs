using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using DocumentFormat.OpenXml.Packaging;
using Jint;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Xsl;
using System.Xml;
using System;
using TextCopy;
using AvaloniaWebView;
using System.Reflection;
using Avalonia.Notification;
using Avalonia.Controls.ApplicationLifetimes;

namespace tex2mathml.Views;

public partial class MainView : UserControl
{
    Engine engine;
    public MainView()
    {
        InitializeComponent();

        engine = new Engine().SetValue("log", new Action<object>(logout));
        string temml = System.IO.File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\assets\temml.min.js");
        engine.Execute(temml);

        ConvertAndCopyButton.Click += ConvertAndCopyButton_Click;

        PART_WebView.WebViewNewWindowRequested += PART_WebView_WebViewNewWindowRequested;

        formula.TextChanged += Formula_TextChanged;
    }

    private void Formula_TextChanged(object? sender, TextChangedEventArgs e)
    {
        var mathML = new object();
        engine.SetValue("inputtext", formula.Text.ToString());
        try
        {
            mathML = engine.Evaluate(@"temml.renderToString(inputtext, { displayMode: true })").ToObject();
            if (mathML.ToString().IndexOf("error") == 0)
            {
                mathML = ReplaceFirstOccurrence(mathML.ToString(), @"<math>", @"<math xmlns=""http://www.w3.org/1998/Math/MathML"">");
            }
            else {
                mathML = @"<p>" + mathML + "</p>";
            }
        }
        catch (Exception jsEngineErr)
        {
            mathML = @"<p>" + jsEngineErr.Message + "</p>";
        }
        finally
        {
            var mathMlHtml = @"<!DOCTYPE html>

    <html>
      <head>
        <meta charset=""UTF-8"" />
        <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
        <title>How to Make Dark Mode for Websites using HTML CSS & JavaScript ?</title>
        <style>
          body {
            padding: 25px;
            background-color: white;
            color: black;
            font-size: 25px;
          }

          .dark-mode {
            background-color: black;
            color: white;
          }

          .light-mode {
            background-color: white;
            color: black;
          }
        </style>
      </head>
      <body>
        " + mathML.ToString() + @"

        <script>
          let element = document.body;
          element.className = ""dark-mode"";
        </script>
      </body>
    </html>
    ";
            PART_WebView.HtmlContent = mathMlHtml;
        }
    }

    private void PART_WebView_WebViewNewWindowRequested(object? sender, WebViewCore.Events.WebViewNewWindowEventArgs e)
    {
        e.UrlLoadingStrategy = WebViewCore.Enums.UrlRequestStrategy.OpenInNewWindow;
    }

    public string mml2omml(string mml)
    {
        string officeML = string.Empty;

        Console.WriteLine(mml);
        XslCompiledTransform xslTransform = new XslCompiledTransform();

        // The OMML2MML.xsl file is located under 
        // %ProgramFiles%\Microsoft Office\Office15\
        xslTransform.Load(@"./assets/MML2OMML.XSL");

        using (TextReader tr = new StringReader(mml))
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
    public string omml2mml(string mathParagraphXml)
    {
        string officeML = string.Empty;

        Console.WriteLine(mathParagraphXml);
        XslCompiledTransform xslTransform = new XslCompiledTransform();

        // The OMML2MML.xsl file is located under 
        // %ProgramFiles%\Microsoft Office\Office15\
        xslTransform.Load(@"./assets/OMML2MML.XSL");

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

    public string GetWordDocumentAsMathML(string docFilePath)
    {
        string officeML = string.Empty;
        using (WordprocessingDocument doc = WordprocessingDocument.Open(docFilePath, false))
        {
            string wordDocXml = doc.MainDocumentPart.Document.OuterXml;
            //string mathParagraphXml = doc.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Math.Paragraph>().First().OuterXml;

            XslCompiledTransform xslTransform = new XslCompiledTransform();

            // The OMML2MML.xsl file is located under 
            // %ProgramFiles%\Microsoft Office\Office15\
            xslTransform.Load(@"./assets/OMML2MML.XSL");

            using (TextReader tr = new StringReader(wordDocXml))
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
        }
        return officeML;
    }

    private string ReplaceFirstOccurrence(string Source, string Find, string Replace)
    {
        int Place = Source.IndexOf(Find);
        string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
        return result;
    }

    private void logout(object? value)
    {
        Debug.WriteLine(value);
    }

    private void ConvertAndCopyButton_Click(object? sender, RoutedEventArgs e)
    {
        Process p = new Process();
        p.StartInfo.FileName = "./assets/pandoc.exe";
        p.StartInfo.Arguments = "-f markdown --mathml -t docx -o temp.docx";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        string formula_str = formula.Text.ToString().Replace(@"\cfrac", @"\dfrac");
        p.StandardInput.WriteLine("$$\n" + formula_str + "\n$$");
        p.StandardInput.Flush();
        p.StandardInput.Close();
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit(); // This waits until the program called is closed


        //Process p = new Process();
        //p.StartInfo.FileName = "./assets/texmath.exe";
        //p.StartInfo.Arguments = "-f mathml -t omml";
        //p.StartInfo.UseShellExecute = false;
        //p.StartInfo.RedirectStandardInput = true;
        //p.StartInfo.RedirectStandardOutput = true;
        //p.StartInfo.RedirectStandardError = true;
        //p.StartInfo.CreateNoWindow = true;
        //p.Start();
        //p.StandardInput.WriteLine(mathML);
        //p.StandardInput.Flush();
        //p.StandardInput.Close();
        //string output = p.StandardOutput.ReadToEnd();
        //p.WaitForExit(); // This waits until the program called is closed

        //Debug.WriteLine("Output:");
        //Debug.WriteLine(output);

        //var result = ReplaceFirstOccurrence(output, @"<m:oMathPara>", @"<m:oMathPara xmlns:m=""http://schemas.openxmlformats.org/officeDocument/2006/math"">");

        Debug.WriteLine("Convert Output:");

        string final_result = GetWordDocumentAsMathML("temp.docx");
        //string final_result = omml2mml(mml2omml(mathML.ToString()));

        mathml.Text = final_result;

        ClipboardService.SetText(final_result);
    }
}
