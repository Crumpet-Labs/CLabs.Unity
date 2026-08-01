using System;

namespace CLabs.Crumb {
    /// <summary>An <see cref="ICrumbSink"/> that fans each log line out to several sinks in order — e.g. the Unity console plus a <see cref="BufferedCrumbSink"/> for an editor view.</summary>
    public sealed class CompositeCrumbSink : ICrumbSink {
        private readonly ICrumbSink[] m_Sinks;

        public CompositeCrumbSink(params ICrumbSink[] sinks) {
            m_Sinks = sinks ?? Array.Empty<ICrumbSink>();
        }

        public void Write(string level, string typeName, string message) {
            foreach (var sink in m_Sinks) {
                sink?.Write(level, typeName, message);
            }
        }
    }
}
