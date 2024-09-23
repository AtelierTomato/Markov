namespace AtelierTomato.Markov.Console
{
	public class WorkerManager
	{
		private readonly ILogger<WorkerManager> logger;
		private readonly object workerLock = new();
		private readonly List<int> availableWorkerIDs = new();

		public WorkerManager(ILogger<WorkerManager> logger)
		{
			this.logger = logger;

			for (int i = 1; i <= 1000; i++)
			{
				availableWorkerIDs.Add(i);
			}
		}

		public int RegisterWorkerID()
		{
			int workerID;
			lock (workerLock)
			{
				if (availableWorkerIDs.Count is 0)
				{
					throw new InvalidOperationException("No available worker IDs.");
				}

				// Find the smallest available worker ID
				workerID = availableWorkerIDs.Min();
				availableWorkerIDs.Remove(workerID);
				logger.LogInformation("Worker {WorkerID} assigned to command.", workerID);
			}
			return workerID;
		}

		public void ReleaseWorkerID(int workerID)
		{
			lock (workerLock)
			{
				availableWorkerIDs.Add(workerID);
				logger.LogInformation("Worker {WorkerID} released.", workerID);
			}
		}
	}
}
