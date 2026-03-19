# Changelog

Все значимые изменения в проекте TwitchService описываются в этом файле.

Формат основан на [Keep a Changelog](https://keepachangelog.com/ru/1.0.0/).

## [Unreleased]

### Добавлено
- **GetPaidSubscribers** — получение списка текущих платных подписчиков Twitch (включая Prime) через Twitch Helix API.
- Формирование аргумента `twitchPaidSubscribers` в формате, аналогичном `users` из PresentViewers, c доп. полями `userId`, `displayName`, `tier`, `isGift`.

## [1.1.0] - 2025-11-02

### Добавлено
- **GetInOutViewers** — отдельные события «пришёл в список зрителей» и «вышел из списка зрителей».
- Глобальная переменная `twitchPreviousPresentViewers` для хранения списка зрителей на предыдущем опросе.
- **ClearPreviousPresentViewers** — очистка списка предыдущих присутствующих зрителей.
- **RemoveTwitchTodaysViewersVariable** и **RemoveTwitchLastViewersNameListVariable** — удаление старых глобальных переменных при обновлении с 1.0.3.
- Единое логирование с префиксом `[TwitchService]:` и уровнями Verbose, Debug, Info, Warn, Error.
- Обработка ошибок: перехват исключений, запись в лог, возврат `false` при ошибке.
- Инициализация `twitchPreviousPresentViewers` в **Init** при отсутствии переменной.
- Документация: `docs/INSTALLATION.md`, `docs/USAGE.md`, инструкция по действию «Add First Word Viewer», изображения для руководств.
- Обновлены README, установка, использование; добавлена лицензия CC BY-NC-SA 4.0.

### Изменено
- **GetNewViewers** — оптимизирована запись глобальной переменной (одна запись после обработки всех зрителей).
- Переход с `List<string>` на `HashSet<string>` для `twitchTodaysViewers` и `twitchPreviousPresentViewers`.
- Переименование метода: `ClearTodayViewers` → **ClearTodaysViewers**.
- Согласованы имена глобальных переменных: `twitchTodaysViewers`, `twitchPreviousPresentViewers`.

### Исправлено
- **AddFirstWordViewer** — удалено добавление пользователя в `twitchPreviousPresentViewers`, чтобы не дублировать логику прихода/ухода зрителей.
- Импорт: добавлено отсутствующее действие в файл экспорта.

### Удалено
- Метод **GetPresentViewersCount** / **GetViewersCount** (функциональность убрана).

## [1.0.3] - 2024-09-09

### Добавлено
- Модуль Twitch Service для Streamer.bot: работа со зрителями Twitch и отправка событий в MiniChat.
- **Init** — инициализация при загрузке: создание глобальной переменной `twitch_todays_viewers` (список зрителей за текущую трансляцию).
- **GetNewViewers** — определение зрителей, впервые появившихся на трансляции, и создание для них событий в MiniChat.
- **ClearTodayViewers** — очистка списка зрителей за сегодня.
- Файл для импорта действий в Streamer.bot.
- README и базовая документация.
