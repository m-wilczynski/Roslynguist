namespace Roslynguist
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.MSBuild;

    public class SolutionLoader
    {
        private readonly HashSet<string> _solutionsToLoad;

        public SolutionLoader()
        {
            _solutionsToLoad = new HashSet<string>();
        }

        public bool AlreadyLoaded { get; private set; }
        public bool IsRunning { get; private set; }

        public void AddSolutionToLoad(string solutionPath)
        {
            if (solutionPath == null)
                throw new ArgumentNullException(nameof(solutionPath));
            _solutionsToLoad.Add(solutionPath);
        }

        public void AddSolutionsToLoad(ICollection<string> solutionsPaths)
        {
            if (solutionsPaths == null)
                throw new ArgumentNullException(nameof(solutionsPaths));
            foreach (var solutionPath in solutionsPaths.Where(path => !string.IsNullOrEmpty((path))))
            {
                AddSolutionToLoad(solutionPath);
            }
        }

        public async Task<Dictionary<string, Solution>> Load(bool ignoreFailed = true)
        {
            if (IsRunning || AlreadyLoaded) return null;
            IsRunning = true;

            var solutions = await LoadSolutions(ignoreFailed);

            AlreadyLoaded = true;
            IsRunning = false;
            return solutions;
        }

        private async Task<Dictionary<string, Solution>> LoadSolutions(bool ignoreFailed)
        {
            var workspace = MSBuildWorkspace.Create();
            var openedSolutions = new Dictionary<string, Solution>();

            foreach (var solution in _solutionsToLoad)
            {
                try
                {
                    openedSolutions.Add(solution, await workspace.OpenSolutionAsync(solution));
                }
                catch (Exception)
                {
                    if (!ignoreFailed) throw;
                }
            }

            return openedSolutions;
        }
    }
}
