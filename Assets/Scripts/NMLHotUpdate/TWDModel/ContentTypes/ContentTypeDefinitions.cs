using System.Collections.Generic;
using System.IO;
using System.Text;
using BaseModel.ContentTypes;

namespace TWDModel.ContentTypes
{
	public static class ContentTypeDefinitions
	{
		private static MessageSerializer _jsonMessageSerializer = new MessageSerializer();

		private static List<string> _supportedLanguagesAll = new List<string>
		{
			"en", "de", "es", "fr", "it", "pt-br", "ru", "tr", "zh-cn", "zh-tw",
			"ja", "ko"
		};

		private static List<string> _supportedLanguagesLight = new List<string>
		{
			"ru", "en", "de", "fr", "it", "es"
		};

		private static List<string> _supportedLanguages = OfflineManager.IsLoadDataManager ? _supportedLanguagesLight : _supportedLanguagesAll;

		private static List<string> _supportedNewsCategories = new List<string> { "Alerts", "Pinned Items" };

		public static List<string> SupportedLanguages => _supportedLanguages;

		public static List<string> SupportedNewsCategories => _supportedNewsCategories;

		public static ContentTypeGroup[] GetContentTypeGroups()
		{
			return new ContentTypeGroup[3]
			{
				new ContentTypeGroup
				{
					Id = "ConfigurationContentGroup",
					Name = "Configuration",
					Description = "Group of different configurations that apps will consume",
					ContentTypes = new ContentType[2]
					{
						new ContentType
						{
							Id = "AdMediation",
							Name = "Ad Mediation",
							ConcreteType = typeof(MediationData),
							Description = "Ad Mediation settings will enable the apps to select multiple ad providers"
						},
						new ContentType
						{
							Id = "UnityAdsIds",
							Name = "Unity Ads Ids",
							ConcreteType = typeof(UnityAdsIds),
							Description = "All the ids for the Unity ads"
						}
					}
				},
				new ContentTypeGroup
				{
					Id = "MultimediaContentGroup",
					Name = "Multimedia",
					Description = "Description of Multimedia Content Type Group",
					ContentTypes = new ContentType[3]
					{
						new ContentType
						{
							Id = "Banner",
							Name = "Banner",
							ConcreteType = typeof(Banner),
							Description = "Description of Banner"
						},
						new ContentType
						{
							Id = "EpisodeVideo",
							Name = "Episode Video",
							ConcreteType = typeof(EpisodeVideo),
							Description = "Description of Episode Content Type"
						},
						new ContentType
						{
							Id = "NewsItem",
							Name = "News Item",
							ConcreteType = typeof(NewsItem),
							Description = "Description of News Item Content Type"
						}
					}
				},
				new ContentTypeGroup
				{
					Id = "CdnContentType",
					Name = "CDN Content Types",
					Description = "This group defines content types that consume BLOB storage and the content is delivered to the client via CDN URL.",
					ContentTypes = new ContentType[4]
					{
						new ContentType
						{
							Id = "Localization",
							Name = "Localization",
							Description = "Provides processed localization content to the game runtime. See deatils on <a href='https://intra.nextgames.com/display/TWD/Pushing+Localization+Files+to+Server'>\"Pushing Localization Files to Server\" on Intra</a>.",
							HttpContentEncoding = "gzip",
							DataSourceKind = CdnContentDataSourceKind.FileUpload,
							ContentViewerFlags = ContentViewerFlags.PlainText,
							DeduplicateByPath = true,
							Parse = delegate(ContentTypeParseContext context)
							{
								string path = "Localization/" + Path.GetFileNameWithoutExtension(context.Filename);
								string s = Encoding.UTF8.GetString(context.Content);
								string fileName = Path.GetFileName(context.Filename);
								return new List<ContentParserResponse>
								{
									new ContentParserResponse
									{
										Content = Encoding.UTF8.GetBytes(s),
										Path = path,
										Filename = fileName,
										HttpContentType = context.ContentType
									}
								};
							}
						},
						new ContentType
						{
							Id = "Image",
							Name = "Image",
							Description = "Provides generic images for game runtime.",
							DataSourceKind = CdnContentDataSourceKind.FileUpload,
							ContentViewerFlags = ContentViewerFlags.ImageGallery,
							Parse = delegate(ContentTypeParseContext context)
							{
								string path = "Image/" + Path.GetFileNameWithoutExtension(context.Filename);
								string fileName = Path.GetFileName(context.Filename);
								return new List<ContentParserResponse>
								{
									new ContentParserResponse
									{
										Content = context.Content,
										Path = path,
										Filename = fileName,
										HttpContentType = context.ContentType
									}
								};
							}
						},
						new ContentType
						{
							Id = "GED",
							Name = "Game Economy Data",
							Description = "Transforms GED sheet from Google Drive into game runtime objects. See details at <a target='_blank' href='https://intra.nextgames.com/display/TWD/GED'>\"GED\" on Intra</a>.",
							HttpContentEncoding = "gzip",
							DataSourceKind = CdnContentDataSourceKind.GoogleDrive,
							ContentViewerFlags = (ContentViewerFlags.Json | ContentViewerFlags.JsonDiff),
							DeduplicateByPath = false,
							Parse = delegate(ContentTypeParseContext context)
							{
								byte[] array = context.Content;
								if (true)
								{
									UnityCompatibleMessageSerializer unityCompatibleMessageSerializer = new UnityCompatibleMessageSerializer();
									string value = Encoding.UTF8.GetString(array);
									GameEconomyData gameEconomyData = _jsonMessageSerializer.Deserialize<GameEconomyData>(value);
									gameEconomyData.Version = 2;
									string s = unityCompatibleMessageSerializer.Serialize(gameEconomyData);
									array = Encoding.UTF8.GetBytes(s);
								}
								return new List<ContentParserResponse>
								{
									new ContentParserResponse
									{
										Path = "GED",
										Filename = context.Filename,
										Content = array,
										HttpContentType = context.ContentType
									}
								};
							}
						},
						new ContentType
						{
							Id = "RunLocation",
							Name = "Run Location",
							Description = "Run Locations define templates that the different missions will take place in. See details at <a target='_blank' href='https://intra.nextgames.com/display/TWD/Tools+System'>\"Tools System\" on Intra</a>.",
							HttpContentEncoding = "gzip",
							DataSourceKind = CdnContentDataSourceKind.FileUpload,
							ContentViewerFlags = ContentViewerFlags.Json,
							DeduplicateByPath = true,
							Parse = delegate(ContentTypeParseContext context)
							{
								List<ContentParserResponse> list = new List<ContentParserResponse>();
								RunLocationModel runLocationModel = null;
								string text = Encoding.UTF8.GetString(context.Content);
								string fileName = Path.GetFileName(context.Filename);
								try
								{
									runLocationModel = _jsonMessageSerializer.Deserialize<RunLocationModel>(text);
								}
								catch
								{
									list.Add(new ContentParserResponse
									{
										Errors = new List<string> { "Unable to deserialize the JSON file: " + fileName }
									});
								}
								if (runLocationModel == null)
								{
									return list;
								}
								foreach (MissionModel mission in runLocationModel.Missions)
								{
									string path = "RunLocation/" + mission.Id;
									list.Add(new ContentParserResponse
									{
										Content = Encoding.UTF8.GetBytes(text),
										Path = path,
										Filename = fileName,
										HttpContentType = context.ContentType
									});
								}
								return list;
							}
						}
					}
				}
			};
		}
	}
}
