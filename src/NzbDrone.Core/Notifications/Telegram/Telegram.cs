using System.Collections.Generic;
using FluentValidation.Results;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Tv;

namespace NzbDrone.Core.Notifications.Telegram
{
    public class Telegram : NotificationBase<TelegramSettings>
    {
        private string FormatMessageWithLink(string message, Series series)
        {
            if (Settings.SendMetadataLink)
            {
                if (Settings.MetadataLinkType is MetadataLinkType.IMDb)
                {
                    message = FormatImdbLinkFromId(message, series.ImdbId.ToString());
                }
                else if (Settings.MetadataLinkType is MetadataLinkType.TVDb)
                {
                    message = FormatTvdbLinkFromId(message, series.TvdbId.ToString());
                }
                else if (Settings.MetadataLinkType is MetadataLinkType.Trakt)
                {
                    message = FormatTraktLinkFromId(message, series.TvdbId.ToString());
                }
                else if (Settings.MetadataLinkType is MetadataLinkType.TVMaze)
                {
                    message = FormatTVMazeLinkFromId(message, series.TvMazeId.ToString());
                }
            }

            return message;
        }

        private readonly ITelegramProxy _proxy;
        private string FormatImdbLinkFromId(string message, string id)
        {
            return $"[{message}](https://www.imdb.com/title/{id})";
        }

        private string FormatTvdbLinkFromId(string message, string id)
        {
            return $"[{message}](http://www.thetvdb.com/?tab=series&id={id})";
        }

        private string FormatTraktLinkFromId(string message, string id)
        {
            return $"[{message}](http://trakt.tv/search/tvdb/{id}?id_type=show)";
        }

        private string FormatTVMazeLinkFromId(string message, string id)
        {
            return $"[{message}](http://www.tvmaze.com/shows/{id}/_)";
        }

        public Telegram(ITelegramProxy proxy)
        {
            _proxy = proxy;
        }

        public override string Name => "Telegram";
        public override string Link => "https://telegram.org/";

        public override void OnGrab(GrabMessage grabMessage)
        {
            _proxy.SendNotification(EPISODE_GRABBED_TITLE, grabMessage.Message, Settings);
        }

        public override void OnDownload(DownloadMessage message)
        {
            _proxy.SendNotification(EPISODE_DOWNLOADED_TITLE, message.Message, Settings);
        }

        public override void OnEpisodeFileDelete(EpisodeDeleteMessage deleteMessage)
        {
            _proxy.SendNotification(EPISODE_DELETED_TITLE, deleteMessage.Message, Settings);
        }

        public override void OnSeriesAdd(SeriesAddMessage message)
        {
            var text = FormatMessageWithLink(message.Message, message.Series);
            _proxy.SendNotification(SERIES_ADDED_TITLE, text, Settings);
        }

        public override void OnSeriesDelete(SeriesDeleteMessage deleteMessage)
        {
            var text = FormatMessageWithLink(deleteMessage.Message, deleteMessage.Series);
            _proxy.SendNotification(SERIES_DELETED_TITLE, deleteMessage.Message, Settings);
        }

        public override void OnHealthIssue(HealthCheck.HealthCheck healthCheck)
        {
            _proxy.SendNotification(HEALTH_ISSUE_TITLE, healthCheck.Message, Settings);
        }

        public override void OnHealthRestored(HealthCheck.HealthCheck previousCheck)
        {
            _proxy.SendNotification(HEALTH_RESTORED_TITLE, $"The following issue is now resolved: {previousCheck.Message}", Settings);
        }

        public override void OnApplicationUpdate(ApplicationUpdateMessage updateMessage)
        {
            _proxy.SendNotification(APPLICATION_UPDATE_TITLE, updateMessage.Message, Settings);
        }

        public override void OnManualInteractionRequired(ManualInteractionRequiredMessage message)
        {
            _proxy.SendNotification(MANUAL_INTERACTION_REQUIRED_TITLE, message.Message, Settings);
        }

        public override ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();

            failures.AddIfNotNull(_proxy.Test(Settings));

            return new ValidationResult(failures);
        }
    }
}
