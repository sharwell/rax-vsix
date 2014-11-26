namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System;
    using System.Windows;

    public static class CustomWindowProperties
    {
        // Using a DependencyProperty as the backing store for CanMinimize.
        public static readonly DependencyProperty CanMinimizeProperty =
            DependencyProperty.RegisterAttached("CanMinimize", typeof(bool), typeof(Window), new PropertyMetadata(true, HandleCanMinimizeChanged));

        // Using a DependencyProperty as the backing store for CanMaximize.
        public static readonly DependencyProperty CanMaximizeProperty =
            DependencyProperty.RegisterAttached("CanMaximize", typeof(bool), typeof(Window), new PropertyMetadata(true, HandleCanMaximizeChanged));

        public static bool GetCanMinimize(DependencyObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            return (bool)obj.GetValue(CanMinimizeProperty);
        }

        public static void SetCanMinimize(DependencyObject obj, bool value)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            obj.SetValue(CanMinimizeProperty, value);
        }

        public static bool GetCanMaximize(DependencyObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            return (bool)obj.GetValue(CanMaximizeProperty);
        }

        public static void SetCanMaximize(DependencyObject obj, bool value)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            obj.SetValue(CanMaximizeProperty, value);
        }

        private static void HandleCanMinimizeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Window window = dependencyObject as Window;
            if (window == null || !(e.NewValue is bool))
                return;

            bool enableButton = (bool)e.NewValue;
            if (enableButton)
                window.Activated -= HandleWindowActivated_CanMinimize;
            else
                window.Activated += HandleWindowActivated_CanMinimize;
        }

        private static void HandleCanMaximizeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Window window = dependencyObject as Window;
            if (window == null || !(e.NewValue is bool))
                return;

            bool enableButton = (bool)e.NewValue;
            if (enableButton)
                window.Activated -= HandleWindowActivated_CanMaximize;
            else
                window.Activated += HandleWindowActivated_CanMaximize;
        }

        private static void HandleWindowActivated_CanMinimize(object sender, EventArgs e)
        {
            Window window = sender as Window;
            if (window == null || GetCanMinimize(window))
                return;

            if (window.ResizeMode == ResizeMode.NoResize)
                return;

            window.HideMinimizeButton();
        }

        private static void HandleWindowActivated_CanMaximize(object sender, EventArgs e)
        {
            Window window = sender as Window;
            if (window == null || GetCanMaximize(window))
                return;

            if (window.ResizeMode == ResizeMode.NoResize)
                return;

            window.HideMaximizeButton();
        }
    }
}
