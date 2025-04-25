using System.Collections;
using System.Collections.Generic;

namespace GameEngine.Pipeline
{
    public interface IPipelineTask<IPayLoad>
    {
        IPayLoad PayLoad { get; set; }
        IEnumerator Process();
    }

    public class PipelineRunner<IPayLoad> 
    {
        private bool isRunning = false;

        public void Run(IEnumerable<IPipelineTask<IPayLoad>> tasks, IPayLoad payLoad)
        {
            var enumerator = GetEnumerator(tasks, payLoad);
            while (enumerator.MoveNext()) { }
        }

        private IEnumerator GetEnumerator(IEnumerable<IPipelineTask<IPayLoad>> tasks, IPayLoad payLoad)
        {
            if (isRunning)
            {
                UnityEngine.Debug.LogError("Pipeline already running!!");
                yield break;
            }

            isRunning = true;

            yield return null;

            var enumerator = GetEnumeratorNoErrorHandling(tasks, payLoad);

            while (true)
            {
                try
                {
                    bool canNext = enumerator.MoveNext();
                    if (canNext == false)
                        break;
                }catch
                {
                    isRunning = false;
                    throw;
                }

                yield return null;
            }

            isRunning = false;
        }

        private IEnumerator GetEnumeratorNoErrorHandling(IEnumerable<IPipelineTask<IPayLoad>> tasks, IPayLoad payLoad)
        {
            yield return null;

            foreach (var task in tasks)
            {
                task.PayLoad = payLoad;
                var taskEnumerator = task.Process();

                yield return null;

                while (taskEnumerator.MoveNext())
                {
                    yield return null;
                }
            }
        }
    }
}