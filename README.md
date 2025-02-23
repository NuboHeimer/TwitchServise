Модуль для streamer.bot, расширяющий функционал взаимодействия со стриминговой площадкой Twitch.

При инициализации модуль проверяет наличие в стримерботе глобальной переменной **twitch_todays_viewers** и создаёт её, если она отсутствует. В дальшейшейм "новые" пользователи будут записываться в неё.

Для общего использования готовы функции:

- AddFirstWordViewer -- берёт имя пользователя из переменной **userName** и добавляет его в список **twitch_todays_viewers**. При использовании с триггером **FirstWord** позволяет добавить в список человека, который написал в чат до того, как его заметил PresentViewers.
    
- ClearTodayViewers -- позволяет очистить список новых зрителей. Вы можете вызывать его в любой удобный момент. У меня оно вызвыается по триггеру старта стрима.
    
- GetNewViewers -- получает текущий список зрителей, сравнивает его с переменной **twitch_todays_viewers**, дописывает в неё тех зрителей, которые там отсутствуют и отправляет в minichat по событию на каждого такого зрителя. Список зрителей берётся из переменной **viewers**, которая заполняется по встроенному в стримербот таймеру Presen Viewers (его надо специально включить в настройках)
    
- GetPresentViewersNameList -- получает текущий список зрителей и заносит их ники в lastTwitchViewersNameList. Список зрителей берётся из переменной **viewers**, которая заполняется по встроенному в стримербот таймеру Presen Viewers (его надо специально включить в настройках). Делалось для модуля RankiSystem.

Полноценный гайд по настройке модуля можно найти тут: https://dzen.ru/a/ZcUZbAlgaiwIu30Q

Зависимости:

  1. MiniChat: https://t.me/streamix_group/3
  2. Streamer.bot: https://streamer.bot/
  3. Итеграция миничата в стримербот: https://docs.play-code.ru/minichat

## License
This project is licensed under the [Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License](https://creativecommons.org/licenses/by-nc-sa/4.0/).  
**You are free to:**
- Share — copy and redistribute the code.
- Adapt — modify, transform, and build upon the code.

**Under the following terms:**
- Attribution — You must give appropriate credit.
- NonCommercial — You may not use the material for commercial purposes.
- ShareAlike — If you remix or modify the code, you must distribute your contributions under the same license.

See the [LICENSE](LICENSE) file for the full legal text.
=======