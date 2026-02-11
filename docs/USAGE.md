# TwitchService - Руководство по использованию

## Disclaimer

Предполагается, что у вас уже есть установленная и настроенная интеграция с миничатом, а также настроенное подключение стримербота к твичу.

## Настройка стримербота.

1. Откройте раздел **Platforms -> Twitch -> Settings**.  

<p align="center">
  <img src="./images/usage/Platforms_Twitch_Settings.png" alt="Настройки Twitch в Streamer.bot">
</p>
2. В разделе `Present Viewers` установите галочки:

- Enabled,
- Live Update.  
Ползунок установите на одну минуту.  

<p align="center">
  <img src="./images/usage/Twitch_PresentViewers.png" alt="Настройки Present Viewers">
</p>

## Работа с экшенами.

### Общие рекомендации по триггерам

>⚠ Важно
>
> При перезапуске трансляции (падение и рестарт) стрим может интерпретироваться как новый.
> Чтобы избежать некорректной очистки списков и ошибок в определении зрителей, используйте более явный триггер: **OBS -> Streaming Started** или привязанную к запуску стрима горячую клавишу.

### \[Twitch] Add First Word Viewer

Экшен добавляет зрителя, написавшего в чат впервые за сегодняшнюю трансляцию, в список «впервые зашедших» зрителей. Это позволяет не отправлять событие в MiniChat для уже увиденных в чате зрителей.  

<p align="center">
  <img src="./images/usage/Twitch_Add_First_Words_Viewer.png" alt="Экшен Add First Word Viewer">
</p>

- Триггер: **Twitch -> General -> First Words**
- Используйте, если не хотите видеть событие в MiniChat для зрителей, написавших сообщение в чат. Если событие нужно — отключите этот экшен.

### \[Twitch] Clear First Words

Экшен очищает сохранённый список зрителей, для которых уже было зафиксировано событие **First Words**, чтобы на новой трансляции первые сообщения снова корректно считались «первыми».

<p align="center">
  <img src="./images/usage/Twitch_Clear_First_Words.png" alt="Экшен Clear First Words">
</p>

- Триггер: **Twitch -> Channel -> Stream Online**.

### \[Twitch] Clear Previous Present Viewers.

Экшен очищает сохранённый список **Present Viewers**, чтобы исключить устаревшие данные перед началом трансляции.  

<p align="center">
  <img src="./images/usage/Twitch_Clear_Previous_Present_Viewers.png" alt="Экшен Clear Previous Present Viewers">
</p>

- Триггер по умолчанию: **Twitch -> Channel -> Stream Online**.
- Очищайте список перед запуском стрима, иначе в нём могут храниться устаревшие данные и зрители будут определяться неверно.
- Примечание: рекомендуется использовать тот же подход к выбору триггера, что и для очистки first words (см. блок «Важно» выше), чтобы избежать проблем при падении и рестарте трансляции.

### \[Twitch] Clear Todays Viewers.

Экшен очищает сохранённый список «сегодняшних» зрителей для корректного определения новых зрителей текущей трансляции.  

<p align="center">
  <img src="./images/usage/Twitch_Clear_Todays_Viewers.png" alt="Экшен Clear Todays Viewers">
</p>

- Триггер: **Twitch -> Channel -> Stream Online**.
- Очищайте список перед запуском стрима, иначе могут сохраниться устаревшие данные и новые зрители будут определяться неверно.
- Примечания: рекомендуется заменить триггер на более явный, см. блок «Важно» выше.

### \[Twitch] Code

Служебный экшен с кодом. Так же в нём можно узнать текущую версию (указана в комментарии в сабэкшенах и в самом коде).  

<p align="center">
  <img src="./images/usage/Twitch_Version.png" alt="Экшен Code с версией скрипта">
</p>

### \[Twitch] Get In Out Viewers

Экшен отправляет в MiniChat пользовательское событие о пришедшем или ушедшем зрителе. По умолчанию выключен. Заменяет собой функционал Get New Viewers.

<p align="center">
  <img src="./images/usage/Twitch_Get_In_Out_Viewers.png" alt="Экшен Get In Out Viewers">
</p>

<p align="center">
  <img src="./images/usage/User_join.png" alt="Пример события о заходе зрителя">
</p>

<p align="center">
  <img src="./images/usage/User_left.png" alt="Пример события об уходе зрителя">
</p>

- Триггер: **Twitch -> Present Viewers**
- Пишет, когда зритель впервые зашёл на трансляцию.
- Пишет, когда зритель просто появился в списке зрителей (пришёл на трансляцию).
- Пишет, когда зритель пропал из списка зрителей (ушёл с трансляции).
- Примечания: требует корректной настройки **Present Viewers** и предварительной очистки списков перед началом трансляции.

### \[Twitch] Get New Viewers

Экшен отправляет в MiniChat пользовательское событие о новом зрителе на текущей трансляции. По умолчанию выключен. Если используете Get In Out Viewers, то оставьте выключенным.

<p align="center">
  <img src="./images/usage/Twitch_Get_New_Viewers.png" alt="Экшен Get New Viewers">
</p>

<p align="center">
  <img src="./images/usage/New_Viewers.png" alt="Пример события о новом зрителе">
</p>

- Триггер: **Twitch -> Present Viewers**
- Примечания: требует корректной настройки **Present Viewers** и предварительной очистки списков перед началом трансляции.