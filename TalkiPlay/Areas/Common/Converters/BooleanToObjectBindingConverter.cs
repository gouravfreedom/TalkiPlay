using System;
using ReactiveUI;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay
{
    public class BooleanToObjectBindingConverter<T> : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return fromType == typeof(bool) ? 100 : 0;
        }

        public bool TryConvert(object fromValue, Type toType, object conversionHint, out object result)
        {
            var logger = Locator.Current.GetService<ILogger>();
            try
            {

                if (fromValue is bool booleanValue)
                {
                    result = booleanValue ? TrueObject : FalseObject;
                }
                else
                {
                    result = FalseObject;
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = FalseObject;
                return false;
            }

            return true;
        }
        
        public T TrueObject { get; set; }

        /// <summary>
        /// The object to return when the source boolean value is false
        /// </summary>
        public T FalseObject { get; set; }
    }
}