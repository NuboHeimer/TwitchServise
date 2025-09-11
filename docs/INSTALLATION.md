# TwitchService - Руководство по установке и обновлению.

## Disclaimer
На данный момент я не рекомендую пользоваться версией 1.0.0 стримербота, так как имеются проблемы работы интеграции с миничатом с этой версией.

## Установка.
1. Скачайте файл `TwitchService.txt` из последнего релиза.
2. Запустите стримербот.
3. В верхнем меню нажмите кнопку `Import`.
![](./images/installation/import_btn.png)
4. Перетащите скачанный ранее `TwitchService.txt` в область `Import String`. Если перетащить не получается, откройте файл блокнотом, скопируйте текст и вставьте его в `Import String`.
5. Нажмите кнопку Import справа внизу.
![](./images/installation/import_btn2.png)
6. Установка завершена.

## Обновление с версии 1.0.3.
1. Запустите экшен **\[Twitch] Remove twitch_todays_viewers**
![](./images/installation/Remove_twitch_todays_viewers.png)
2. Запустите экшен **\[Twitch] Remove twitchLastViewersNameList**
![](./images/installation/Remove_twitchLastViewersNameList.png)

Это нужно для удаления старых глобальных переменных, которые больше не используются.