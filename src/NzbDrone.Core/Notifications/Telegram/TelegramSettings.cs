using System.ComponentModel;
using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;
namespace NzbDrone.Core.Notifications.Telegram
{
    public enum MetadataLinkType
    {
        IMDb,
        TVDb,
        TVMaze,
        Trakt,
    }

    public class TelegramSettingsValidator : AbstractValidator<TelegramSettings>
    {
        public TelegramSettingsValidator()
        {
            RuleFor(c => c.BotToken).NotEmpty();
            RuleFor(c => c.ChatId).NotEmpty();
            RuleFor(c => c.TopicId).Must(topicId => !topicId.HasValue || topicId > 1)
                                   .WithMessage("Topic ID must be greater than 1 or empty");
        }
    }

    public class TelegramSettings : IProviderConfig
    {
        private static readonly TelegramSettingsValidator Validator = new TelegramSettingsValidator();

        [FieldDefinition(0, Label = "NotificationsTelegramSettingsBotToken", Privacy = PrivacyLevel.ApiKey, HelpLink = "https://core.telegram.org/bots")]
        public string BotToken { get; set; }

        [FieldDefinition(1, Label = "NotificationsTelegramSettingsChatId", HelpLink = "http://stackoverflow.com/a/37396871/882971", HelpText = "NotificationsTelegramSettingsChatIdHelpText")]
        public string ChatId { get; set; }

        [FieldDefinition(2, Label = "NotificationsTelegramSettingsTopicId", HelpLink = "https://stackoverflow.com/a/75178418", HelpText = "NotificationsTelegramSettingsTopicIdHelpText")]
        public int? TopicId { get; set; }

        [FieldDefinition(3, Label = "NotificationsTelegramSettingsSendSilently", Type = FieldType.Checkbox, HelpText = "NotificationsTelegramSettingsSendSilentlyHelpText")]
        public bool SendSilently { get; set; }

        [DefaultValue(true)]
        [FieldDefinition(4, Label = "NotificationsTelegramSettingsMetadataLinkType", Type = FieldType.Checkbox, HelpText = "NotificationsTelegramSettingsSendMetadataLink")]
        public bool SendMetadataLink { get; set; }
        [FieldDefinition(5, Label = "NotificationsTelegramSettingsMetadataLinkType", Type = FieldType.Select, SelectOptions = typeof(MetadataLinkType), HelpText = "NotificationsTelegramSettingsMetadataLinkType")]
        public MetadataLinkType MetadataLinkType { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
