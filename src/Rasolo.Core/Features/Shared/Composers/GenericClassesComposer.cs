﻿using Rasolo.Core.Features.ArticlePage;
using Rasolo.Core.Features.Shared.GlobalSettings;
using Rasolo.Core.Features.Shared.Services;
using Rasolo.Core.Features.StartPage;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Rasolo.Core.Features.Shared.Composers
{
	public class GenericClassesComposer : IUserComposer
	{
		public void Compose(Composition composition)
		{
			composition.Register<IArticlePageViewModelFactory, ArticlePageViewModelFactory>();
			composition.Register<IGlobalSettingsPageViewModelFactory, GlobalSettingsPagePageViewModelFactory>();
			composition.Register<IStartPageViewModelFactory, StartPageViewModelFactory>();
			composition.Register<IUmbracoService, UmbracoService>();
		}
	}
}