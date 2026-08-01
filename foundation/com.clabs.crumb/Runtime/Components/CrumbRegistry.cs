using System;
using CLabs.Utility;

namespace CLabs.Crumb {
    /// <summary>Type-keyed lookup of the live <see cref="CrumbLogger"/>s, so tooling can toggle or re-filter logging per owning type.</summary>
    public sealed class CrumbRegistry : Registry<Type, CrumbLogger> { }
}
