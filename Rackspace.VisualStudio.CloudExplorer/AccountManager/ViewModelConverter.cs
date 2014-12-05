namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;

    public class ViewModelConverter : IValueConverter
    {
        private static readonly ConcurrentDictionary<Type, ViewModelCache> _viewModels =
            new ConcurrentDictionary<Type, ViewModelCache>();

        public Type ViewModelType
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type viewModelType;
            if (parameter == null)
            {
                viewModelType = ViewModelType;
                if (viewModelType == null)
                    throw new ArgumentException("The view model converter does not have a default view model type specified.");
            }
            else
            {
                viewModelType = parameter as Type;
                if (viewModelType == null)
                    throw new ArgumentException("Converter parameter must be a view model type.");
            }

            ViewModelCache viewModelCache = _viewModels.GetOrAdd(viewModelType, CreateViewModelCache);
            return viewModelCache.ViewToViewModel.GetValue(value, key => CreateViewModel(viewModelCache, key, viewModelType));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type viewModelType;
            if (parameter == null)
            {
                viewModelType = ViewModelType;
                if (viewModelType == null)
                    throw new ArgumentException("The view model converter does not have a default view model type specified.");
            }
            else
            {
                viewModelType = parameter as Type;
                if (viewModelType == null)
                    throw new ArgumentException("Converter parameter must be a view model type.");
            }

            ViewModelCache viewModelCache;
            if (!_viewModels.TryGetValue(viewModelType, out viewModelCache))
                return null;

            object model;
            if (!viewModelCache.ViewModelToView.TryGetValue(value, out model))
                return null;

            return model;
        }

        private static object CreateViewModel(ViewModelCache viewModelCache, object model, Type viewModelType)
        {
            object viewModel = Activator.CreateInstance(viewModelType, model);
            viewModelCache.ViewModelToView.Add(viewModel, model);
            return viewModel;
        }

        private static ViewModelCache CreateViewModelCache(Type viewModelType)
        {
            return new ViewModelCache();
        }

        private sealed class ViewModelCache
        {
            public readonly ConditionalWeakTable<object, object> ViewToViewModel = new ConditionalWeakTable<object, object>();
            public readonly ConditionalWeakTable<object, object> ViewModelToView = new ConditionalWeakTable<object, object>();
        }
    }
}
