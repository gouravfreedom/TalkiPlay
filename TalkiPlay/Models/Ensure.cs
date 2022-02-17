using System;
using System.Collections.Generic;
using System.Text;

namespace TalkiPlay.Shared
{
    public static class Ensure
    {
        public static void ArgumentNotNull(object source, string argumentName)
        {
            if (source == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
