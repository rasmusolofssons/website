﻿using System.Collections.Generic;
using System.Linq;
using Rasolo.Core.Features.Shared.Compositions;
using Rasolo.Core.Features.Shared.Constants;
using Rasolo.Services.Abstractions.UmbracoHelper;
using Rasolo.Services.Constants;
using Umbraco.Web.Models;
using Zone.UmbracoMapper.V8;

namespace Rasolo.Core.Features.BlogsPage
{
	public class BlogsPageViewModelFactory : BaseContentPageViewModelFactory<BlogsPage>, IBlogsPageViewModelFactory
	{
		private readonly IUmbracoMapper _umbracoMapper;
		private readonly IUmbracoHelper _umbracoHelper;

		public BlogsPageViewModelFactory(IUmbracoMapper umbracoMapper, IUmbracoHelper umbracoHelper) : base(umbracoMapper, umbracoHelper)
		{
			this._umbracoMapper = umbracoMapper;
			_umbracoHelper = umbracoHelper;
		}

		public override void SetViewModelProperties(BlogsPage viewModel, ContentModel contentModel)
		{

			this._umbracoMapper.Map(contentModel.Content, viewModel);

			if (viewModel.Children != null && viewModel.Children.Any())
			{
				var blogPages = new List<BlogPage.BlogPage>();
				var blogChildren = this._umbracoHelper.ChildrenOfType(contentModel.Content, DocumentTypeAlias.BlogPage);
				this._umbracoMapper.MapCollection(blogChildren, blogPages);
				viewModel.BlogPages = blogPages;
			}

			if (viewModel.BlogPages?.Count() >= 1)
			{
				viewModel.ShowPosts = true;
				viewModel.Posts = viewModel.BlogPages;
			}

			viewModel.PostsTitle = viewModel.Title;
		}
	}
}