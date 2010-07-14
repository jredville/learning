using System;

namespace Runner {
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class SampleAttribute : Attribute {
        readonly string sampleName;

        // This is a positional argument
        public SampleAttribute(string name) {
            this.sampleName = name;
            ClassSample = true;
        }

        public string SampleName {
            get { return sampleName; }
        }

        public bool ClassSample {
            get;
            set;
        }
    }
}
