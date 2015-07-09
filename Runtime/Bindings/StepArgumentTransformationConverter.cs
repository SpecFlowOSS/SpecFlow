namespace TechTalk.SpecFlow.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using TechTalk.SpecFlow.Bindings.Reflection;
    using TechTalk.SpecFlow.Infrastructure;
    using TechTalk.SpecFlow.Tracing;

    internal class StepArgumentTransformationConverter : IStepArgumentTypeConverter
    {
        private readonly IStepArgumentTypeConverter stepArgumentTypeConverter;
        private readonly ITestTracer testTracer;
        private readonly IBindingRegistry bindingRegistry;
        private readonly IContextManager contextManager;
        private readonly IBindingInvoker bindingInvoker;

        public StepArgumentTransformationConverter(IStepArgumentTypeConverter stepArgumentTypeConverter, ITestTracer testTracer, IBindingRegistry bindingRegistry, IContextManager contextManager, IBindingInvoker bindingInvoker)
        {
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;
            this.testTracer = testTracer;
            this.bindingRegistry = bindingRegistry;
            this.contextManager = contextManager;
            this.bindingInvoker = bindingInvoker;
        }

        public object Convert(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var stepTransformation = GetMatchingStepTransformation(values, typeToConvertTo, cultureInfo, true);

            if (stepTransformation == null)
                throw new SpecFlowException("The StepTransformationConverter cannot convert the specified value.");

            if (stepTransformation.Method.Parameters.Count() > 1  && values.Count>1)
            {
                return DoMultipleValueTransform(stepTransformation, values, cultureInfo);
            }
            return DoTransform(stepTransformation, values.Dequeue(), cultureInfo);
        }

        public bool CanConvert(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            return GetMatchingStepTransformation(values, typeToConvertTo, cultureInfo, false) != null;
        }

        private IStepArgumentTransformationBinding GetMatchingStepTransformation(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo, bool traceWarning)
        {
            var stepTransformations = bindingRegistry.GetStepTransformations().Where(t => CanConvert(t, values, typeToConvertTo, cultureInfo)).ToArray();

            if (stepTransformations.Length > 1 && traceWarning)
            {
                testTracer.TraceWarning(string.Format("Multiple step transformation matches to the input ({0}, target type: {1}). We use the first.", values.Peek(), typeToConvertTo));
            }

            return stepTransformations.FirstOrDefault();
        }

        private bool CanConvert(IStepArgumentTransformationBinding stepTransformationBinding, Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            if (!stepTransformationBinding.Method.ReturnType.TypeEquals(typeToConvertTo))
                return false;

            if (stepTransformationBinding.Regex != null && !(values.Peek() is string))
            {
                return false;
            }

            object[] arguments;
            if (stepTransformationBinding.Method.Parameters.Count() > 1 && values.Count > 1)
            {
                arguments = values.ToArray();
            }
            else
            {
                arguments = GetStepTransformationArguments(stepTransformationBinding, values.Peek(), cultureInfo);
            }
            
            var parameters = stepTransformationBinding.Method.Parameters.ToArray();

            if (arguments == null || arguments.Length < parameters.Length)
                return false;

            for (var i = 0; i < parameters.Length; i++)
            {
                if (!stepArgumentTypeConverter.CanConvert(CreateQueue(arguments[i]), parameters[i].Type, cultureInfo))
                    return false;
            }

            return true;
        }

        private object DoMultipleValueTransform(IStepArgumentTransformationBinding stepTransformation, Queue<object> remainingValues, CultureInfo cultureInfo)
        {
            var arguments = new List<object>();
            foreach (IBindingParameter param in stepTransformation.Method.Parameters)
            {
                arguments.Add(stepArgumentTypeConverter.Convert(remainingValues, param.Type, cultureInfo));
            }
            TimeSpan duration;
            return bindingInvoker.InvokeBinding(stepTransformation, contextManager, arguments.ToArray(), testTracer, out duration);
        }

        private object DoTransform(IStepArgumentTransformationBinding stepTransformationBinding, object value, CultureInfo cultureInfo)
        {
            var parameters = stepTransformationBinding.Method.Parameters.ToArray();
            var arguments = GetStepTransformationArguments(stepTransformationBinding, value, cultureInfo)
                .Select((arg, i) => stepArgumentTypeConverter.Convert(CreateQueue(arg), parameters[i].Type, cultureInfo)).ToArray();

            TimeSpan duration;
            return bindingInvoker.InvokeBinding(stepTransformationBinding, contextManager, arguments, testTracer, out duration);
        }

        private object[] GetStepTransformationArguments(IStepArgumentTransformationBinding stepTransformation, object value, CultureInfo cultureInfo)
        {
            if (stepTransformation.Regex != null && value is string)
                return GetStepTransformationArgumentsFromRegex(stepTransformation, (string)value, cultureInfo);

            return new[] { value };
        }

        private object[] GetStepTransformationArgumentsFromRegex(IStepArgumentTransformationBinding stepTransformation, string stepSnippet, CultureInfo cultureInfo)
        {
            var match = stepTransformation.Regex.Match(stepSnippet);

            if (!match.Success)
            {
                return null;
            }

            return match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).Cast<object>().ToArray();
        }

        private static Queue<object> CreateQueue(object value)
        {
            Queue<Object> queue = new Queue<object>();
            queue.Enqueue(value);
            return queue;
        }
    }

    //    public class StepArgumentTypeConverter : IStepArgumentTypeConverter
    //    {
    //        private readonly ITestTracer testTracer;
    //        private readonly IBindingRegistry bindingRegistry;
    //        private readonly IContextManager contextManager;
    //        private readonly IBindingInvoker bindingInvoker;
    //
    //        public StepArgumentTypeConverter(ITestTracer testTracer, IBindingRegistry bindingRegistry, IContextManager contextManager, IBindingInvoker bindingInvoker)
    //        {
    //            this.testTracer = testTracer;
    //            this.bindingRegistry = bindingRegistry;
    //            this.contextManager = contextManager;
    //            this.bindingInvoker = bindingInvoker;
    //        }
    //
    //        protected virtual IStepArgumentTransformationBinding GetMatchingStepTransformation(object value, IBindingType typeToConvertTo, bool traceWarning, CultureInfo cultureInfo)
    //        {
    //            var stepTransformations = bindingRegistry.GetStepTransformations().Where(t => CanConvert(t, value, typeToConvertTo,cultureInfo)).ToArray();
    //            if (stepTransformations.Length > 1 && traceWarning)
    //            {
    //                testTracer.TraceWarning(string.Format("Multiple step transformation matches to the input ({0}, target type: {1}). We use the first.", value, typeToConvertTo));
    //            }
    //
    //            return stepTransformations.Length > 0 ? stepTransformations[0] : null;
    //        }
    //
    //        private object Convert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
    //        {
    //            if (value == null) throw new ArgumentNullException("value");
    //
    //            if (typeToConvertTo == value.GetType())
    //                return value;
    //
    //            var stepTransformation = GetMatchingStepTransformation(value, typeToConvertTo, true,cultureInfo);
    //            if (stepTransformation != null)
    //                return DoTransform(stepTransformation, value, cultureInfo);
    //
    //            return ConvertSimple(typeToConvertTo, value, cultureInfo);
    //        }
    //
    //        public object Convert(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo)
    //        {
    //            if (values == null)
    //            {
    //                throw new ArgumentNullException("value");
    //            }
    //            object value = values.Peek();
    //            if (typeToConvertTo == value.GetType())
    //            {
    //                return values.Dequeue();
    //            }
    //
    //            IStepArgumentTransformationBinding stepTransformation = GetMatchingStepTransformation(value, typeToConvertTo, true,cultureInfo);
    //            if (stepTransformation != null)
    //            {
    //                if (stepTransformation.Method.Parameters.Count() > 1)
    //                {
    //                    return DoMultipleValueTransform(stepTransformation, values, cultureInfo);
    //                }
    //                return DoTransform(stepTransformation, values.Dequeue(), cultureInfo);
    //            }
    //
    //            return ConvertSimple(typeToConvertTo, values.Dequeue(), cultureInfo);
    //        }
    //
    //        private object DoMultipleValueTransform(IStepArgumentTransformationBinding stepTransformation, Queue<object> remainingValues, CultureInfo cultureInfo)
    //        {
    //            var arguments = new List<object>();
    //            foreach (IBindingParameter param in stepTransformation.Method.Parameters)
    //            {
    //                arguments.Add(ConvertSimple(param.Type, remainingValues.Dequeue(), cultureInfo));
    //            }
    //            TimeSpan duration;
    //            return bindingInvoker.InvokeBinding(stepTransformation, contextManager, arguments.ToArray(), testTracer, out duration);
    //        }
    //
    //        private object DoTransform(IStepArgumentTransformationBinding stepTransformation, object value, CultureInfo cultureInfo)
    //        {
    //            object[] arguments;
    //            if (stepTransformation.Regex != null && value is string)
    //                arguments = GetStepTransformationArgumentsFromRegex(stepTransformation, (string)value, cultureInfo);
    //            else
    //                arguments = new object[] { value };
    //
    //            TimeSpan duration;
    //            return bindingInvoker.InvokeBinding(stepTransformation, contextManager, arguments, testTracer, out duration);
    //        }
    //
    ////        private object[] GetStepTransformationArgumentsFromRegex(IStepArgumentTransformationBinding stepTransformation, string stepSnippet, CultureInfo cultureInfo)
    ////        {
    ////            var match = stepTransformation.Regex.Match(stepSnippet);
    ////            var argumentStrings = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value);
    ////            var bindingParameters = stepTransformation.Method.Parameters.ToArray();
    ////            return argumentStrings
    ////                .Select((arg, argIndex) => this.Convert(arg, bindingParameters[argIndex].Type, cultureInfo))
    ////                .ToArray();
    ////        }
    //
    //        public bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
    //        {
    //            if (value == null) throw new ArgumentNullException("value");
    //
    //            if (typeToConvertTo == value.GetType())
    //                return true;
    //
    //            var stepTransformation = GetMatchingStepTransformation(value, typeToConvertTo, false,cultureInfo);
    //            if (stepTransformation != null)
    //                return true;
    //
    //            return CanConvertSimple(typeToConvertTo, value, cultureInfo);
    //        }
    //
    //        private bool CanConvert(IStepArgumentTransformationBinding stepTransformationBinding, object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
    //        {
    //            if (!stepTransformationBinding.Method.ReturnType.TypeEquals(typeToConvertTo))
    //                return false;
    //
    //            if (stepTransformationBinding.Regex != null && !(value is string))
    //            {
    //                return false;
    //            }
    //
    //            var arguments = GetStepTransformationArguments(stepTransformationBinding, value, cultureInfo);
    //            var parameters = stepTransformationBinding.Method.Parameters.ToArray();
    //
    //            if (arguments == null || arguments.Length != parameters.Length)
    //                return false;
    //
    //            for (var i = 0; i < parameters.Length; i++)
    //            {
    //                if (!CanConvert(arguments[i], parameters[i].Type, cultureInfo))
    //                    return false;
    //            }
    //
    //            return true;
    //        }
    //
    //        private static object ConvertSimple(IBindingType typeToConvertTo, object value, CultureInfo cultureInfo)
    //        {
    //            if (!(typeToConvertTo is RuntimeBindingType))
    //                throw new SpecFlowException("The StepArgumentTypeConverter can be used with runtime types only.");
    //
    //            return ConvertSimple(((RuntimeBindingType)typeToConvertTo).Type, value, cultureInfo);
    //        }
    //
    //        private static object ConvertSimple(Type typeToConvertTo, object value, CultureInfo cultureInfo)
    //        {
    //            if (typeToConvertTo.IsEnum && value is string)
    //                return Enum.Parse(typeToConvertTo, (string)value, true);
    //
    //            if (typeToConvertTo == typeof(Guid?) && string.IsNullOrEmpty(value as string))
    //                return null;
    //
    //            if (typeToConvertTo == typeof(Guid) || typeToConvertTo == typeof(Guid?))
    //                return new GuidValueRetriever().GetValue(value as string);
    //
    //            return System.Convert.ChangeType(value, typeToConvertTo, cultureInfo);
    //        }
    //
    //        public static bool CanConvertSimple(IBindingType typeToConvertTo, object value, CultureInfo cultureInfo)
    //        {
    //
    //            try
    //            {
    //                ConvertSimple(typeToConvertTo, value, cultureInfo);
    //                return true;
    //            }
    //            catch (InvalidCastException)
    //            {
    //                return false;
    //            }
    //            catch (OverflowException)
    //            {
    //                return false;
    //            }
    //            catch (FormatException)
    //            {
    //                return false;
    //            }
    //            catch (ArgumentException)
    //            {
    //                return false;
    //            }
    //        }
    //
    //        private object[] GetStepTransformationArguments(IStepArgumentTransformationBinding stepTransformation, object value, CultureInfo cultureInfo)
    //        {
    //            if (stepTransformation.Regex != null && value is string)
    //                return GetStepTransformationArgumentsFromRegex(stepTransformation, (string)value, cultureInfo);
    //
    //            return new[] { value };
    //        }
    //
    //        private object[] GetStepTransformationArgumentsFromRegex(IStepArgumentTransformationBinding stepTransformation, string stepSnippet, CultureInfo cultureInfo)
    //        {
    //            var match = stepTransformation.Regex.Match(stepSnippet);
    //
    //            if (!match.Success)
    //            {
    //                return null;
    //            }
    //
    //            return match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).Cast<object>().ToArray();
    //        }
    //    }
}