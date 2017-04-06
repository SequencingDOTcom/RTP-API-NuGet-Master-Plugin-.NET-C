using System;

namespace Sequencing.MasterPluginSample.Web.Models
{
    /// <summary>
    /// Personalized forecast results page model
    /// </summary>
    public class RunResult : CommonData
    {
        public string Weather { get; set; }
        public string Risk { get; set; }
        public string RawRisk { get; set; }
        public DateTime JobDateTime { get; set; }
    }
}