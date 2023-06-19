namespace CalendarsIntegrator.Core.Abstracts
{

    /// <summary>
    /// Represent an integrator component
    /// </summary>
    public interface IIntegrator
    {

        /// <summary>
        /// Sources of calendar entries
        /// </summary>
        public IEnumerable<ISink> InputSinks { get; }

        /// <summary>
        /// Destinations of calendar entries
        /// </summary>
        public IEnumerable<ISink> OutputSinks { get; }

        /// <summary>
        /// Executes syncronization from inputs to outputs
        /// </summary>
        /// <returns></returns>
        public Task Sync();


    }
}