#!/usr/bin/env bash

exec() {
  stdout="/tmp/out-$RANDOM"
  ("$@" > "$stdout") || cat "$stdout"
  rm "$stdout"
}

ef-all() {
  exec dotnet build Courier.SqliteMigrations
  dotnet ef "$@" --startup-project Courier --project Courier.SqliteMigrations -- --provider sqlite
  exec dotnet build Courier.PostgresMigrations
  dotnet ef "$@" --startup-project Courier --project Courier.PostgresMigrations -- --provider postgres
}

if [ "$1" = "add" ]; then
  ef-all migrations add "$2"
elif [ "$1" = "push" ]; then
  ef-all database update
elif [ "$1" = "--" ]; then
  ef-all "${@:2}"
else
  echo "Invalid arguments"
  exit 1
fi


