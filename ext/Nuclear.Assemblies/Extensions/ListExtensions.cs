using System;
using System.Collections.Generic;
using Nuclear.Exceptions;

namespace Nuclear.Assemblies.Extensions {
    internal static class ListExtensions {

        internal static Boolean TryTake<T>(this List<T> @this, Predicate<T> match, out T element) {
            Throw.If.Object.IsNull(@this, nameof(@this));
            Throw.If.Object.IsNull(match, nameof(match));

            element = default;

            if(@this.Exists(match)) {
                element = @this.Find(match);
                @this.Remove(element);
            }

            return element != null;
        }

    }
}
