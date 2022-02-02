#!/usr/bin/env bash

build_dir="./build"
publish_dir="./publish"

restore() {
  dotnet restore "Courier/Courier.csproj"
  dotnet restore "Courier.PostgresMigrations/Courier.PostgresMigrations.csproj"
  dotnet restore "Courier.SqliteMigrations/Courier.SqliteMigrations.csproj"  
}

build() {
  dotnet build "Courier/Courier.csproj" -c Release -o "$build_dir"
  dotnet build "Courier.PostgresMigrations/Courier.PostgresMigrations.csproj" -c Release -o "$build_dir"
  dotnet build "Courier.SqliteMigrations/Courier.SqliteMigrations.csproj" -c Release -o "$build_dir"
}

publish() {
  dotnet publish "Courier/Courier.csproj" -c Release -o "$publish_dir"
  cp "$build_dir/Courier.PostgresMigrations.dll" "$publish_dir"
  cp "$build_dir/Courier.SqliteMigrations.dll" "$publish_dir"
}

cleanup() {
  (rm -rf "$build_dir" &> /dev/null) || true
}

(restore && build && publish) || true
cleanup
