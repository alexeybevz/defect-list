# Базовый официальный образ SQL Server для Linux
FROM mcr.microsoft.com/mssql/server:2022-latest

# Переключимся на root для установки инструментов
USER root

# Обновляем пакеты и ставим инструменты командной строки sqlcmd (mssql-tools18)
# + unixODBC-dev (драйвер) — официальный способ
RUN apt-get update \
 && apt-get install -y curl gnupg apt-transport-https software-properties-common \
 && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
 && curl -o /etc/apt/sources.list.d/mssql-release.list https://packages.microsoft.com/config/ubuntu/22.04/prod.list \
 && apt-get update \
 && ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev \
 && echo 'export PATH="$PATH:/opt/mssql-tools18/bin"' >> /etc/profile \
 && ln -s /opt/mssql-tools18/bin/sqlcmd /usr/local/bin/sqlcmd \
 && apt-get clean && rm -rf /var/lib/apt/lists/*

# Копируем стартовый скрипт и SQL-скрипты
COPY docker-entrypoint.sh /docker-entrypoint.sh
COPY sql /docker-entrypoint-initdb.d

# На Windows файлы .sh часто с CRLF — убираем \r и делаем исполняемым
RUN sed -i 's/\r$//' /docker-entrypoint.sh \
 && chmod +x /docker-entrypoint.sh

# По умолчанию контейнер будет запускаться через этот скрипт
CMD ["/docker-entrypoint.sh"]