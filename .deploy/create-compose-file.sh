#!/bin/bash

bot_token="$1"
connection_string="$2"
db_pass="$3"

echo "$bot_token" \
"$connection_string" \
"$db_pass"

cat <<EOF > compose-override.yml
version: "3.8"
services:
  backend:
    environment:
      NPU_ROZKLAD_BotToken: $bot_token
      NPU_ROZKLAD_MySqlConnectionString: $connection_string

  db:
    environment:
      MYSQL_ROOT_PASSWORD: $db_pass

EOF