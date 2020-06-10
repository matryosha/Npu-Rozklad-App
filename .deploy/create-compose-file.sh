#!/bin/bash

bot_token=$BOT_TOKEN
connection_string=$CONNECTION_STRING
db_pass=$DB_PASS

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