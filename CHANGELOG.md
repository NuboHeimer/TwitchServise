# Changelog

Все значимые изменения в проекте TwitchService описываются в этом файле.

Формат основан на [Keep a Changelog](https://keepachangelog.com/ru/1.0.0/).

## [Unreleased]

### Добавлено
- **GetInOutViewers** — отдельные события «пришёл в список зрителей» и «вышел из списка зрителей».
- Глобальная переменная `twitchPreviousPresentViewers` для хранения списка зрителей на предыдущем опросе.
- **ClearPreviousPresentViewers** — очистка списка предыдущих присутствующих зрителей.
- **RemoveTwitchTodaysViewersVariable** и **RemoveTwitchLastViewersNameListVariable** — удаление старых глобальных переменных при обновлении с 1.0.3.
- Класс **Logger** — единое логирование с префиксом `[TwitchService]:` и уровнями Verbose, Debug, Info, Warn, Error.
- **ErrorHandler** — обёртка над публичными методами: перехват исключений, запись в лог, возврат `false` при ошибке.
- Инициализация `twitchPreviousPresentViewers` в **Init** при отсутствии переменной.
- Документация: `docs/INSTALLATION.md`, `docs/USAGE.md`, инструкция по действию «Add First Word Viewer», изображения для руководств.
- CI: GitHub Action для переноса issue в колонку Testing.
- Обновлены README, установка, использование; добавлена лицензия CC BY-NC-SA 4.0.

### Изменено
- **GetNewViewers** — запись глобальной переменной вынесена из цикла (одна запись после обработки всех зрителей).
- Переход с `List<string>` на `HashSet<string>` для `twitchTodaysViewers` и `twitchPreviousPresentViewers`.
- Переименование переменных/методов: `ClearTodayViewers` → **ClearTodaysViewers**, согласованные имена глобальных переменных (`twitchTodaysViewers`, `twitchPreviousPresentViewers`).
- Формирование события для MiniChat вынесено в отдельный метод **CreateViewerEvent** (вызов «MiniChat Method Collection» → CreateCustomEvent).
- Структура кода: бизнес-логика вынесена в класс **TwitchServiceInternal**; публичные методы в `CPHInline` только вызывают внутренние и оборачивают в `ErrorHandler`.
- Класс **Internal** переименован в **TwitchServiceInternal**.
- В перегрузках **Logger** (Verbose, Debug, Info, Warn, Error с `params object[]`) формирование строки заменено на `string.Join(", ", additional)` вместо цикла с конкатенацией.
- Оформление: исправлены отступы, убраны дублирующий префикс в логах и лишние комментарии; в `Logger` поля приведены к `_camelCase` и помечены `readonly`; **ErrorHandler** перенесён в конец класса `CPHInline`; исправлена позиция лога в **Init**.
- Обновлён .gitignore под текущую структуру проекта; исправлены опечатки в названии проекта в CI.

### Исправлено
- **AddFirstWordViewer** — удалено добавление пользователя в `twitchPreviousPresentViewers`, чтобы не дублировать логику прихода/ухода зрителей.
- Импорт: добавлено отсутствующее действие в файл экспорта.

### Удалено
- Метод **GetPresentViewersCount** / **GetViewersCount** (функциональность убрана).
- Конфигурация semantic-release, package.json, .releaserc.json (упрощение структуры проекта).

## [1.0.3] - 2024-09-09

### Добавлено
- Модуль Twitch Service для Streamer.bot: работа со зрителями Twitch и отправка событий в MiniChat.
- **Init** — инициализация при загрузке: создание глобальной переменной `twitch_todays_viewers` (список зрителей за текущую трансляцию).
- **GetNewViewers** — определение зрителей, впервые появившихся на трансляции, и создание для них событий в MiniChat.
- **ClearTodayViewers** — очистка списка зрителей за сегодня.
- Файл для импорта действий в Streamer.bot.
- README и базовая документация.
