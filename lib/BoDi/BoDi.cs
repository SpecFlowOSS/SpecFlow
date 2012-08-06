/**************************************************************************************
 * 
 * BoDi: A very simple IoC container, easily embeddable also as a source code. 
 * 
 * BoDi was created to support SpecFlow (http://www.specflow.org) by Gaspar Nagy (http://gasparnagy.blogspot.com/)
 * 
 * Project source & unit tests: http://github.com/gasparnagy/BoDi
 * License: Simplified BSD License
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 * 
 */
using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace BoDi
{
#if !BODI_LIMITEDRUNTIME
    [Serializable]
#endif
    public class ObjectContainerException : Exception
    {
        public ObjectContainerException(string message, IEnumerable<Type> resolutionPath) : base(GetMessage(message, resolutionPath))
        {
        }

#if !BODI_LIMITEDRUNTIME
        protected ObjectContainerException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
#endif

        static private string GetMessage(string message, IEnumerable<Type> resolutionPath)
        {
            if (resolutionPath == null || !resolutionPath.Any())
                return message;

            return string.Format("{0} (resolution path: {1})", message, string.Join("->", resolutionPath.Select(t => t.FullName).ToArray()));
        }
    }

    public interface IObjectContainer: IDisposable
    {
        /// <summary>
        /// Registeres a type as the desired implementation type of an interface.
        /// </summary>
        /// <param name="name">A name to register named instance, otherwise null.</param>
        /// <typeparam name="TType">Implementation type</typeparam>
        /// <typeparam name="TInterface">Interface will be resolved</typeparam>
        /// <exception cref="ObjectContainerException">If there was already a resolve for the <typeparamref name="TInterface"/>.</exception>
        /// <remarks>
        ///     <para>Previous registrations can be overriden before the first resolution for the <typeparamref name="TInterface"/>.</para>
        /// </remarks>
        void RegisterTypeAs<TType, TInterface>(string name = null) where TType : class, TInterface;

        /// <summary>
        /// Registers an instance 
        /// </summary>
        /// <typeparam name="TInterface">Interface will be resolved</typeparam>
        /// <param name="name">A name to register named instance, otherwise null.</param>
        /// <param name="instance">The instance implements the interface.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="instance"/> is null.</exception>
        /// <exception cref="ObjectContainerException">If there was already a resolve for the <typeparamref name="TInterface"/>.</exception>
        /// <remarks>
        ///     <para>Previous registrations can be overriden before the first resolution for the <typeparamref name="TInterface"/>.</para>
        ///     <para>The instance will be registered in the object pool, so if a <see cref="Resolve{T}()"/> (for another interface) would require an instance of the dynamic type of the <paramref name="instance"/>, the <paramref name="instance"/> will be returned.</para>
        /// </remarks>
        void RegisterInstanceAs<TInterface>(TInterface instance, string name = null) where TInterface : class;

        /// <summary>
        /// Registers an instance 
        /// </summary>
        /// <param name="name">A name to register named instance, otherwise null.</param>
        /// <param name="instance">The instance implements the interface.</param>
        /// <param name="interfaceType">Interface will be resolved</param>
        /// <exception cref="ArgumentNullException">If <paramref name="instance"/> is null.</exception>
        /// <exception cref="ObjectContainerException">If there was already a resolve for the <paramref name="interfaceType"/>.</exception>
        /// <remarks>
        ///     <para>Previous registrations can be overriden before the first resolution for the <paramref name="interfaceType"/>.</para>
        ///     <para>The instance will be registered in the object pool, so if a <see cref="Resolve{T}()"/> (for another interface) would require an instance of the dynamic type of the <paramref name="instance"/>, the <paramref name="instance"/> will be returned.</para>
        /// </remarks>
        void RegisterInstanceAs(object instance, Type interfaceType, string name = null);

        /// <summary>
        /// Resolves an implementation object for an interface or type.
        /// </summary>
        /// <typeparam name="T">The interface or type.</typeparam>
        /// <returns>An object implementing <typeparamref name="T"/>.</returns>
        /// <remarks>
        ///     <para>The container pools the objects, so if the interface is resolved twice or the same type is registered for multiple interfaces, a single instance is created and returned.</para>
        /// </remarks>
        T Resolve<T>();

        /// <summary>
        /// Resolves an implementation object for an interface or type.
        /// </summary>
        /// <param name="name">A name to resolve named instance, otherwise null.</param>
        /// <typeparam name="T">The interface or type.</typeparam>
        /// <returns>An object implementing <typeparamref name="T"/>.</returns>
        /// <remarks>
        ///     <para>The container pools the objects, so if the interface is resolved twice or the same type is registered for multiple interfaces, a single instance is created and returned.</para>
        /// </remarks>
        T Resolve<T>(string name);

        /// <summary>
        /// Resolves an implementation object for an interface or type.
        /// </summary>
        /// <param name="typeToResolve">The interface or type.</param>
        /// <param name="name">A name to resolve named instance, otherwise null.</param>
        /// <returns>An object implementing <paramref name="typeToResolve"/>.</returns>
        /// <remarks>
        ///     <para>The container pools the objects, so if the interface is resolved twice or the same type is registered for multiple interfaces, a single instance is created and returned.</para>
        /// </remarks>
        object Resolve(Type typeToResolve, string name = null);
    }

    public interface IContainedInstance
    {
        IObjectContainer Container { get; }
    }

    public class ObjectContainer : IObjectContainer
    {
        private const string REGISTERED_NAME_PARAMETER_NAME = "registeredName";

        private struct RegistrationKey
        {
            public readonly Type Type;
            public readonly string Name;

            public RegistrationKey(Type type, string name)
            {
                if (type == null) throw new ArgumentNullException("type");

                Type = type;
                Name = name;
            }

            public override string ToString()
            {
                Debug.Assert(Type.FullName != null);
                if (Name == null)
                    return Type.FullName;

                return string.Format("{0}('{1}')", Type.FullName, Name);
            }

            bool Equals(RegistrationKey other)
            {
                return Equals(other.Type, Type) && String.Equals(other.Name, Name, StringComparison.CurrentCultureIgnoreCase);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != typeof(RegistrationKey)) return false;
                return Equals((RegistrationKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return Type.GetHashCode();
                }
            }
        }
        private interface IRegistration
        {
            object Resolve(ObjectContainer container, RegistrationKey keyToResolve, IEnumerable<Type> resolutionPath);
        }

        private class TypeRegistration : IRegistration
        {
            readonly Type ImplementationType;

            public TypeRegistration(Type implementationType)
            {
                ImplementationType = implementationType;
            }

            public object Resolve(ObjectContainer container, RegistrationKey keyToResolve, IEnumerable<Type> resolutionPath)
            {
                var pooledObjectKey = new RegistrationKey(ImplementationType, keyToResolve.Name);
                object obj = container.GetPooledObject(pooledObjectKey);

                if (obj == null)
                {
                    if (ImplementationType.IsInterface)
                        throw new ObjectContainerException("Interface cannot be resolved: " + keyToResolve, resolutionPath);

                    obj = container.CreateObject(ImplementationType, resolutionPath, keyToResolve);
                    container.objectPool.Add(pooledObjectKey, obj);
                }

                return obj;
            }

            public override string ToString()
            {
                return "Type: " + ImplementationType.FullName;
            }
        }

        private class InstanceRegistration : IRegistration
        {
            readonly object Instance;

            public InstanceRegistration(object instance)
            {
                Instance = instance;
            }

            public object Resolve(ObjectContainer container, RegistrationKey keyToResolve, IEnumerable<Type> resolutionPath)
            {
                return Instance;
            }

            public override string ToString()
            {
                return "Instance: " + Instance;
            }
        }

        private class NamedInstanceDictionaryRegistration : IRegistration
        {
            public object Resolve(ObjectContainer container, RegistrationKey keyToResolve, IEnumerable<Type> resolutionPath)
            {
                var typeToResolve = keyToResolve.Type;
                Debug.Assert(typeToResolve.IsGenericType && typeToResolve.GetGenericTypeDefinition() == typeof(IDictionary<,>));

                var genericArguments = typeToResolve.GetGenericArguments();
                var keyType = genericArguments[0];
                var targetType = genericArguments[1];
                var result = (IDictionary)Activator.CreateInstance(typeof (Dictionary<,>).MakeGenericType(genericArguments));

                foreach (var namedRegistration in container.registrations.Where(r => r.Key.Name != null && r.Key.Type == targetType).Select(r => r.Key))
                {
                    var convertedKey = ChangeType(namedRegistration.Name, keyType);
                    Debug.Assert(convertedKey != null);
                    result.Add(convertedKey, container.Resolve(namedRegistration.Type, namedRegistration.Name));
                }

                return result;
            }

            private object ChangeType(string name, Type keyType)
            {
                if (keyType == typeof(string))
                    return name;

                if (keyType.IsEnum)
                    return Enum.Parse(keyType, name, true);

                return Convert.ChangeType(name, keyType, CultureInfo.CurrentCulture);
            }
        }

        private bool isDisposed = false;
        private readonly ObjectContainer baseContainer;
        private readonly Dictionary<RegistrationKey, IRegistration> registrations = new Dictionary<RegistrationKey, IRegistration>();
        private readonly Dictionary<RegistrationKey, object> resolvedObjects = new Dictionary<RegistrationKey, object>();
        private readonly Dictionary<RegistrationKey, object> objectPool = new Dictionary<RegistrationKey, object>();

        public ObjectContainer()
        {
            RegisterInstanceAs<IObjectContainer>(this);
        }

        public ObjectContainer(IObjectContainer baseContainer) : this()
        {
            if (baseContainer != null && !(baseContainer is ObjectContainer))
                throw new ArgumentException("Base container must be an ObjectContainer", "baseContainer");

            this.baseContainer = (ObjectContainer)baseContainer;
        }

        #region Registration

        public void RegisterTypeAs<TInterface>(Type implementationType, string name = null) where TInterface : class
        {
            Type interfaceType = typeof(TInterface);
            RegisterTypeAs(implementationType, interfaceType, name);
        }

        public void RegisterTypeAs<TType, TInterface>(string name = null) where TType : class, TInterface
        {
            Type interfaceType = typeof(TInterface);
            Type implementationType = typeof(TType);
            RegisterTypeAs(implementationType, interfaceType, name);
        }

        private RegistrationKey CreateNamedInstanceDictionaryKey(Type targetType)
        {
            return new RegistrationKey(typeof(IDictionary<,>).MakeGenericType(typeof(string), targetType), null);
        }

        private void AddRegistration(RegistrationKey key, IRegistration registration)
        {
            registrations[key] = registration;

            if (key.Name != null)
            {
                var dictKey = CreateNamedInstanceDictionaryKey(key.Type);
                if (!registrations.ContainsKey(dictKey))
                {
                    registrations[dictKey] = new NamedInstanceDictionaryRegistration();
                }
            }
        }

        private void RegisterTypeAs(Type implementationType, Type interfaceType, string name)
        {
            var registrationKey = new RegistrationKey(interfaceType, name);
            AssertNotResolved(registrationKey);

            ClearRegistrations(registrationKey);

            AddRegistration(registrationKey, new TypeRegistration(implementationType));
        }

        public void RegisterInstanceAs(object instance, Type interfaceType, string name = null)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            var registrationKey = new RegistrationKey(interfaceType, name);
            AssertNotResolved(registrationKey);

            ClearRegistrations(registrationKey);
            AddRegistration(registrationKey, new InstanceRegistration(instance));
            objectPool[new RegistrationKey(instance.GetType(), name)] = instance;
        }

        public void RegisterInstanceAs<TInterface>(TInterface instance, string name = null) where TInterface : class
        {
            RegisterInstanceAs(instance, typeof(TInterface), name);
        }

        private void AssertNotResolved(RegistrationKey interfaceType)
        {
            if (resolvedObjects.ContainsKey(interfaceType))
                throw new ObjectContainerException("An object have been resolved for this interface already.", null);
        }

        private void ClearRegistrations(RegistrationKey registrationKey)
        {
            registrations.Remove(registrationKey);
        }

#if !BODI_LIMITEDRUNTIME
        public void RegisterFromConfiguration()
        {
            var section = (BoDiConfigurationSection)ConfigurationManager.GetSection("boDi");
            if (section == null)
                return;

            RegisterFromConfiguration(section.Registrations);
        }

        public void RegisterFromConfiguration(ContainerRegistrationCollection containerRegistrationCollection)
        {
            if (containerRegistrationCollection == null)
                return;

            foreach (ContainerRegistrationConfigElement registrationConfigElement in containerRegistrationCollection)
            {
                RegisterFromConfiguration(registrationConfigElement);
            }
        }

        private void RegisterFromConfiguration(ContainerRegistrationConfigElement registrationConfigElement)
        {
            Type interfaceType = Type.GetType(registrationConfigElement.Interface, true);
            Type implementationType = Type.GetType(registrationConfigElement.Implementation, true);

            RegisterTypeAs(implementationType, interfaceType, string.IsNullOrEmpty(registrationConfigElement.Name) ? null : registrationConfigElement.Name);
        }
#endif

        #endregion

        #region Resolve

        public T Resolve<T>()
        {
            return Resolve<T>(null);
        }

        public T Resolve<T>(string name)
        {
            Type typeToResolve = typeof(T);

            object resolvedObject = Resolve(typeToResolve, name);

            return (T)resolvedObject;
        }

        public object Resolve(Type typeToResolve, string name = null)
        {
            return Resolve(typeToResolve, Enumerable.Empty<Type>(), name);
        }

        private object Resolve(Type typeToResolve, IEnumerable<Type> resolutionPath, string name)
        {
            AssertNotDisposed();

            var keyToResolve = new RegistrationKey(typeToResolve, name);
            object resolvedObject;
            if (!resolvedObjects.TryGetValue(keyToResolve, out resolvedObject))
            {
                resolvedObject = CreateObjectFor(keyToResolve, resolutionPath);
                resolvedObjects.Add(keyToResolve, resolvedObject);
            }
            Debug.Assert(typeToResolve.IsInstanceOfType(resolvedObject));
            return resolvedObject;
        }

        private IRegistration GetRegistrationResult(RegistrationKey keyToResolve)
        {
            IRegistration registration;
            if (registrations.TryGetValue(keyToResolve, out registration))
            {
                return registration;
            }

            if (baseContainer != null)
                return baseContainer.GetRegistrationResult(keyToResolve);

            if (IsSpecialNamedInstanceDictionaryKey(keyToResolve))
            {
                var targetType = keyToResolve.Type.GetGenericArguments()[1];
                return GetRegistrationResult(CreateNamedInstanceDictionaryKey(targetType));
            }

            if(IsNamedInstanceDictionaryKey(keyToResolve))
            {
                return new NamedInstanceDictionaryRegistration();
            }

            return null;
        }

        private bool IsSpecialNamedInstanceDictionaryKey(RegistrationKey keyToResolve)
        {
            return IsNamedInstanceDictionaryKey(keyToResolve) && 
                   keyToResolve.Type.GetGenericArguments()[0] != typeof(string);
        }

        private bool IsNamedInstanceDictionaryKey(RegistrationKey keyToResolve)
        {
            return keyToResolve.Name == null && keyToResolve.Type.IsGenericType && keyToResolve.Type.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }

        private object GetPooledObject(RegistrationKey pooledObjectKey)
        {
            object obj;
            if (objectPool.TryGetValue(pooledObjectKey, out obj))
                return obj;

            if (baseContainer != null)
                return baseContainer.GetPooledObject(pooledObjectKey);

            return null;
        }

        private object CreateObjectFor(RegistrationKey keyToResolve, IEnumerable<Type> resolutionPath)
        {
            if (keyToResolve.Type.IsPrimitive || keyToResolve.Type == typeof(string))
                throw new ObjectContainerException("Primitive types cannot be resolved: " + keyToResolve.Type.FullName, resolutionPath);

            var registrationResult = GetRegistrationResult(keyToResolve) ?? new TypeRegistration(keyToResolve.Type);

            return registrationResult.Resolve(this, keyToResolve, resolutionPath);
        }

        private object CreateObject(Type type, IEnumerable<Type> resolutionPath, RegistrationKey keyToResolve)
        {
            var ctors = type.GetConstructors();
            if (ctors.Length == 0)
                ctors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

            if (ctors.Length == 0)
            {
                throw new ObjectContainerException("Class must have a constructor! " + type.FullName, resolutionPath);
            }

            int maxParamCount = ctors.Max(ctor => ctor.GetParameters().Length);
            var maxParamCountCtors = ctors.Where(ctor => ctor.GetParameters().Length == maxParamCount).ToArray();

            object obj;
            if (maxParamCountCtors.Length == 1)
            {
                ConstructorInfo ctor = maxParamCountCtors[0];
                if (resolutionPath.Contains(type))
                    throw new ObjectContainerException("Circular dependency found! " + type.FullName, resolutionPath);

                var args = ResolveArguments(ctor.GetParameters(), keyToResolve, resolutionPath.Concat(new[] { type }));
                obj = ctor.Invoke(args);
            }
            else
            {
                throw new ObjectContainerException("Multiple public constructors with same maximum parameter count are not supported! " + type.FullName, resolutionPath);
            }

            return obj;
        }

        private object[] ResolveArguments(IEnumerable<ParameterInfo> parameters, RegistrationKey keyToResolve, IEnumerable<Type> resolutionPath)
        {
            return parameters.Select(p => IsRegisteredNameParameter(p) ? ResolveRegisteredName(keyToResolve) : Resolve(p.ParameterType, resolutionPath, null)).ToArray();
        }

        private object ResolveRegisteredName(RegistrationKey keyToResolve)
        {
            return keyToResolve.Name;
        }

        private bool IsRegisteredNameParameter(ParameterInfo parameterInfo)
        {
            return parameterInfo.ParameterType == typeof (string) &&
                   parameterInfo.Name.Equals(REGISTERED_NAME_PARAMETER_NAME);
        }

        #endregion

        private void AssertNotDisposed()
        {
            if (isDisposed)
                throw new ObjectContainerException("Object container disposed", null);
        }

        public void Dispose()
        {
            isDisposed = true;

            foreach (var obj in objectPool.Values.OfType<IDisposable>().Where(o => !ReferenceEquals(o, this)))
                obj.Dispose();

            objectPool.Clear();
            registrations.Clear();
            resolvedObjects.Clear();
        }
    }

    #region Configuration handling
#if !BODI_LIMITEDRUNTIME

    public class BoDiConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        [ConfigurationCollection(typeof(ContainerRegistrationCollection), AddItemName = "register")]
        public ContainerRegistrationCollection Registrations
        {
            get { return (ContainerRegistrationCollection)this[""]; }
            set { this[""] = value; }
        }
    }

    public class ContainerRegistrationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ContainerRegistrationConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var registrationConfigElement = ((ContainerRegistrationConfigElement)element);
            string elementKey = registrationConfigElement.Interface;
            if (registrationConfigElement.Name != null)
                elementKey = elementKey + "/" + registrationConfigElement.Name;
            return elementKey;
        }

        public void Add(string implementationType, string interfaceType, string name = null)
        {
            BaseAdd(new ContainerRegistrationConfigElement
                        {
                            Implementation = implementationType,
                            Interface = interfaceType,
                            Name = name
                        });
        }
    }

    public class ContainerRegistrationConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("as", IsRequired = true)]
        public string Interface
        {
            get { return (string)this["as"]; }
            set { this["as"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Implementation
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = false, DefaultValue = null)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
    }

#endif
    #endregion
}