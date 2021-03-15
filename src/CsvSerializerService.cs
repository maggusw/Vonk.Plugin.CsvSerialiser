﻿using System.Threading.Tasks;
using Vonk.Core.Repository;
using Serilog;
using Vonk.Core.Context;
using Task = System.Threading.Tasks.Task;

namespace Vonk.Plugin.CsvSerializer
{
    public class CsvSerializerService
    {
        private readonly ISearchRepository _searchRepository;

        public CsvSerializerService(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }
        
        public async Task<SearchResult> GetSearchResult(IVonkContext context)
        {
            Log.Information("Handling Csv");
            _ = await Task.FromResult(true);
            var (_, args, _) = context.Parts();

            foreach (var arg in args)
            {
                if (arg.ArgumentValue.Contains("/$csv"))
                {
                    arg.ArgumentValue = arg.ArgumentValue.Remove
                        (arg.ArgumentValue.Length - 5);
                } 
            }
            
            
            // FIXME: double check this
            
    //        if (!response.Success()) throw new ArgumentException("Server returned 404. Please check query");
    
            var searchResult = await _searchRepository.Search(context.Arguments,
                SearchOptions.Latest(context.ServerBase, context.Request.Interaction, context.InformationModel));
            return searchResult;
            
        }
    }
}