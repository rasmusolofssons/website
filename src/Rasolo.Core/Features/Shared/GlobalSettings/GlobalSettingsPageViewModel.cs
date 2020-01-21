﻿using AutoMapper.Configuration.Annotations;
using Rasolo.Core.Features.Shared.Compositions;
using Umbraco.Web.Models;

namespace Rasolo.Core.Features.Shared.GlobalSettings
{
	public class GlobalSettingsPageViewModel : IGlobalSettingsPageViewModel
	{
		public string CookiesNoticeText { get; set; }
		public string CookiesAcceptText { get; set; }
		public Link CookiesLink { get; set; }
		[Ignore]
		public bool ShowCookiesNotice { get; set; }
		public string HomeText { get; set; }
		[Ignore]
		public string HomeTextColor { get; set; }
	}
}