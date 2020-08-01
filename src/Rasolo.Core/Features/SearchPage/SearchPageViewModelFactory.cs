﻿using System;
using System.Linq;
using System.Web.UI.WebControls;
using Examine;
using Examine.Search;
using Rasolo.Core.Features.SearchPage.Examine;
using Rasolo.Core.Features.Shared.Abstractions;
using Rasolo.Core.Features.Shared.Compositions;
using Rasolo.Core.Features.Shared.Constants;
using Rasolo.Core.Features.Shared.Constants.PropertyTypeAlias;
using Rasolo.Core.Features.Shared.GlobalSettings;
using Rasolo.Services.Abstractions.HttpRequest;
using Rasolo.Services.Abstractions.UmbracoHelper;
using Umbraco.Core;
using Umbraco.Examine;
using Umbraco.Web.Models;
using Zone.UmbracoMapper.V8;

namespace Rasolo.Core.Features.SearchPage
{
	public class SearchPageViewModelFactory : BaseContentPageViewModelFactory<SearchPage>, ISearchPageViewModelFactory
	{
		private readonly IUmbracoMapper _umbracoMapper;
		private readonly IUmbracoHelper _umbracoHelper;
		private readonly IHttpUtility _httpUtility;
		private readonly IHttpRequest _httpRequest;
		private readonly IExamineManager _examineManager;
		private readonly GlobalSettingsPageViewModel _globalSettingsPageViewModel;

		public SearchPageViewModelFactory(IUmbracoMapper umbracoMapper, IUmbracoHelper umbracoHelper, IHttpUtility httpUtility, IHttpRequest httpRequest,
			IExamineManager examineManager, IGlobalSettingsPageViewModelFactory globalSettingsPageViewModelFactory)
		 : base(umbracoMapper, umbracoHelper)
		{
			_umbracoMapper = umbracoMapper;
			_umbracoHelper = umbracoHelper;
			_httpUtility = httpUtility;
			_httpRequest = httpRequest;
			_examineManager = examineManager;
			_globalSettingsPageViewModel = globalSettingsPageViewModelFactory.CreateModel(null);
		}
		public override SearchPage CreateModel(SearchPage viewModel, ContentModel contentModel)
		{
			viewModel = base.CreateModel(viewModel, contentModel);

			return viewModel;
		}

		public override void SetViewModelProperties(SearchPage viewModel, ContentModel contentModel)
		{
			base.SetViewModelProperties(viewModel, contentModel);

			viewModel.Query = this._httpUtility.UrlDecode(this._httpRequest.QueryString[QueryStrings.SearchQuery]);
			viewModel.CurrentPaginationPageNumber = int.Parse(string.IsNullOrEmpty(this._httpRequest.QueryString[QueryStrings.Pagination]) ? "1" : this._httpRequest.QueryString[QueryStrings.Pagination]);
			

			if (!string.IsNullOrEmpty(viewModel.Query))
			{
				Search(viewModel);
			}


			viewModel.NumberOfPages = (int)Math.Round(Convert.ToDecimal((double)viewModel.TotalItems / this._globalSettingsPageViewModel.SearchResultsPerPage), MidpointRounding.AwayFromZero);
			viewModel.ShowPagination = viewModel.NumberOfPages >= 2;
			viewModel.PaginationSearchQuery = $"{viewModel.Url}?{QueryStrings.SearchQuery}={viewModel.Query}&{QueryStrings.Pagination}=";
			viewModel.ShowNextPagePaginationSymbol = viewModel.CurrentPaginationPageNumber < viewModel.NumberOfPages;
			viewModel.ShowPreviousPagePaginationSymbol = viewModel.CurrentPaginationPageNumber > 1;
			viewModel.NextPaginationPageUrl = $"{viewModel.PaginationSearchQuery}{viewModel.CurrentPaginationPageNumber + 1}";
			viewModel.PreviousPaginationPageUrl = $"{viewModel.PaginationSearchQuery}{viewModel.CurrentPaginationPageNumber + -1}";
			viewModel.ShowSearchResults = !string.IsNullOrEmpty(viewModel.Query);
		}

		public void Search(SearchPage viewModel)
		{
			if (!this._examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex index))
			{
				throw new InvalidOperationException($"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}");
			}

			var searcher = index.GetSearcher();
			var query = searcher.CreateQuery(IndexTypes.Content);
			var operation = query.GroupedOr(new[] { "__NodeTypeAlias" }, new []{DocumentTypeAlias.BlogPostPage});
			var searchTerms = viewModel.Query.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
			var exactSearchPhrase = new ExactPhraseExamineValue(viewModel.Query);
			foreach (string searchTerm in searchTerms)
			{
				{
					operation.And().GroupedOr(new[] { PropertyTypeAlias.Title, PropertyTypeAlias.Preamble }, new[] { searchTerm.Fuzzy(0.4f), exactSearchPhrase });
				}
			}

			var searchResults = operation.Execute();
			var searchResultItems = searchResults.Select(MapViewModels);

			viewModel.Results = searchResultItems.Skip((viewModel.CurrentPaginationPageNumber -1) * this._globalSettingsPageViewModel.SearchResultsPerPage).Take(this._globalSettingsPageViewModel.SearchResultsPerPage).ToList();
			viewModel.TotalItems = searchResultItems.Count();

		}

		private SearchResultItem MapViewModels(ISearchResult result)
		{
			if (result == null) return null;
			var content = this._umbracoHelper.Content(result.Id);

			if (content == null) return null;
			var viewModel = new SearchResultItem();

			this._umbracoMapper.Map(content, viewModel);

			viewModel.ShowTeaserMediaAltText = !string.IsNullOrEmpty(viewModel.TeaserMediaAltText);

			return viewModel;
		}
	}
}