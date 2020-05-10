# Npu Rozklad App
National Pedagogical Dragomanov University timetable application with telegram bot frontend

The app is built upon working solution using this as ["API"](https://ei.npu.edu.ua/Client/Client/) <sub>~~and that api is really bad don't look at the source code and do not even try to look at what's server returns to the browser~~</sub>


---
## Project structure:

### [NpuRozklad.Core](https://github.com/matryosha/Npu-Rozklad-App/tree/v3/Src/NpuRozklad.Core)

The core of the application. All entities that shared between other parts of the program as well as interfaces that must be implemented somewhere and could be used across the app.

### [NpuRozklad](https://github.com/matryosha/Npu-Rozklad-App/tree/v3/Src/NpuRozklad)

Implement some common staff from Core and also managing localization.

### [NpuRozklad.LessonsProvider](https://github.com/matryosha/Npu-Rozklad-App/tree/v3/Src/NpuRozklad.LessonsProvider)

Does the dirty stuff working with the university's server to provide handy [api](https://github.com/matryosha/Npu-Rozklad-App/blob/master/Src/NpuRozklad.Core/Interfaces/ILessonsProvider.cs) for getting classes.
Caches some things but still serialization takes time.

### [NpuRozklad.Persistence](https://github.com/matryosha/Npu-Rozklad-App/tree/v3/Src/NpuRozklad.Persistence)

For storing users with their faculty groups. Uses Ef Core for implementing [DAO](https://github.com/matryosha/Npu-Rozklad-App/blob/master/Src/NpuRozklad.Core/Interfaces/IRozkladUsersDao.cs).

### NpuRozklad.Subscribtion

Not implemented in v3 but was [working?](https://github.com/matryosha/Npu-Rozklad-App/tree/v2/RozkladSubscribeModule) in v2 so can be ported
### [NpuRozklad.Telegram](https://github.com/matryosha/Npu-Rozklad-App/tree/v3/Src/NpuRozklad.Telegram)

Using [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) encapsulates logic working with telegram api and provide app functionality through that. Uses separated entity to store telegram users so persistence not managed by NpuRozklad.Persistence but rather inside NpuRozklad.Telegram project. Not sure if this a good idea to split the domain user entity into two.

### [NpuRozklad.Web](https://github.com/matryosha/Npu-Rozklad-App/tree/v3/Src/NpuRozklad.Web)

Ties up everything with adding logging and configuring all parts. Endpoint for telegram api webhook using asp net core

---

There is also tests but they are dumb and ugly <sub><sup>~~like Soda~~</sup></sub>
