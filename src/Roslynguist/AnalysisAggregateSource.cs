namespace Roslynguist
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;

    public class AnalysisAggregateSource
    {
        private readonly Dictionary<SolutionId, List<Compilation>> _projectCompilations = new Dictionary<SolutionId, List<Compilation>>();
        private readonly Dictionary<string, Solution> _loadedSolutions = new Dictionary<string, Solution>();

        public async Task<bool> LoadSolution(string solutionPath)
        {
            if (_loadedSolutions.ContainsKey(solutionPath)) return false;

            var loader = new SolutionLoader();
            loader.AddSolutionToLoad(solutionPath);
            var solution = await loader.Load();

            foreach (var kvp in solution)
            {
                _loadedSolutions.Add(kvp.Key, kvp.Value);
                ConcurrentBag<Compilation> compilations = new ConcurrentBag<Compilation>();
                foreach (var proj in kvp.Value.Projects.AsParallel())
                {
                    var compilation = await proj.GetCompilationAsync();
                    compilations.Add(compilation);
                }
                _projectCompilations.Add(kvp.Value.Id, compilations.ToList());
            }

            return true;
        }

        public Dictionary<SolutionId, List<Compilation>> GetCompilationsPerSolution()
        {
            return new Dictionary<SolutionId, List<Compilation>>(_projectCompilations);
        } 
    }
}
