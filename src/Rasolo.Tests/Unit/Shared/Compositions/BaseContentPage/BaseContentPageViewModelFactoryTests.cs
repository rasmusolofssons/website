﻿using System.Web;
using Moq;
using NUnit.Framework;
using Rasolo.Core.Features.Shared.Composers;
using Rasolo.Core.Features.Shared.Compositions;
using Rasolo.Core.Features.Shared.Constants.PropertyTypeAlias;
using Rasolo.Tests.Unit.Base;
using Shouldly;
using Umbraco.Core.Models.PublishedContent;

namespace Rasolo.Tests.Unit.Shared.Compositions.BaseContentPage
{
	internal class BaseContentPageViewModelFactoryTests <TModel> : UmbracoBaseTests where TModel : Core.Features.Shared.Compositions.BaseContentPage, new()
	{
		private BaseContentPageViewModelFactory<Core.Features.Shared.Compositions.BaseContentPage> _sut;

		public override void SetUp()
		{
			base.SetUp();
			this._sut = new BaseContentPageViewModelFactory<Core.Features.Shared.Compositions.BaseContentPage>();
		}

		[Test]
		[TestCase(null, "")]
		[TestCase("", "")]
		public void Given_CreateModel_When_TitleNullOrEmpty_Then_ReturnViewModelWithTitleEmptyString(string title, string expected)
		{
			var contentPage = new TModel() { Title = title };
			var viewModel = this._sut.CreateModel(contentPage);

			viewModel.Title.ShouldBe(expected);
		}

		[Test]
		[TestCase("The content page title", "The content page title")]
		[TestCase("Another content page title", "Another content page title")]
		public void Given_CreateModel_When_TitleGiven_Then_ReturnViewModelWithTitle(string title, string expected)
		{
			var contentPage = new TModel() { Title = title };
			var viewModel = this._sut.CreateModel(contentPage);

			viewModel.Title.ShouldBe(expected);
		}

		[Test]
		[TestCase("The content page main body", "The content page main body")]
		[TestCase("Another content page main body", "Another content page main body")]
		public void Given_CreateModel_When_MainBodyGiven_Then_ReturnViewModelWithMainBody(string mainBody, string expected)
		{
			var contentPage = new TModel() { MainBody = new HtmlString(mainBody) };
			var viewModel = this._sut.CreateModel(contentPage);

			viewModel.MainBody.ToString().ShouldBe(expected);
		}

		[Test]
		public void Given_CreateModel_When_HeroImageGiven_Then_ReturnViewModelWithHeroImage()
		{
			var page = new Core.Features.Shared.Compositions.BaseContentPage();
			var viewModel = SetUpGetHeroImage(page);

			viewModel.HeroImage.ShouldBe(page.HeroImage);
		}

		Core.Features.Shared.Compositions.BaseContentPage SetUpGetHeroImage(Core.Features.Shared.Compositions.BaseContentPage page)
		{
			var imageMock = new Mock<IPublishedProperty>();
			imageMock.Setup(c => c.Alias).Returns(BaseContentPagePropertyAlias.HeroImage);
			imageMock.Setup(c => c.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			imageMock.Setup(c => c.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns(SetupImage().Object);
			var contentModel = SetupContent(typeof(TModel).Name, imageMock);
			var umbracoMapper = new UmbracoMapperComposer().SetupMapper();
			umbracoMapper.Map(contentModel.Content, page);

			return this._sut.CreateModel(page);
		}

		[Test]
		public void Given_CreateModel_When_HeroImageGiven_Then_ReturnViewModelWithShowHeroImageTrue()
		{
			var viewModel = SetUpGetHeroImage(new Core.Features.Shared.Compositions.BaseContentPage());

			viewModel.ShowHeroImage.ShouldBe(true);
		}

		[Test]
		public void Given_CreateModel_When_HeroImageNotGiven_Then_ReturnViewModelWithShowHeroImageFalse()
		{
			var contentPage = new TModel() { HeroImage = null };
			var viewModel = this._sut.CreateModel(contentPage);

			viewModel.ShowHeroImage.ShouldBe(false);
		}

		[Test]
		public void Given_CreateModel_When_MainBodyNullOrEmpty_Then_ReturnViewModelWithMainBodyEmptyString()
		{
			var contentPage = new TModel() { MainBody = null };
			var viewModel = this._sut.CreateModel(contentPage);

			viewModel.MainBody.ToString().ShouldBe(string.Empty);
		}
	}
}