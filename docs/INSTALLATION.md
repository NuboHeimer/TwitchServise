# TwitchService - Руководство по установке и обновлению.

## Установка.
1. Скачайте файл `TwitchService.txt` из последнего [релиза](https://github.com/NuboHeimer-for-streamers/TwitchService/releases/latest).
2. Запустите стримербот.
3. В верхнем меню нажмите кнопку `Import`.

<p align="center">
  <img src="./images/installation/import_btn.png" alt="Кнопка Import">
</p>
4. Перетащите скачанный ранее `TwitchService.txt` в область `Import String`. Если перетащить не получается, откройте файл блокнотом, скопируйте текст и вставьте его в `Import String`.
5. Нажмите кнопку Import справа внизу.

<p align="center">
  <img src="./images/installation/import_btn2.png" alt="Кнопка Import внизу справа">
</p>
5.1. Начиная с версии 1.0.0 Streamer.bot предупреждает, что вы импортируете кастомный C# код. Соглашаемся.

<p align="center">
  <img src="./images/installation/Warning.png" alt="Предупреждение об импорте кастомного кода">
</p>

6. Установка завершена.

## Обновление с версии 1.0.3.
1. Запустите экшен **\[Twitch] Remove twitch_todays_viewers**

<p align="center">
  <img src="./images/installation/Remove_twitch_todays_viewers.png" alt="Экшен Remove twitch_todays_viewers">
</p>
2. Запустите экшен **\[Twitch] Remove twitchLastViewersNameList**

<p align="center">
  <img src="./images/installation/Remove_twitchLastViewersNameList.png" alt="Экшен Remove twitchLastViewersNameList">
</p>

Это нужно для удаления старых глобальных переменных, которые больше не используются.