using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace Oracle.RightNow.Cti.CtiServiceProvider.ServiceModel {
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    //public class KnownAssemblyAttribute : System.Attribute, IContractBehavior {
    //    MessagesDataContractResolver _resolver;

    //    public KnownAssemblyAttribute(string name) {
    //        this.Assembly = name;
    //        _resolver = new MessagesDataContractResolver();
    //    }

    //    public string Assembly { get; set; }

    //    public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) {
    //    }

    //    public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime) {
    //        CreateMyDataContractSerializerOperationBehaviors(contractDescription);
    //    }

    //    public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime) {
    //        CreateMyDataContractSerializerOperationBehaviors(contractDescription);
    //    }

    //    public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint) {
    //    }

    //    internal void CreateMyDataContractSerializerOperationBehaviors(ContractDescription contractDescription) {
    //        foreach (var operation in contractDescription.Operations) {
    //            CreateMyDataContractSerializerOperationBehavior(operation);
    //        }
    //    }

    //    internal void CreateMyDataContractSerializerOperationBehavior(OperationDescription operation) {
    //        DataContractSerializerOperationBehavior dataContractSerializerOperationbehavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
    //        dataContractSerializerOperationbehavior.DataContractResolver = this._resolver;
    //    }
    //}

    //public class MessagesDataContractResolver : DataContractResolver {
    //    XmlDictionary _dictionary = new XmlDictionary();
    //    Assembly _assembly;

    //    public MessagesDataContractResolver() {
    //        var messageType = typeof(Oracle.RightNow.Cti.CtiServiceProvider.Messaging.Messages.Message);
    //        KnownTypes = new List<Type>(messageType.Assembly.GetTypes().Where(t => messageType.IsAssignableFrom(t)));
    //        //KnownTypes = new List<Type>();
    //        //

    //        //_assembly = Assembly.Load(new AssemblyName(assemblyName));
    //        //foreach (Type type in _assembly.GetTypes()) {
    //        //    if (!messageType.IsAssignableFrom(type))
    //        //        continue;

    //        //    bool knownTypeFound = false;
    //        //    System.Attribute[] attrs = System.Attribute.GetCustomAttributes(type);
    //        //    if (attrs.Length != 0) {
    //        //        foreach (System.Attribute attr in attrs) {
    //        //            if (attr is KnownTypeAttribute) {
    //        //                Type t = ((KnownTypeAttribute)attr).Type;
    //        //                if (this.KnownTypes.IndexOf(t) < 0) {
    //        //                    // Adding the type to the known types list
    //        //                    this.KnownTypes.Add(t);
    //        //                }
    //        //                knownTypeFound = true;
    //        //            }
    //        //        }
    //        //    }
    //        //    if (!knownTypeFound) {
    //        //        // Add the name and namespace of the type into the dictionary
    //        //        string name = type.Name;
    //        //        string namesp = type.Namespace;
    //        //        XmlDictionaryString result;
    //        //        if (!_dictionary.TryLookup(name, out result)) {
    //        //            _dictionary.Add(name);
    //        //        }
    //        //        if (!_dictionary.TryLookup(namesp, out result)) {
    //        //            _dictionary.Add(namesp);
    //        //        }
    //        //    }
    //        //}
    //    }

    //    public IList<Type> KnownTypes {
    //        get;
    //        set;
    //    }

    //    // Deserialization
    //    public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver) {
    //        var type = KnownTypes.FirstOrDefault(t => string.Compare(t.Name, typeName) == 0);

    //        if (type != null) {
    //            return type;
    //        }
    //        else {
    //            return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
    //        }
    //    }

    //    // Serialization
    //    public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace) {
    //        if (KnownTypes.Contains(type)) {
    //            typeName = new XmlDictionaryString(XmlDictionary.Empty, type.Name, 0);
    //            typeNamespace = new XmlDictionaryString(XmlDictionary.Empty, type.Namespace, 0);
    //            return true;
    //        }
    //        typeName = null;
    //        typeNamespace = null;
    //        return false;
    //    }
    //}
}
