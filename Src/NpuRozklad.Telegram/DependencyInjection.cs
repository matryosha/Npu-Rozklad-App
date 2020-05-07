using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NpuRozklad.Telegram.BotActions;
using NpuRozklad.Telegram.Display.Common.Controls;
using NpuRozklad.Telegram.Display.Common.Controls.FacultyGroupsInlineMenu;
using NpuRozklad.Telegram.Display.Common.Controls.KeyboardMarkupMenu;
using NpuRozklad.Telegram.Display.Common.Text;
using NpuRozklad.Telegram.Display.Timetable;
using NpuRozklad.Telegram.Display.Timetable.SelectingFacultyGroupToAddMenu;
using NpuRozklad.Telegram.Display.Timetable.SelectingFacultyMenu;
using NpuRozklad.Telegram.Display.Timetable.TimetableFacultyGroupViewMenu;
using NpuRozklad.Telegram.Handlers;
using NpuRozklad.Telegram.Handlers.CallbackQueryHandlers;
using NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers;
using NpuRozklad.Telegram.Handlers.MessageHandlers;
using NpuRozklad.Telegram.Interfaces;
using NpuRozklad.Telegram.LongLastingUserActions;
using NpuRozklad.Telegram.Persistence;
using NpuRozklad.Telegram.Services;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTelegramNpu(this IServiceCollection services,
            Action<TelegramNpuOptions> telegramNpuOptionsBuilder)
        {
            var options = new TelegramNpuOptions();
            telegramNpuOptionsBuilder(options);

            services.AddScoped<ResetCurrentUserAction>();
            services.AddScoped<ShowApplicationVersionAction>();
            services.AddScoped<ShowFacultyGroupsForFacultyDoesNotExistMessageAction>();
            services.AddScoped<ShowMessageAction>();
            services.AddScoped<ShowMainMenuAction>();
            services.AddScoped<ShowTimetableFacultyGroupsMenuAction>();
            services.AddScoped<ShowTimetableFacultyGroupViewMenuAction>();
            services.AddScoped<ShowTimetableSelectingFacultyGroupToAddMenuAction>();
            services.AddScoped<ShowTimetableSelectingFacultyMenuAction>();
            services.AddScoped<ShowTimetableFacultyGroupsRemoveMenuAction>();

            services.AddSingleton<IFacultyGroupsInlineMenuCreator, FacultyGroupsInlineMenuCreator>();
            services.AddSingleton<KeyboardMarkupMenuCreator>();
            services.AddSingleton<BackInlineButtonCreator>();
            services.AddSingleton<InlineKeyboardButtonsCreator>();
            services.AddSingleton<MainMenuCreator>();
            services.AddScoped<OneDayLessonsToTelegramMessageText>();
            services.AddScoped<TimetableFacultyGroupsKeyboardCreator>();
            services.AddScoped<TimetableFacultyListKeyboardCreator>();
            services.AddScoped<DayOfWeekInlineButtonsCreator>();
            services.AddScoped<TimetableFacultyGroupViewInlineMenuCreator>();
            services.AddScoped<WeekSelectorInlineButtonsCreator>();
            services.AddScoped<TimetableFacultyGroupsMenu>();
            services.AddTransient<TimetableFacultyGroupsRemoveMenu>();

            services.AddScoped<ITelegramUpdateHandler, TelegramUpdateHandler>();
            services.AddSingleton<ICallbackQueryHandler, CallbackQueryGlobalHandler>();
            services.AddSingleton<ITelegramMessageHandler, TelegramMessageGlobalHandler>();
            services.AddSingleton<AddGroupCallbackHandler>();
            services.AddSingleton<ShowTimetableFacultyGroupsMenuCallbackHandler>();
            services.AddTransient<ShowTimetableFacultyGroupViewMenuCallbackHandler>();
            services.AddSingleton<SpecificCallbackQueryHandlerProvider>();
            services.AddSingleton<CommandHandler>();
            services.AddSingleton<LongLastingUserActionGeneralHandler>();
            services.AddSingleton<MessageTextHandler>();

            services.AddSingleton<ILongLastingUserActionHandlerFactory, LongLastingUserActionHandlerFactory>();
            services.AddSingleton<ILongLastingUserActionManager, LongLastingUserActionManager>();
            
            services.AddScoped<ITelegramRozkladUserDao, TelegramRozkladUserDao>();

            services.AddScoped<ICurrentTelegramUserContext, CurrentTelegramUserContext>();
            services.AddTransient<ICurrentUserInitializerService, CurrentUserInitializerService>();
            services.AddScoped<ICurrentScopeMessageIdProvider, CurrentScopeMessageIdProvider>();
            services.AddSingleton<ICurrentUserLocalizationService, CurrentUserLocalizationService>();
            services.AddSingleton<ICurrentTelegramUserProvider, CurrentTelegramUserProvider>();
            services.AddTelegramBotClient(options.BotApiToken);
            services.AddSingleton<ITelegramBotActions, TelegramBotActions>();
            services.AddTransient<TimetableSelectingFacultyActionHandler>();
            services.AddTransient<TimetableSelectingFacultyGroupToAddActionHandler>();
            services.AddTransient<TimetableFacultyGroupsMenuGroupSelectedHandler>();
            services.AddTransient<OpenRemoveGroupsMenuCallbackHandler>();
            services.AddTransient<RemoveGroupCallbackHandler>();

            services.AddTelegramDbContext(options.ConnectionString);

            return services;
        }

        private static void AddTelegramBotClient(this IServiceCollection services, string botApiToken)
        {
            if (string.IsNullOrWhiteSpace(botApiToken))
                throw new ArgumentNullException(nameof(botApiToken));
            
            services.AddSingleton<ITelegramBotService>(provider =>
            {
                var externalServiceProvider = provider.GetService<ICurrentScopeServiceProvider>();
                return new TelegramBotService(botApiToken, externalServiceProvider);
            });
        }

        private static void AddTelegramDbContext(this IServiceCollection services, 
            string connectionString)
        {
            services.AddDbContext<TelegramDbContext>(builder => 
                builder.UseMySql(connectionString, optionsBuilder => 
                    optionsBuilder.EnableRetryOnFailure(10)));

            var provider = services.BuildServiceProvider();
            var dbContext = provider.GetService<TelegramDbContext>();
            
            dbContext.Database.Migrate();
        }
    }

    public class TelegramNpuOptions
    {
        public string BotApiToken { get; set; }
        public string ConnectionString { get; set; } 
    }
}