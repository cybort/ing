﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Sakuno.ING.Composition;
using Sakuno.ING.Localization;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell.Layout;
using Sakuno.UserInterface;
using Sakuno.UserInterface.Controls;

namespace Sakuno.ING.Shell.Desktop
{
    [Export(typeof(IShell))]
    internal class DesktopShell : FlexibleShell<FrameworkElement>, IShell
    {
        private readonly LayoutSetting layoutSetting;
        private readonly ILocalizationService localization;
        private readonly string localeName;
        private readonly FontFamily userFont;
        private readonly List<Window> layoutWindows = new List<Window>();
        private LayoutRoot layout;

        public DesktopShell(LayoutSetting layoutSetting, ILocalizationService localization, LocaleSetting locale)
            : base(localization)
        {
            this.layoutSetting = layoutSetting;
            this.localization = localization;
            localeName = locale.Language.Value;
            var userFontName = locale.UserLanguageFont.Value;
            if (!string.IsNullOrEmpty(userFontName))
                userFont = new FontFamily(userFontName);
        }

        public void Run()
        {
            var app = new ThemedApp
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };

            app.Startup += (s, e) =>
            {
                Window window;
                try
                {
                    if (layoutSetting.XamlString.Value.IsNullOrEmpty())
                        LayoutFallback();
                    else
                        layout = (LayoutRoot)XamlReader.Parse(layoutSetting.XamlString.Value);
                }
                catch
                {
                    LayoutFallback();
                }

                window = new MainWindow { MainContent = layout.MainWindow.LoadContent() };

                InitWindow(window);

                Application.Current.MainWindow = window;
                window.Show();

                void LayoutFallback()
                {
                    layout = (LayoutRoot)Application.LoadComponent(new Uri("/Sakuno.ING.Shell.Desktop;component/Layout/Default.xaml", UriKind.Relative));
                }
            };

            app.Run();
        }

        private void InitWindow(Window window)
        {
            if (!string.IsNullOrEmpty(localeName))
                window.Language = XmlLanguage.GetLanguage(localeName);
            if (userFont != null)
                window.FontFamily = userFont;
        }

        public void SwitchWindow(string windowId)
        {
            var windows = Application.Current.Windows;
            for (int i = 0; i < windows.Count; i++)
            {
                var w = windows[i];
                if (windowId.Equals(w.Tag))
                {
                    w.Activate();
                    return;
                }
            }

            if (windowId == "Settings")
            {
                var w = new SettingsWindow { DataContext = CreateSettingViews() };
                InitWindow(w);
                w.Show();
                w.Activate();
            }

            var view = layout[windowId];
            if (view != null)
            {
                var w = new ModernWindow
                {
                    Tag = windowId,
                    Title = localization.GetLocalized("ViewTitle", windowId) ?? windowId,
                    Content = view.LoadContent(),
                };
                InitWindow(w);
                w.Show();
                w.Activate();
            }
        }
    }
}
