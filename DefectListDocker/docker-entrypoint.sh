#!/bin/bash
set -e

# Запускаем SQL Server в фоне
/opt/mssql/bin/sqlservr &

# Ждём готовности сервера (пробуем коннектиться sqlcmd-ом в цикле)
echo "Waiting for SQL Server to be available..."
for i in {1..60}; do
  if /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "${SA_PASSWORD}" -C -Q "SELECT 1" &>/dev/null; then
    echo "SQL Server is up."
    break
  fi
  echo "  ...retry $i/60"
  sleep 2
done

# Прогоняем все .sql в алфавитном порядке (00_init.sql, 01_..., 02_... и т.д.)
if [ -d "/docker-entrypoint-initdb.d" ]; then
  for f in /docker-entrypoint-initdb.d/*.sql; do
    [ -e "$f" ] || continue
    echo "Running $f ..."
    /opt/mssql-tools18/bin/sqlcmd -S localhost -C -U sa -P "${SA_PASSWORD}" \
      -v DB_NAME="${DB_NAME}" \
      -i "$f"
  done
fi

# Держим основной процесс живым
wait