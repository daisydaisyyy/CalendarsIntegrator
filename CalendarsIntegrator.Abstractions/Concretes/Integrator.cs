using CalendarsIntegrator.Core.Abstracts;

namespace CalendarsIntegrator.Core.Concretes
{

    /// <summary>
    /// Implement integration
    /// </summary>
    public class Integrator : IIntegrator
    {
        private IEnumerable<ISink> _InputSinks;
        private IEnumerable<ISink> _OutputSinks;
        private ISearch _search;
        public IEnumerable<ISink> InputSinks => _InputSinks;
        public IEnumerable<ISink> OutputSinks => _OutputSinks;
        public Integrator(IEnumerable<ISink> input, IEnumerable<ISink> output, ISearch search)
        {
            _InputSinks = input;
            _OutputSinks = output;
            _search = search;
        }

        public async Task Sync()
        {
            await LoadSinks();

            await AddUnexisting();

            await RemoveUnexisting();

            foreach (var sink in OutputSinks)
            {
                await sink.Load(_search);
                
            }

        }

        private async Task LoadSinks()
        {
            foreach (var sink in InputSinks)
            {
                await sink.Load(_search);
            }

            foreach (var sink in OutputSinks)
            {
                await sink.Load(_search);
            }
        }

        private async Task AddUnexisting()
        {
            foreach (var inputSink in InputSinks)
            {
                inputSink.GetEntries().Result.Count();
                foreach (var entry in await inputSink.GetEntries())
                {

                    foreach (var outputSink in OutputSinks)
                    {
                       if (!await outputSink.Exists(entry))
                         await outputSink.Insert(entry);
                    }
                }
            }
        }

        private async Task RemoveUnexisting()
        {
            foreach (var outputSink in OutputSinks)
            {
                foreach (var entry in await outputSink.GetEntries())
                {
                    bool found = false;

                    foreach (var inputSink in InputSinks)
                    {
                        if (await inputSink.Exists(entry))
                        {
                            found = true;
                            break;
                        }
                    }

                    if(!found)
                        await outputSink.Delete(entry);

                  
                }
                outputSink.GetEntries().Result.Count();
            }
            
        }
    }
}
