using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.RightNow.Cti.MediaBar;

namespace Oracle.RightNow.Cti {
    public class DemoServiceProvider : IServiceProvider {
        private readonly Dictionary<Type, Type> _serviceTypes;
        private readonly Dictionary<Type, object> _services;

        public DemoServiceProvider() {
            _serviceTypes = new Dictionary<Type, Type>();
            _services = new Dictionary<Type, object>();

            InitializeDefaultServiceTypes();
        }

        protected virtual void InitializeDefaultServiceTypes() {
            _serviceTypes.Add(typeof(IMediaBarProvider), Type.GetType("Oracle.RightNow.Cti.MediaBar.MediaBar, Oracle.RightNow.Cti.MediaBar"));
        }

        public object GetService(Type serviceType) {
            Type registeredType;
            object instance = null;
            if (_serviceTypes.TryGetValue(serviceType, out registeredType)) {
                if (!_services.TryGetValue(registeredType, out instance)) {
                    instance = Activator.CreateInstance(registeredType);
                    _services.Add(registeredType, instance);
                }
            }
            
            return instance;
        }
    }

    public static class IServiceProviderExtensions {
        public static T GetService<T>(this IServiceProvider serviceProvider) where T :class {
            return serviceProvider.GetService(typeof(T)) as T;
        }
    }
}