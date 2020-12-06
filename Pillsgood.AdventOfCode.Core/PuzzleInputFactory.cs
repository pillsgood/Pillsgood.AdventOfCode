using Pillsgood.AdventOfCode.Abstractions;

namespace Pillsgood.AdventOfCode.Core
{
    internal class PuzzleInputFactory
    {
        private readonly PuzzleDataManager _dataManager;
        private readonly IAocClient _client;

        public PuzzleInputFactory(PuzzleDataManager dataManager, IAocClient client = null)
        {
            _dataManager = dataManager;
            _client = client;
        }

        public IPuzzleInput Create(PuzzleMetadata metadata)
        {
            return new PuzzleInput(() =>
            {
                var data = _dataManager.Get(metadata);
                if (string.IsNullOrEmpty(data.Input))
                {
                    data.Input = _client?.GetPuzzleInput(data).Result;
                }

                return data.Input;
            });
        }
    }
}