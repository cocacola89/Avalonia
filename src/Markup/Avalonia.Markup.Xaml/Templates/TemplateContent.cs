// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Markup.Xaml.Context;
using NameScope = Avalonia.Controls.NameScope;

#if SYSTEM_XAML
using System.Xaml;
using System.Windows.Markup;
#else
using Portable.Xaml;
using Portable.Xaml.Markup;
#endif

namespace Avalonia.Markup.Xaml.Templates
{
    [XamlDeferLoad(typeof(TemplateLoader), typeof(IControl))]
    public class TemplateContent
    {
        private NameScopeAdapter _namescope = new NameScopeAdapter();
        private XamlObjectWriter _writer;

        public TemplateContent(XamlReader reader, IXamlObjectWriterFactory factory)
        {
            var settings = factory.GetParentSettings();
            settings.ExternalNameScope = _namescope;
            settings.RegisterNamesOnExternalNamescope = true;
            settings.RootObjectInstance = null;

            _writer = factory.GetXamlObjectWriter(settings);
            List = new XamlNodeList(reader.SchemaContext);

            XamlServices.Transform(reader, List.Writer);
        }

        public XamlNodeList List { get; }

        public IControl Load()
        {
            var reader = List.GetReader();

            _writer.Clear();
            XamlServices.Transform(reader, _writer, false);

            var nameScope = _namescope.Extract();

            if (_writer.Result is StyledElement s)
            {
                NameScope.SetNameScope(s, nameScope);
            }

            return (IControl)_writer.Result;
        }
    }
}
