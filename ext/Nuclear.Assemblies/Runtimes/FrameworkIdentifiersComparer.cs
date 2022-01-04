using System;
using System.Collections.Generic;

namespace Nuclear.Assemblies.Runtimes {

    internal class FrameworkIdentifiersComparer : IComparer<FrameworkIdentifiers> {

        public Int32 Compare(FrameworkIdentifiers x, FrameworkIdentifiers y) {
            if(!Enum.IsDefined(typeof(FrameworkIdentifiers), x) && !Enum.IsDefined(typeof(FrameworkIdentifiers), y)) {
                return 0;
            }

            if(x != y) {
                if(!Enum.IsDefined(typeof(FrameworkIdentifiers), x)) {
                    return -Compare(y, x);

                } else if(!Enum.IsDefined(typeof(FrameworkIdentifiers), y)) {
                    return 1;
                }
            }

            return x.CompareTo(y);
        }

    }

}
