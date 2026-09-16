using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Turune.Models
{
    public class MenuOption
    {
        public string Key { get; set; }
        public List<MenuOptionDescription> Description { get; set; } = new(); //In order to make descriptions include things like URLs and line breaks,
                                                                              //they have to be represented as a list of parts rather than a single string.
        
        public List<MenuOption> Children { get; set; } = new(); //Sub-options, allowing for nested menus.

        public bool HasChildren => Children.Count > 0;
    }

    //Descriptions can be made up of multiple parts, including text, links, and line breaks.
    public abstract class MenuOptionDescription { }

    public class TextPart : MenuOptionDescription
    {
        public string Text { get; set; }
    }

    public class LinkPart : MenuOptionDescription
    {
        public string Text { get; set; }
        public string Url { get; set; }
    }

    public class LineBreakPart : MenuOptionDescription { }

    //Renderer for the MenuOptionDescription parts to display them in a WPF TextBlock or similar control.
    public static class DescriptionRenderer
    {
        public static void Render(InlineCollection target, IEnumerable<MenuOptionDescription> parts)
        {
            target.Clear(); //Clear the target collection before adding new parts.

            if (parts == null) return;

            foreach (var part in parts)
            {
                switch (part)
                {
                    case TextPart t:
                        target.Add(new Run(t.Text));
                        break;

                    case LineBreakPart:
                        target.Add(new LineBreak());
                        break;

                    case LinkPart l:
                        var link = new Hyperlink(new Run(l.Text))
                        {
                            NavigateUri = new Uri(l.Url)
                        };
                        link.RequestNavigate += OnRequestNavigate;
                        target.Add(link);
                        break;
                }
            }
        }

        //Used for URL links, opens the link in the default internet browser.
        private static void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true
            });
            e.Handled = true;
        }
    }
}
