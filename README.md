# TwitchService

[![GitHub Release](https://img.shields.io/github/v/release/NuboHeimer-for-streamers/TwitchService)](https://github.com/NuboHeimer-for-streamers/TwitchService/releases/latest)
[![License: CC BY-NC-SA 4.0](https://img.shields.io/badge/License-CC%20BY--NC--SA%204.0-lightgrey.svg)](https://creativecommons.org/licenses/by-nc-sa/4.0/)

Модуль для [Streamer.bot](https://streamer.bot), расширяющий функционал взаимодействия со стриминговой площадкой [Twitch](https://www.twitch.tv).

## ⚠ Требования

- Актуальная версия [Streamer.bot](https://streamer.bot/) Ссылка ведёт на официальный сайт.
- Актуальная версия [Миничат](https://t.me/streamix_group/3). Ссылка ведёт на нужный раздел официальной группы в Telegram.
- **Интеграция Миничат → Streamer.bot**
  - [Для Streamer.bot 0.2.8](https://t.me/StreamfonyBot?start=_tgr_JpK_P4xlZmI6) Ссылка реферальная. Ведёт на приложение в Telegram. Интеграция находится в разделе «Плагины».
  - [Прямая ссылка на интеграцию для Streamer.bot 0.2.8](https://t.me/StreamfonyBot/app?startapp=plugin_19-utm_share). (Ведёт в приложение Telegram).
  - Для Streamer.bot версий от 1.0.0 и выше вам нужна [моя версия интеграции](https://t.me/nuboheimersb/702/757), поскольку оригинальная с этими версиями работает некорректно.

## 🎯 Возможности

![](docs/images/readme/Event_log.png)

- Отправка в журнал событий Миничат зрителя, _впервые_ зашедшего на текущую трансляцию.
- Отправка в журнал событий Миничат зрителя, появившегося в списке зрителей (пришёл в список).
- Отправка в журнал событий Миничат ушедшего зрителя.
- Игнорирование зрителя, написавшего в чат, до того как модуль пометил его "новым".
- Очистка списка "пришедших" зрителей.

## 🚀 Быстрый старт

### Первая установка
1. Импортируйте *TwitchService.txt* в Streamer.bot.
2. Настройте **Present Viewers** для Twitch.
3. Настройте под себя условия очистки списков и включения экшенов.

### Обновление с версии 1.0.3.
1. Импортируйте *TwitchService.txt* в Streamer.bot.
2. Запустите два экшена через test trigger:
- **\[Twitch] Remove twitch_todays_viewers**
- **\[Twitch] Remove twitchLastViewersNameList**

## 📚 Подробные руководства
- **[Установка и обновление](docs/INSTALLATION.md)**
- **[Настройка и использование](docs/USAGE.md)**

## 🆘 Поддержка

- **Автор**: NuboHeimer
- **Личка Telegram**: [@nuboheimer](https://t.me/nuboheimer)
- **Группа Telegram**: [@nuboheimersb](https://t.me/nuboheimersb/30)
- **Email**: nuboheimer@yandex.ru
- **VK**: [vk.com/nuboheimer](https://vk.com/nuboheimer)

## 📄 Лицензия

Этот проект распространяется под лицензией [Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License](https://creativecommons.org/licenses/by-nc-sa/4.0/).

**Вы можете:**

- 🔄 **Делиться** — копировать и распространять код
- 🔧 **Адаптировать** — изменять, трансформировать и улучшать код

**При следующих условиях:**

- 📝 **Атрибуция** — Вы должны указать авторство
- 🚫 **Некоммерческое использование** — Материал нельзя использовать в коммерческих целях
- 🔗 **ShareAlike** — При изменении кода Вы должны распространять его под той же лицензией

Полный текст лицензии доступен в файле [LICENSE](LICENSE).
